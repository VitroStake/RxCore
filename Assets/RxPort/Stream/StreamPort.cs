using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Assertions;

namespace VitroStake.RxPort {
  public interface IStreamSource {
    IDisposable Subscribe(Action action, GameObject gameObject);
    IDisposable Subscribe<TPayload>(Action<TPayload> action, GameObject gameObject);
  }

  internal class StreamSource<TStreamId, TNotice> : IStreamSource
    where TStreamId : struct
    where TNotice : Enum {

    public IDisposable Subscribe(Action action, GameObject gameObject) {
      Assert.IsNotNull(gameObject);

      var observable = SubjectStore<TStreamId, TNotice, Unit>.GetOrCreateObservable(_id, _notice);
      return observable
        .TakeUntilDestroy(gameObject)
        .Subscribe(_ =>
        {
          action();
        });
    }

    public IDisposable Subscribe<TPayload>(Action<TPayload> action, GameObject gameObject) {
      Assert.IsNotNull(gameObject);

      var observable = SubjectStore<TStreamId, TNotice, TPayload>.GetOrCreateObservable(_id, _notice);
      return observable
        .TakeUntilDestroy(gameObject)
        .Subscribe(payload =>
        {
          action(payload);
        });
    }

    public StreamSource(TStreamId id, TNotice notice) {
      _id = id;
      _notice = notice;
    }

    private readonly TStreamId _id;
    private readonly TNotice _notice;
  }

  public static class Stream {
    public static void OnNext<TStreamId, TNotice>(TStreamId id, TNotice notice)
      where TStreamId : struct
      where TNotice : Enum {

      var observer = SubjectStore<TStreamId, TNotice, Unit>.GetOrCreateObserver(id, notice);
      observer.OnNext(Unit.Default);
    }

    public static void OnNext<TStreamId, TNotice, TPayload>(TStreamId id, TNotice notice, TPayload payload)
      where TStreamId : struct
      where TNotice : Enum {

      if (payload == null)
        throw new ArgumentNullException();

      var observer = SubjectStore<TStreamId, TNotice, TPayload>.GetOrCreateObserver(id, notice);
      observer.OnNext(payload);
    }

    public static IStreamSource Of<TStreamId, TNotice>(TStreamId id, TNotice notice)
      where TStreamId : struct
      where TNotice : Enum {

      return new StreamSource<TStreamId, TNotice>(id, notice);
    }
  }

  public abstract class StreamPort {
    protected static class Stream {
      public static IObservable<Unit> Of<TStreamId, TNotice>(TStreamId id, TNotice notice)
        where TStreamId : struct
        where TNotice : Enum {

        var observable = SubjectStore<TStreamId, TNotice, Unit>.GetOrCreateObservable(id, notice);
        Assert.IsNotNull(StreamDisposer.Instance);
        return observable.TakeUntilDestroy(StreamDisposer.Instance);
      }
    }

    // In order to pay attention to the type of a payload, Stream<TPayload> is divided from Stream.
    protected class Stream<TPayload> {
      public static IObservable<TPayload> Of<TStreamId, TNotice>(TStreamId id, TNotice notice)
        where TStreamId : struct
        where TNotice : Enum {

        var observable = SubjectStore<TStreamId, TNotice, TPayload>.GetOrCreateObservable(id, notice);
        Assert.IsNotNull(StreamDisposer.Instance);
        return observable.TakeUntilDestroy(StreamDisposer.Instance);
      }
    }

    public void Open() {
      Assert.IsFalse(_opening);
      Assert.IsTrue(IsValid);

      var streams = SubscribeStreams();
      Register(streams);

      _opening = true;

      PortStore.UpdateOrAddPort(this);
    }

    public void Close() {
      _disposables.Clear();
      _opening = false;
    }

    protected void Register(params IDisposable[] streams) {
      foreach (var stream in streams)
        _disposables.Add(stream);
    }

    protected abstract IDisposable[] SubscribeStreams();
    public abstract bool IsValid { get; }

    public bool IsOpen  => _opening;
    public bool IsClose => !_opening;

    public StreamPort() {
      Assert.IsNotNull(StreamDisposer.Instance);

      StreamDisposer.Instance.OnDestroyAsObservable()
        .Subscribe(_ =>
        {
          // You have to manually dispose a CompositeDisposable because it's not automatically disposed.
          _disposables.Dispose();
        });
    }

    protected CompositeDisposable _disposables = new();
    private bool _opening;
  }
}

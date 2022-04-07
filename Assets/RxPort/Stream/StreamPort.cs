using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Assertions;

namespace VitroStake.RxPort {
  using Internal;

  public abstract class StreamPort : StreamFaucet {
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

    public StreamPort Open() {
      Assert.IsFalse(_opening);
      Assert.IsTrue(IsValid);

      var streams = SubscribeStreams();
      Register(streams);

      _opening = true;

      PortStore.UpdateOrAddPort(this);

      return this;
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

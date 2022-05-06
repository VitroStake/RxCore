using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;
using UnityEngine.Assertions;

namespace VitroStake.RxPort {
  public interface IStreamSource {
    IDisposable Subscribe(Action action, GameObject gameObject);
    IDisposable Subscribe<TPayload>(Action<TPayload> action, GameObject gameObject);
  }

  internal struct StreamSource<TStreamId, TNotice> : IStreamSource
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
}

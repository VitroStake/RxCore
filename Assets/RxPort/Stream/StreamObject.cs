using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;
using UnityEngine.Assertions;

namespace VitroStake.RxPort {
  using Internal;

  public abstract class StreamObject {
    protected static class Stream {
      public static void OnNext<TNotice>(TNotice notice) where TNotice : Enum {
        var observer = SubjectStore<TNotice, Unit>.GetOrCreateObserver(notice);
        observer.OnNext(Unit.Default);
      }

      public static IObservable<Unit> Of<TNotice>(TNotice notice) where TNotice : Enum {
        var observable = SubjectStore<TNotice, Unit>.GetOrCreateObservable(notice);
        Assert.IsNotNull(StreamDisposer.Instance);
        return observable.TakeUntilDestroy(StreamDisposer.Instance);
      }
    }

    protected static class Stream<TPayload> {
      public static void OnNext<TNotice>(TNotice notice, TPayload payload) where TNotice : Enum {
        if (payload == null)
          throw new ArgumentNullException();

        var observer = SubjectStore<TNotice, TPayload>.GetOrCreateObserver(notice);
        observer.OnNext(payload);
      }

      public static IObservable<TPayload> Of<TNotice>(TNotice notice) where TNotice : Enum {
        var observable = SubjectStore<TNotice, TPayload>.GetOrCreateObservable(notice);
        Assert.IsNotNull(StreamDisposer.Instance);
        return observable.TakeUntilDestroy(StreamDisposer.Instance);
      }
    }
  }
}

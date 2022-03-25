using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using System;
using UnityEngine.Assertions;

namespace VitroStake.RxPort {
  using Internal;

  public static class Stream {
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

  public static class Stream<TPayload> {
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

namespace VitroStake.RxPort.Internal {
  internal static class SubjectStore<TNotice, TPayload> where TNotice : Enum {
    public static IObserver<TPayload> GetOrCreateObserver(TNotice notice) {
      if (!_subjects.ContainsKey(notice))
        _subjects[notice] = new Subject<TPayload>();

      return _subjects[notice];
    }

    public static IObservable<TPayload> GetOrCreateObservable(TNotice notice) {
      if (!_subjects.ContainsKey(notice))
        _subjects[notice] = new Subject<TPayload>();

      return _subjects[notice];
    }

    // _subjects doesn't need to be reset in Domain Reloading
    private static Dictionary<TNotice, Subject<TPayload>> _subjects = new();
  }
}

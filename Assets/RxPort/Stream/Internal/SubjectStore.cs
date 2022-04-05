using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using System;

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

    // RuntimeInitializeOnLoadMethod is not called in Generic classes,
    // so they have no means of initializing static fields without domain reload.
    // But in case of SubjectStore, _subjects doesn't need to be initialized other than capacity issue,
    // because it doesn't have any states.
    private static Dictionary<TNotice, Subject<TPayload>> _subjects = new();
  }
}

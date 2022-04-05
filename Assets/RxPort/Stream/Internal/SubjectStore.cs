using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using System;

namespace VitroStake.RxPort.Internal {
  internal static class SubjectStore<TNotice, TPayload> where TNotice : Enum {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeInitialize() {
      _subjects = new();
    }

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

    private static Dictionary<TNotice, Subject<TPayload>> _subjects = new();
  }
}

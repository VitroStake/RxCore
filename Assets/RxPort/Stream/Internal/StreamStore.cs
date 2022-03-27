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

    // _subjects doesn't need to be reset in Domain Reloading
    private static Dictionary<TNotice, Subject<TPayload>> _subjects = new();
  }
}

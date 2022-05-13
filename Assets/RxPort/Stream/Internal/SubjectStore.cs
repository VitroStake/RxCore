using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using System;

namespace VitroStake.RxPort {
  internal static class SubjectStore<TStreamId, TNotice, TPayload>
    where TStreamId : struct
    where TNotice : Enum {

    public static IObserver<TPayload> GetOrCreateObserver(TStreamId id, TNotice notice) {
      if (!_subjects.ContainsKey((id, notice)))
        _subjects[(id, notice)] = new Subject<TPayload>();

      return _subjects[(id, notice)];
    }

    public static IObservable<TPayload> GetOrCreateObservable(TStreamId id, TNotice notice) {
      if (!_subjects.ContainsKey((id, notice)))
        _subjects[(id, notice)] = new Subject<TPayload>();

      return _subjects[(id, notice)];
    }

    // RuntimeInitializeOnLoadMethod is not called in Generic classes,
    // so they have no means of initializing static fields without domain reload.
    // But in case of SubjectStore, _subjects doesn't need to be initialized other than capacity issue,
    // because it doesn't have any states.
    private static Dictionary<(TStreamId, TNotice), Subject<TPayload>> _subjects = new Dictionary<(TStreamId, TNotice), Subject<TPayload>>(); // if C#9, use new()
  }
}

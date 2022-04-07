using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;

namespace VitroStake.RxPort {
  using Internal;

  public abstract class StreamFaucet {
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
  }
}

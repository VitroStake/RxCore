using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace VitroStake.RxPort {
  public static class PortStore {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeInitialize() {
      _ports = new();
    }

    public static bool HasPort(Type type) {
      return _ports.ContainsKey(type);
    }

    public static bool TryGetPort<TPort>(out TPort port) where TPort : StreamPort {
      return TryGetPortImpl(out port, p => true);
    }

    public static bool TryGetOpenPort<TPort>(out TPort port) where TPort : StreamPort {
      return TryGetPortImpl(out port, p => p.IsOpen);
    }

    public static bool TryGetClosePort<TPort>(out TPort port) where TPort : StreamPort {
      return TryGetPortImpl(out port, p => p.IsClose);
    }

    private static bool TryGetPortImpl<TPort>(out TPort port, Func<StreamPort, bool> condition) where TPort : StreamPort {
      var type = typeof(TPort);
      port = null;

      if (HasPort(type)) {
        var target = (TPort)_ports[type];

        if (target.IsValid && condition(target)) {
          port = target;
          return true;
        }
      }

      return false;
    }

    internal static void UpdateOrAddPort(StreamPort port) {
      _ports[port.GetType()] = port;
    }

    private static Dictionary<Type, StreamPort> _ports = new();
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;
using UnityEngine.Assertions;

namespace VitroStake.RxPort {
  public abstract class StreamPort {
    public StreamPort() {
      _disposables = new CompositeDisposable();
    }

    public StreamPort Open() {
      Assert.IsFalse(_opening);
      Assert.IsTrue(IsValid);

      OpenCore();
      _opening = true;

      StreamPortStore.UpdateOrAddProcess(this);

      return this;
    }

    public void Close() {
      _disposables.Clear();
      _opening = false;
    }

    public bool IsOpen  => _opening;
    public bool IsClose => !_opening;

    protected abstract void OpenCore();
    public abstract bool IsValid { get; }

    protected void Register(params IDisposable[] streams) {
      foreach (var stream in streams)
        _disposables.Add(stream);
    }

    protected CompositeDisposable _disposables;

    // CompositeDisposable is not automatically disposed
    ~StreamPort() {
      _disposables.Dispose();
    }

    private bool _opening;
  }

  public static class StreamPortStore {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeInitialize() {
      _ports = new Dictionary<Type, StreamPort>();
    }

    public static bool HasPort(Type type) {
      return _ports.ContainsKey(type);
    }

    public static bool TryGetPort<TPort>(out TPort port) where TPort : StreamPort {
      return TryGetProcessImpl(out port, p => true);
    }

    public static bool TryGetOpenPort<TPort>(out TPort port) where TPort : StreamPort {
      return TryGetProcessImpl(out port, p => p.IsOpen);
    }

    public static bool TryGetClosePort<TPort>(out TPort port) where TPort : StreamPort {
      return TryGetProcessImpl(out port, p => p.IsClose);
    }

    private static bool TryGetProcessImpl<TPort>(out TPort port, Func<StreamPort, bool> condition) where TPort : StreamPort {
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

    internal static void UpdateOrAddProcess(StreamPort port) {
      _ports[port.GetType()] = port;
    }

    private static Dictionary<Type, StreamPort> _ports;
  }
}

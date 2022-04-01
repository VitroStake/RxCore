using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;
using UnityEngine.Assertions;

namespace VitroStake.RxPort {
  public abstract class StreamPort : StreamObject {
    public StreamPort Open() {
      Assert.IsFalse(_opening);
      Assert.IsTrue(IsValid);

      OpenCore();
      _opening = true;

      PortStore.UpdateOrAddPort(this);

      return this;
    }

    public void Close() {
      _disposables.Clear();
      _opening = false;
    }

    protected void Register(params IDisposable[] streams) {
      foreach (var stream in streams)
        _disposables.Add(stream);
    }

    protected abstract void OpenCore();
    public abstract bool IsValid { get; }

    public bool IsOpen  => _opening;
    public bool IsClose => !_opening;

    public StreamPort() {
      _disposables = new CompositeDisposable();
    }

    // CompositeDisposable is not automatically disposed
    ~StreamPort() {
      _disposables.Dispose();
    }

    protected CompositeDisposable _disposables;
    private bool _opening;
  }
}

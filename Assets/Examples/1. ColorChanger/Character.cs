using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using System;

namespace VitroStake.RxPort {
  public class Character : MonoBehaviour {
    void Awake() {
      new Port(this, _iniArg).Open();
    }

    [SerializeField] private IniArg _iniArg;

    [Serializable]
    private struct IniArg {
      public MeshRenderer Renderer;
      public Color Color;
    }

    private class Port : StreamPort {
      protected override IDisposable[] SubscribeStreams() {
        return new[] {
          Stream.Of(_id, Notice.Character.ChangeColor)
            .Subscribe(_ =>
            {
              _model.ChangeColor();
            })
        };
      }

      public override bool IsValid => _model != null;

      public Port(Character character, IniArg iniArg) {
        _id = character.GetInstanceID();
        _model = new(iniArg);
      }

      private readonly int _id;
      private readonly Model _model;
    }

    private class Model {
      public Model(IniArg ini) {
        _renderer = ini.Renderer;
        _color = ini.Color;
      }

      public void ChangeColor() {
        _renderer.material.color = _color;
      }

      private MeshRenderer _renderer;
      private Color _color;
    }
  }
}

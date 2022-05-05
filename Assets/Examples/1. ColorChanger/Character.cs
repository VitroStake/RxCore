using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VitroStake.RxPort {
  public class Character : MonoBehaviour {
    void Awake() {
      var id = GetInstanceID();
      Stream.Of(id, Notice.Character.ChangeColor).Subscribe(ChangeColor, gameObject);
    }

    public void ChangeColor() {
      _renderer.material.color = _color;
    }

    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Color _color;
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VitroStake.RxPort {
  public class ColorChanger : MonoBehaviour {
    void Start() {
      OnNext(_player, _enemy);
    }

    private void OnNext(Character player, Character enemy) {
      var playerId = player.GetInstanceID();
      Stream.OnNext(playerId, Notice.Character.ChangeColor);

      var enemyId = enemy.GetInstanceID();
      Stream.OnNext(enemyId, Notice.Character.ChangeColor);
    }

    [SerializeField] private Character _player;
    [SerializeField] private Character _enemy;
  }
}

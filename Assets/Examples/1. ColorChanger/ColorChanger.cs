using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VitroStake.RxPort {
  public class ColorChanger : MonoBehaviour {
    void Start() {
      Util.OnNext(_player, _enemy);
    }

    [SerializeField] private Character _player;
    [SerializeField] private Character _enemy;

    private class Util : StreamFaucet {
      public static void OnNext(Character player, Character enemy) {
        var playerId = player.GetInstanceID();
        OnNext(playerId, Notice.Character.ChangeColor);

        var enemyId = enemy.GetInstanceID();
        OnNext(enemyId, Notice.Character.ChangeColor);
      }
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VitroStake.RxPort.Internal {
  internal static class StreamDisposer {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeInitialize() {
      Instance = new GameObject("StreamDisposer");

      // OnDestroy() is called when you quit the game
      GameObject.DontDestroyOnLoad(Instance);
    }

    public static GameObject Instance { get; private set; }
  }
}

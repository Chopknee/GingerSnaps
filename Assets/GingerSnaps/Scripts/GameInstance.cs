using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps {
	public class GameInstance : MonoBehaviour {

		private void Awake() {
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 120;
		}

		private static GameInstance _Instance = null;
		public static GameInstance Instance {get; private set;}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RuntimeInitialization() {
			if (_Instance != null)
				return;

			_Instance = new GameObject("GingerSnaps.GameInstance").AddComponent<GameInstance>();
			DontDestroyOnLoad(_Instance.gameObject);
		}

	}
}
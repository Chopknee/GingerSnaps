using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCapturer : MonoBehaviour {
	#if UNITY_EDITOR

	private static ScreenCapturer instance = null;
	private int number = 0;
	
	public void Update() {
		//Debug.Log("Hello?");
		if (UnityEngine.InputSystem.Keyboard.current.homeKey.ReadValue() > 0.5f) {
			UnityEngine.ScreenCapture.CaptureScreenshot("screenshot_" + number + ".png");
			number ++;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void OnBeforeSceneLoadRuntimeMethod() {
		GameObject go = new GameObject("Dugan.ScreenCapturer");
		instance = go.AddComponent<ScreenCapturer>();
		DontDestroyOnLoad(go);
	}

	#endif
}

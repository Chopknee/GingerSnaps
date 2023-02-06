using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Scenes.Game {
	public class Scene : MonoBehaviour {

		private new Camera camera = null;

		private Player.Controller player = null;
		private Player.CameraControllerFixedAngle playerCamera = null;

		private PPFx.SickProfileManager sickScreenEffect = null;

		private static Scene Instance = null;

		private void Awake() {
			player = transform.Find("Player").gameObject.AddComponent<Player.Controller>();
			player.jumpVelocity = 15.0f;
			
			playerCamera = new GameObject("PlayerCamera").AddComponent<Player.CameraControllerFixedAngle>();
			playerCamera.transform.SetParent(transform);
			playerCamera.target = player.transform;
			playerCamera.forwardAngle = (Vector3.forward * 2.0f + Vector3.down).normalized;
			playerCamera.transform.forward = playerCamera.forwardAngle;
			playerCamera.followDistance = 23.0f;

			camera = transform.Find("Camera").GetComponent<Camera>();
			camera.transform.SetParent(playerCamera.transform);
			camera.transform.localPosition = Vector3.zero;
			camera.transform.localScale = Vector3.one;
			camera.transform.localRotation = Quaternion.identity;

			player.viewpoint = playerCamera.transform;
			player.groundLayerMask = camera.cullingMask;

			Popups.HUD.Popup hud = Dugan.PopupManager.Load<Popups.HUD.Popup>();
			hud.PostAwake();
			hud.SetDirection(1);

			sickScreenEffect = gameObject.AddComponent<PPFx.SickProfileManager>();
			
			Instance = this;
		}

		public IEnumerator SickEnumerator() {
			Debug.Log("SOMETHING");
			sickScreenEffect.SetDirection(-1);
			yield return new WaitForSeconds(10.0f);
			sickScreenEffect.SetDirection(-1);
		}

		public static void DoSick() {
			Instance.StartCoroutine(Instance.SickEnumerator());
		}

	}
}
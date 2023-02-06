using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GingerSnaps.Scenes.Game {
	public class Scene : MonoBehaviour {

		public float gameLength = 60.0f;

		private new Camera camera = null;

		private Player.Controller player = null;
		private Player.CameraControllerFixedAngle playerCamera = null;

		private PPFx.SickProfileManager sickScreenEffect = null;

		private static Scene Instance = null;

		private Popups.Intro.Popup intro = null;

		private Popups.HUD.Popup hud = null;

		public int score = 0;
		private float sickTime = 0.0f;

		private bool bPlaying = false;

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

			Transform pots = transform.Find("Pots");
			for (int i = 0; i < pots.childCount; i++) {
				Destructable des = pots.GetChild(i).GetComponent<Destructable>();
				if (des == null)
					continue;
				des.OnBroken += OnPotBroken;
			}

			sickScreenEffect = gameObject.AddComponent<PPFx.SickProfileManager>();
			
			Instance = this;

			hud = Dugan.PopupManager.Load<Popups.HUD.Popup>();
			hud.PostAwake();
			hud.SetDirection(-1, true);

			intro = Dugan.PopupManager.LoadNoAdd<Popups.Intro.Popup>();
			intro.transform.position = new Vector3(2000.0f, 0.0f, 0.0f);
			intro.PostAwake();
			intro.OnClosed += OnIntroClosed;
		}

		private IEnumerator Game() {
			// Allow the game to run for a while.
			yield return new WaitForSeconds(gameLength);
			// Display score for a short period
			hud.SetDirection(1);
			yield return new WaitForSeconds(1.0f);
			hud.SetScoreValue(score);
			// Restart the game
			yield return new WaitForSeconds(10.0f);
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public static void AddScore(int score) {
			score += score;
		}

		private void Update() {
			if (sickScreenEffect.GetDirection() > 0) {
				sickTime += Time.deltaTime;
				if (sickTime >= 10.0f)
					sickScreenEffect.SetDirection(-1);
			}
		}

		private void OnIntroClosed(Popup popup) {
			if (bPlaying)
				return;

			bPlaying = true;
			StartCoroutine(Game());
		}

		private void OnPotBroken(Destructable destructable) {
			AddScore(destructable.scoreValue);
			if (destructable.bDoCatnip) {
				sickScreenEffect.SetDirection(1);
			}
		}

	}
}
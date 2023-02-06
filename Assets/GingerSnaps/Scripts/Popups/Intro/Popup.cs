using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Popups.Intro {
	public class Popup : GingerSnaps.Popup {

		private new Camera camera = null;

		private RectTransform content = null;
		private CanvasGroup canvasGroup = null;

		private Dugan.UI.Button btnStart = null;
		private Dugan.UI.Button btnQuit = null;

		protected override void Awake() {
			camera = transform.Find("Camera").GetComponent<Camera>();

			content = transform.Find("Content") as RectTransform;
			canvasGroup = content.GetComponent<CanvasGroup>();

			btnStart = content.Find("BtnStart").gameObject.AddComponent<UI.ButtonGraphics>().button;
			btnStart.OnPointerUp += OnClickBtnStart;

			btnQuit = content.Find("BtnQuit").gameObject.AddComponent<UI.ButtonGraphics>().button;
			btnQuit.OnPointerUp += OnClickBtnQuit;

			gameObject.name = "Intro.Popup";

			base.Awake();
		}

		private void OnClickBtnStart(Dugan.Input.PointerTarget target, string args) {
			SetDirection(-1);
		}

		private void OnClickBtnQuit(Dugan.Input.PointerTarget target, string args) {
			Application.Quit();
		}

		protected override void OnResize() {
			camera.orthographicSize = Dugan.Screen.layoutSize.y * 0.5f;
			content.sizeDelta = Dugan.Screen.layoutSize;
		}

		private void Update() {
			if (Input.start.bPressed) {
				SetDirection(timeAnimation.GetDirection() > 0? -1 : 1);
			}
		}

		protected override void OnAnimationUpdate() {
			float a = timeAnimation.GetNormalizedTime();
			a = Dugan.Mathf.Easing.EaseInOutCirc(a);
			canvasGroup.alpha = a;
			Time.timeScale = 1.0f - a;
		}
	}
}
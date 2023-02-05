using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Popups.Intro {
	public class Popup : GingerSnaps.Popup {

		private new Camera camera = null;

		private RectTransform content = null;

		private Dugan.UI.Button btnStart = null;
		private Dugan.UI.Button btnQuit = null;

		protected override void Awake() {
			camera = transform.Find("Camera").GetComponent<Camera>();

			content = transform.Find("Content") as RectTransform;

			btnStart = content.Find("BtnStart").gameObject.AddComponent<UI.ButtonGraphics>().button;
			btnStart.OnPointerUp += OnClickBtnStart;

			btnQuit = content.Find("BtnQuit").gameObject.AddComponent<UI.ButtonGraphics>().button;
			btnQuit.OnPointerUp += OnClickBtnQuit;

			base.Awake();
			base.PostAwake();
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

		protected override void OnAnimationUpdate() {
			
		}

	}
}
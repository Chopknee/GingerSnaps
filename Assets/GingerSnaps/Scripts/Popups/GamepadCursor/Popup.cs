using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Popups.GamepadCursor {
	public class Popup : GingerSnaps.Popup {

		private new Camera camera = null;

		private RectTransform content = null;

		private Transform cursor = null;

		private bool bSetInitialPosition = false;

		private float hideTimer = 0.0f;

		private Vector2 lastPos = Vector2.zero;

		protected override void Awake() {
			camera = transform.Find("Camera").GetComponent<Camera>();

			content = transform.Find("Content") as RectTransform;

			cursor = content.Find("Cursor");

			base.Awake();
		}

		private void Update() {
			if (!bSetInitialPosition) {
				GamepadPointer.SetPosition(Dugan.Screen.screenSizeInUnits * 0.5f);
				bSetInitialPosition = true;
			}

			cursor.localPosition = GamepadPointer.gamepadPointer.position * 2.0f;

			if (hideTimer < 5.0f) {
				hideTimer += Time.deltaTime;
			}

			cursor.gameObject.SetActive(hideTimer < 5.0f);

			if (GamepadPointer.gamepadPointer.position != lastPos)
				hideTimer = 0.0f;

			lastPos = GamepadPointer.gamepadPointer.position;
		}

		protected override void OnResize() {
			camera.orthographicSize = Dugan.Screen.layoutSize.y * 0.5f;
			content.sizeDelta = Dugan.Screen.layoutSize;
		}

	}
}
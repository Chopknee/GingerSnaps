using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Popups.GamepadCursor {
	public class Popup : GingerSnaps.Popup {

		private new Camera camera = null;

		private RectTransform content = null;

		private Transform cursor = null;

		protected override void Awake() {
			camera = transform.Find("Camera").GetComponent<Camera>();

			content = transform.Find("Content") as RectTransform;

			cursor = content.Find("Cursor");

			base.Awake();
		}

		private void Update() {
			cursor.localPosition = GamepadPointer.position * 2.0f;
		}

		protected override void OnResize() {
			camera.orthographicSize = Dugan.Screen.layoutSize.y * 0.5f;
			content.sizeDelta = Dugan.Screen.layoutSize;
		}

	}
}
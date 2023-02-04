using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Popups.HUD {
	public class Popup : GingerSnaps.Popup {

		private new Camera camera = null;

		private RectTransform content = null;

		protected override void Awake() {
			camera = transform.Find("Camera").GetComponent<Camera>();

			content = transform.Find("Content") as RectTransform;

			base.Awake();
		}

		protected override void OnResize() {
			camera.orthographicSize = Dugan.Screen.layoutSize.y * 0.5f;
			content.sizeDelta = Dugan.Screen.layoutSize;
		}

	}
}
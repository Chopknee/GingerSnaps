using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.UI {
	public class ButtonGraphics : MonoBehaviour {

		public Dugan.UI.Button button {get; private set;}

		private void Awake() {
			button = gameObject.AddComponent<Dugan.UI.Button>();
			button.tintOnClick = true;
		}

		private void Update() {
			float targetScale = 1.0f;
			if (button.GetPointerOver())
				targetScale = 1.3f;

			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, Time.deltaTime * 10.0f);
		}

	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Popups.HUD {
	public class Popup : GingerSnaps.Popup {

		private new Camera camera = null;

		private RectTransform content = null;
		private CanvasGroup canvasGroup = null;

		private Dugan.TimeAnimation scoreAnimation = null;

		private TMPro.TextMeshProUGUI txtFinalScore = null;
		private string strScoreFormat = "";

		private int scoreTarget = 0;

		protected override void Awake() {
			camera = transform.Find("Camera").GetComponent<Camera>();

			content = transform.Find("Content") as RectTransform;
			canvasGroup = content.GetComponent<CanvasGroup>();

			txtFinalScore = content.Find("TxtFinalScore").GetComponent<TMPro.TextMeshProUGUI>();
			strScoreFormat = txtFinalScore.text;

			scoreAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			scoreAnimation.SetLengthInSeconds(4.0f);
			scoreAnimation.AnimationUpdateCallback += RenderScore;
			scoreAnimation.SetDirection(-1, true);

			base.Awake();
		}

		public void SetScoreValue(int score) {
			scoreTarget = score;
			scoreAnimation.SetDirection(1);
		}

		private void RenderScore() {
			float a = scoreAnimation.GetNormalizedTime();
			a = Dugan.Mathf.Easing.EaseInOutExpo(a);
			int scoreValue = Mathf.RoundToInt(Mathf.Lerp(0, scoreTarget, a));
			txtFinalScore.text = string.Format(strScoreFormat, scoreValue.ToString("000"));
		}

		protected override void OnAnimationUpdate() {
			float a = timeAnimation.GetNormalizedTime();
			a = Dugan.Mathf.Easing.EaseInOutExpo(a);
			canvasGroup.alpha = a;
		}

		protected override void OnResize() {
			camera.orthographicSize = Dugan.Screen.layoutSize.y * 0.5f;
			content.sizeDelta = Dugan.Screen.layoutSize;
		}

	}
}
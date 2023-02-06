using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace GingerSnaps.PPFx {
	public class SickProfileManager : Dugan.TimeAnimation {

		private PostProcessVolume volume = null;

		private PPFx.Distortion.Distortion distortionEffect = null;
		private ChromaticAberration chromaticAberrationEffect = null;

		private float targetDistortionScale = 8.0f;
		private float targetDistortionImpact = 0.02f;
		private float targetCAIntensity = 0.5f;

		private void Awake() {
			volume = gameObject.AddComponent<PostProcessVolume>();
			volume.isGlobal = true;
			volume.profile = Resources.Load<PostProcessProfile>("GingerSnaps/PPFx/SickEffectProfile");
			volume.profile.TryGetSettings<PPFx.Distortion.Distortion>(out distortionEffect);
			volume.profile.TryGetSettings<ChromaticAberration>(out chromaticAberrationEffect);

			// targetDistortionScale = distortionEffect.scale.value;
			// targetDistortionImpact = distortionEffect.impact.value;
			// targetCAIntensity = chromaticAberrationEffect.intensity.value;

			AnimationUpdateCallback += OnAnimationUpdate;
			SetDirectionCallback += OnSetDirection;
			AnimationCompleteCallback += OnAnimationComplete;
			SetLengthInSeconds(4.0f);
			SetDirection(-1, true);
		}

		private void OnSetDirection() {
			if (direction > 0) {
				distortionEffect.enabled.value = true;
				chromaticAberrationEffect.enabled.value = true;
			}
		}

		private void OnAnimationUpdate() {
			float a = GetNormalizedTime();
			a = Dugan.Mathf.Easing.EaseInOutExpo(a);

			distortionEffect.scale.value = Mathf.Lerp(0.0f, targetDistortionScale, a);
			distortionEffect.impact.value = Mathf.Lerp(0.0f, targetDistortionImpact, a);
			chromaticAberrationEffect.intensity.value = Mathf.Lerp(0.0f, targetCAIntensity, a);
		}

		private void OnAnimationComplete() {
			if (direction < 0) {
				distortionEffect.enabled.value = false;
				chromaticAberrationEffect.enabled.value = false;
			}
		}

	}
}
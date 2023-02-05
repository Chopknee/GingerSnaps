using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace GingerSnaps.PPFx.Distortion {
	[System.Serializable]
	[PostProcess(typeof(DistortionRenderer), PostProcessEvent.AfterStack, "GingerSnaps/Distortion")]
	public sealed class Distortion : PostProcessEffectSettings {
		public FloatParameter scale = new FloatParameter() {value = 5};
		public FloatParameter impact = new FloatParameter() {value = 0.05f};
		public FloatParameter speed = new FloatParameter() {value = 10};
	}

	public sealed class DistortionRenderer : PostProcessEffectRenderer<Distortion> {

		public override void Render(PostProcessRenderContext context) {
			var sheet = context.propertySheets.Get(Shader.Find("Hidden/GingerSnaps/PPFx/Distortion"));
			sheet.properties.SetFloat("_Scale", settings.scale);
			sheet.properties.SetFloat("_Impact", settings.impact);
			sheet.properties.SetFloat("_Speed", settings.speed);
			context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
		}
	}
}


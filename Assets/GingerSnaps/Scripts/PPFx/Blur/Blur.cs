using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace GingerSnaps.PPFx.Blur {
	[System.Serializable]
	[PostProcess(typeof(BlurRenderer), PostProcessEvent.AfterStack, "GingerSnaps/Blur")]
	public sealed class Blur : PostProcessEffectSettings {
		public IntParameter blurDistance = new IntParameter() {value = 1};
	}

	public sealed class BlurRenderer : PostProcessEffectRenderer<Blur> {

		public override void Render(PostProcessRenderContext context) {
			var sheet = context.propertySheets.Get(Shader.Find("Hidden/GingerSnaps/PPFx/Blur"));
			sheet.properties.SetFloat("_Blur", settings.blurDistance);
			context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
		}
	}
}


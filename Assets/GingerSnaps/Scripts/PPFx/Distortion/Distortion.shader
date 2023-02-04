Shader "Hidden/GingerSnaps/PPFx/Distortion" {
	HLSLINCLUDE

		#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		#include "../ShaderIncludes/ClassicNoise3D.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

		float2 _MainTex_TexelSize;
		float _Scale;
		float _Impact;
		float _Speed;

		float4 Frag(VaryingsDefault i) : SV_Target {
			
			float3 posA = float3(i.texcoord.x * _Scale, i.texcoord.y * _Scale, _Time.x * _Speed);
			float3 posB = float3(i.texcoord.x * _Scale, i.texcoord.y * _Scale, (_Time.x-1.63584) * _Speed);
			float2 coordMod = float2((ClassicNoise(posA) * 2.0) - 1.0, (ClassicNoise(posA) * 2.0) - 1.0);
			coordMod = coordMod * _Impact;

			float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + coordMod);

			return color;
		}
	
	ENDHLSL

	SubShader {
		Cull off ZWrite off ZTest Always

		Pass {
			HLSLPROGRAM
				#pragma vertex VertDefault
				#pragma fragment Frag
			ENDHLSL
		}
	}
}

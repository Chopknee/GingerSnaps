Shader "Hidden/GingerSnaps/PPFx/Blur" {
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

        float _Blur;
        float2 _MainTex_TexelSize;

        float4 Frag(VaryingsDefault i) : SV_Target {

            //Apply the desaturation and tone

            float amountX = (1 / _MainTex_TexelSize.x) / _Blur;
            float amountY = (1 / _MainTex_TexelSize.y) / _Blur;

            float2 bottomLeftUV = i.texcoord - float2(_MainTex_TexelSize.x * amountX, _MainTex_TexelSize.y * amountY);
            float2 topRightUV = i.texcoord + float2(_MainTex_TexelSize.x * amountX, _MainTex_TexelSize.y * amountY);  
            float2 bottomRightUV = i.texcoord + float2(_MainTex_TexelSize.x * amountX, -_MainTex_TexelSize.y * amountY);
            float2 topLeftUV = i.texcoord + float2(-_MainTex_TexelSize.x * amountX, _MainTex_TexelSize.y * amountY);

            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));

            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, bottomLeftUV);
            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, bottomRightUV);
            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, topLeftUV);
            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, topRightUV);
            color = color * (1.0/5.0);

            //color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);
            //color = color * float4 (_SepiaColor.rgb, 1);

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

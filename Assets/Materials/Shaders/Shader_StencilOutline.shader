Shader "Custom/URP/SpriteStencilOutline_Fixed"
{
    Properties
    {
        [MainTexture] _MainTex("Main Tex", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineSize("Outline Size", Float) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }

            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Keep
            }

            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineSize;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 scaleOffset = float3(IN.positionOS.xy * (1 + _OutlineSize * 0.05), 0);
                OUT.positionHCS = TransformObjectToHClip(float4(scaleOffset, 1.0));
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float alpha = tex2D(_MainTex, IN.uv).a;
                return half4(_OutlineColor.rgb, alpha * _OutlineColor.a);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Sprite"
            Tags { "LightMode" = "UniversalForward" }

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, IN.uv);
                return texColor * _Color;
            }
            ENDHLSL
        }
    }
}

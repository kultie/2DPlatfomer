Shader "Hidden/TiviScreenTransitionShader"
{
    Properties
    {
        _FirstTex ("First Texture", 2D) = "white" {}
        _SecondTex ("Second Texture", 2D) = "white" {}
        _TransitionTex ("Transition Texture", 2D) = "white" {}
        _FirstTexNoise("Noise 1", 2D) = "white" {}
        _Cutoff("Progress", Range (0, 1)) = 0
        _NoiseOffset("Noise Offset", float) = 0
        
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _FirstTex;
            sampler2D _SecondTex;
            sampler2D _TransitionTex;
            sampler2D _FirstTexNoise;
            float _Cutoff;
            float _NoiseOffset;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 first_uv = i.uv;

                fixed4 firstNoiseCol = tex2D(_FirstTexNoise, i.uv);
                first_uv.x += firstNoiseCol.x * sin(_NoiseOffset);

                fixed4 firstCol = tex2D(_FirstTex, first_uv);


                fixed4 secondCol = tex2D(_SecondTex, i.uv);
                fixed4 noiseCol = tex2D(_TransitionTex, i.uv);
                float step_value = step(_Cutoff, noiseCol.r - 0.005);


                // return step_value > 0 ? firstCol : secondCol;
                return step_value * firstCol + (1 - step_value) * secondCol;
                // if(noiseCol.r > _Cutoff)
                // {   
                //     return firstCol;
                // }
                // return secondCol;
            }
            ENDCG
        }
    }
}
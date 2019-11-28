Shader "YKShaders/DistortShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("Texture", 2D ) = "black" {}
		_CircleCenterX ("CircleCenterX",Float) = 0.3
		_CircleCenterY ("CircleCenterY",Float) = 0.8
		_CircleRadius ("CircleRadius",Float) = 50
		_STGWidth ("STGWidth",Float) = 384
		_STGHeight ("STGHeight",Float) = 448
		_DistortFactor ("DistortFactor",Range(0.0,0.2)) = 0.01
		_EffectColor ("EffectColor",Color) = (0.62,0.22,0.61,1)
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Lighting Off
		Blend One OneMinusSrcAlpha

		Tags { "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"}

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _NoiseTex;
			float _CircleCenterX;
			float _CircleCenterY;
			float _CircleRadius;
			float _STGWidth;
			float _STGHeight;
			float _DistortFactor;
			float _EffectColor;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 pos = float2(_STGWidth*i.uv.x,_STGHeight*i.uv.y);
				float2 center = float2(_CircleCenterX,_CircleCenterY);
				float2 dirVec = pos - center;
				float len = length(dirVec);
				fixed4 col;
				float k = step(len,_CircleRadius);
				//float k = 1;
				float2 tmp = float2(_Time.y,_Time.x);
				float4 noise = tex2D(_NoiseTex, i.uv - tmp * 0.15);
				float2 offset = noise.xy * _DistortFactor;
				col = tex2D(_MainTex,i.uv+offset*k);
				return col;
			}
			ENDCG
		}
	}
}
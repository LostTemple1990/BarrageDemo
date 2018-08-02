Shader "YKShaders/DistortShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CircleCenterX ("CircleCenterX",Range(0,1)) = 0.3
		_CircleCenterY ("CircleCenterY",Range(0,1)) = 0.8
		_CircleRadius ("CircleRadius",Float) = 50
		_STGWidth ("STGWidth",Float) = 384
		_STGHeight ("STGHeight",Float) = 448
		_DistortFactor ("DistortFactor",Range(0.1,0.25)) = 0.15
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
				if ( len <= _CircleRadius )
				{
					float2 offset = normalize(dirVec) * (_CircleRadius - len) / _CircleRadius * _DistortFactor;
					col = tex2D(_MainTex,i.uv+offset);
					col = lerp(col,_EffectColor,1-len/_CircleRadius);
					return col;
				}
				col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
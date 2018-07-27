Shader "YKShaders/DistortShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CircleTex ("Texture", 2D) = "black" {}
		_CircleCenterX ("CircleCenterX",Range(0,1)) = 0.3
		_CircleCenterY ("CircleCenterY",Range(0,1)) = 0.8
		_CircleRadiusRatioWithWidth ("CircleRadiusRatioWithWidth",Range(0,1)) = 0.2
		_CircleRadiusRatioWithHeight ("CircleRadiusRatioWithWidth",Range(0,1)) = 0.2
		_DectectRectLeftBottomU ("DetectRectLeftBottomU",Float) = 0.3
		_DectectRectLeftBottomV ("DetectRectLeftBottomV",Float) = 0.3
		_DistortFactor ("DistortFactor",Range(0.1,0.25)) = 0.15
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Lighting Off
		Blend One OneMinusSrcAlpha

		Tags { "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"}

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
			sampler2D _CircleTex;
			float _CircleCenterX;
			float _CircleCenterY;
			float _CircleRadiusRatioWithWidth;
			float _CircleRadiusRatioWithHeight;
			float _DistortFactor;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 lbBorder = float2(_CircleCenterX-_CircleRadiusRatioWithWidth,_CircleCenterY-_CircleRadiusRatioWithHeight);
				float2 rtBorder = float2(_CircleCenterX+_CircleRadiusRatioWithWidth,_CircleCenterY+_CircleRadiusRatioWithHeight);
				fixed4 col;
				if ( i.uv.x >= lbBorder.x && i.uv.x <= rtBorder.x && i.uv.y >= lbBorder.y && i.uv.y <= rtBorder.y )
				{
					float2 localUV = float2((i.uv.x-lbBorder.x)/2/_CircleRadiusRatioWithWidth,(i.uv.y-lbBorder.y)/2/_CircleRadiusRatioWithHeight);
					fixed4 distortCol = tex2D(_CircleTex,localUV);
					if ( distortCol.a > 0 )
					{
						//float2 dirVec = localUV - float2(0.5,0.5);
						//float maxLen = max(_CircleRadiusRatioWithWidth,_CircleRadiusRatioWithHeight);
						//float2 offset = normalize(dirVec) * (maxLen - length(dirVec)) * _DistortFactor;
						float2 dirVec = i.uv - float2(_CircleCenterX,_CircleCenterY);
						float maxLen = max(_CircleRadiusRatioWithWidth,_CircleRadiusRatioWithHeight);
						float2 offset = normalize(dirVec) * (maxLen - length(dirVec)) * _DistortFactor;
						col = tex2D(_MainTex,i.uv+offset);
						return col;
					}
				}
				col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
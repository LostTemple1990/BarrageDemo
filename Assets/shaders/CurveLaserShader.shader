Shader "YKShaders/CurveLaserShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Index ("LaserTextureIndex",Float) = 0
	}
	SubShader
	{
		// No culling or depth
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
			float _Index;

			fixed4 frag (v2f i) : SV_Target
			{
				//float vUnit = 1.0 / 16;
				//float startV = vUnit * _Index;
				//float2 uv = float2(i.uv.x,i.uv.y * vUnit + startV);
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}

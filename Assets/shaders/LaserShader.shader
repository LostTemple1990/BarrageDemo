Shader "YKShaders/LaserShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
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

			fixed4 frag (v2f i) : SV_Target
			{
				float scaleX = sin(i.uv.y*3.1415926);
				float halfLen = scaleX / 2;
				fixed4 col;
				if ( abs(i.uv.x-0.5) > halfLen )
				{
					col = 0,0,0,0;
				}
				else
				{
					i.uv.x = (i.uv.x - 0.5 + halfLen) / scaleX;
					col = tex2D(_MainTex, i.uv);
				}
				return col;
			}
			ENDCG
		}
	}
}

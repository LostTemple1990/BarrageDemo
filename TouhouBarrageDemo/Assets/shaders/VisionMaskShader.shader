Shader "YKShaders/VisionMaskShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PlayerX ("PlayerX",Float) = 0
		_PlayerY ("PlayerY",Float) = 0
		_MaskRadius ("CircleRadius",Float) = 50
		_STGWidth ("STGWidth",Float) = 384
		_STGHeight ("STGHeight",Float) = 448
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
			"CanUseSpriteAtlas" = "True"}

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
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}
			
			sampler2D _MainTex;
			float _PlayerX;
			float _PlayerY;
			float _MaskRadius;
			float _STGWidth;
			float _STGHeight;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 pos = float2(_STGWidth*i.uv.x-_STGWidth*0.5,_STGHeight*i.uv.y-_STGHeight*0.5);
				float2 center = float2(_PlayerX,_PlayerY);
				float2 dirVec = pos - center;
				float len = length(dirVec);
				clip(len-_MaskRadius);
				fixed4 col = tex2D(_MainTex,i.uv);
				col *= i.color;
				col.rgb *= col.a;
				return col;
			}
			ENDCG
		}
	}
}
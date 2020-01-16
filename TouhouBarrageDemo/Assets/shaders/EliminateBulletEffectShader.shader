Shader "YKShaders/EliminateBulletEffectShader"
{
	Properties
	{
		[PerRendererData]_MainTex("Texture", 2D) = "white" {}
		_EliminateTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Back ZWrite Off ZTest Always
		Lighting Off
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		//Blend SrcAlpha One

		Tags{ "Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane" 
		"CanUseSpriteAtlas" = "True"}

		Pass
		{
			CGPROGRAM
			#pragma enable_d3d11_debug_symbols
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

			sampler2D _MainTex;
			sampler2D _EliminateTex;
			float4 _MainTex_ST;
			int _Index;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv,_MainTex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				uint index = floor(i.color.a / 0.125);
				float row = floor(index / 4);
				float col = index - row * 4;
				row = 1 - row;
				float u = col * 0.25;
				float v = row * 0.5;
				float2 uv = float2(u + i.uv.x * 0.25,v + i.uv.y * 0.5);
				//float2 uv = float2(col+i.uv.x,row+i.uv.y);
				fixed4 color = tex2D(_EliminateTex, uv);
				color *= fixed4(i.color.rgb,1);
				//color.rgb *= color.a;
				return color;
			}
			ENDCG
		}
	}
}

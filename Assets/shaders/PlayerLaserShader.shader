Shader "YKShaders/PlayerLaserShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_LaserTotalLen("TotalLen",float) = 256
		_CurLaserLen("CurLen",float) = 256
		_RepeatCount("RepeatCount",float) = 2.5
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Lighting Off
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Tags{ "Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True" }

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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}

			sampler2D _MainTex;
			float _LaserTotalLen;
			float _CurLaserLen;
			float _RepeatCount;

			fixed4 frag(v2f i) : SV_Target
			{
				// 截取掉超过了长度的部分
				clip(_CurLaserLen / (_LaserTotalLen*_RepeatCount) - i.uv.x);
				// 计算分块之后每一块的长度
				float blockLen = 1 / _RepeatCount;
				// 当前uv对应应该取到原图uv的位置
				float2 texUV = float2(fmod(i.uv.x, blockLen) / blockLen, i.uv.y);
				// 计算滚动的比例，默认速度是2秒完成一次全图uv滚动
				float rate = fmod(_Time.y,2) / 2;
				float u = fmod(rate + texUV.x,1);
				float2 uv = float2(u,i.uv.y);
				fixed4 col = tex2D(_MainTex, uv);
				col *= i.color;
				col.rgb *= col.a;
				return col;
			}
			ENDCG
		}
	}
}
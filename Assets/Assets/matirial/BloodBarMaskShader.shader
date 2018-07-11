Shader "YKShaders/BloodBarMaskShader"
{
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" {}
		_MaskTex ("MaskTexture", 2D) = "white" {}
		_Rate ("Rate", float) = 1
	}
	SubShader
	{
		Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha

		Tags { "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
			}

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
				//fixed4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _MaskTex;
			float _Rate;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed maskAlpha = tex2D(_MaskTex,i.uv).a;
				col.a *= maskAlpha >= _Rate ? 0 : 1;
				return col;
			}
			ENDCG
		}
	}

	//SubShader
	//{
		//AlphaTest LEqual [_Rate]
		//Pass
		//{
			//SetTexture(_MaskTex){combine texture}
			//SetTexture(_MainTex){combine texture,previous}	
		//}
	//}
}

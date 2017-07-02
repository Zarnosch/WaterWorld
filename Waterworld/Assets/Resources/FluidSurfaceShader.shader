Shader "Unlit/FluidSurfaceShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FluidHeight ("Fluid Height", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 normal :NORMAL;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			Texture2D _FluidHeight;
			
			v2f vert (appdata v)
			{
				v2f o;
				int2 texPos = int2(v.vertex.x, v.vertex.z);
				float4 vert = float4(v.vertex.x, _FluidHeight[texPos].x * 10, v.vertex.z, v.vertex.w);
				o.color = float4(0, _FluidHeight[texPos].x, 0, 1);
				o.vertex = UnityObjectToClipPos(vert);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgba = fixed4(i.color.g, i.color.g, i.color.g, 1);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

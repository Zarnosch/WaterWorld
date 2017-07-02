Shader "Unlit/FluidSimShader"
{
	Properties
	{
		_FluidHeight ("Fluid Height", 2D) = "white" {}
		_Flux ("Flux", 2D) = "white" {}
		_DT("Delta Time", Float) = 0.002
		_Resolution("Simulation Mesh Resolution", int) = 64
		_A("Pipe Diameter", Float) = 1
		_L("Pipe Length", Float) = 1
		_G("Gravity", Float) = 9.81
		_D("Damping", Float) = 0.001
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Name "VolumeAssignment"
			CGPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			Texture2D _FluidHeight;
			Texture2D _Flux;
			float _DT;
			int _Resolution;
			float _A;
			float _L;
			float _G;
			float _D;

			struct frag_input
			{
         		fixed4 pos : POSITION;
         		float4 height : Height;
			};

			frag_input vert (uint id : SV_VertexID)
			{
				frag_input o;
				int2 idm = int2(id % (_Resolution), id / (_Resolution));
				int2 idl = int2(idm.x - 1, idm.y);
				int2 idr = int2(idm.x + 1, idm.y);
				int2 idt = int2(idm.x, idm.y + 1);
				int2 idb = int2(idm.x, idm.y - 1);

				fixed4 p = fixed4(fixed(idm.x) / fixed(_Resolution), fixed(idm.y) / fixed(_Resolution), 0, 1);
				fixed4 po = fixed4(1.0 - p.x * 2.0, 1.0 - p.y * 2.0, 0, 1);

				float height = 0;
				if(idm.x == 0)
				{
					o.pos.x = -1.0;
					height = _FluidHeight[idr].x;
					//height = 1;
				}
				if(idm.y == 0)
				{
					o.pos.y = -1.0;
					height = _FluidHeight[idt].x;
					//height = 1;
				}
				if(idm.x == _Resolution - 1)
				{
					o.pos.x = 1.0;
					height = _FluidHeight[idl].x;
					//height = 1;
				}
				if(idm.y == _Resolution - 1)
				{
					o.pos.y = 1.0;
					height = _FluidHeight[idb].x;
					//height = 1;
				}
				if(idm.x > 0 && idm.y > 0 && idm.x < _Resolution -1 && idm.y < _Resolution - 1)
				{
					height = _FluidHeight[idm].x;
					//height = 0;
				}

				height = 1;
				o.pos = po;
				o.height = float4(height, 0, 0, 0);
				return o;
			}			
			
			fixed4 frag (frag_input i) : COLOR
			{
				return i.height;
			}
			ENDCG
		}

		Pass
		{
			Name "FluxPreCalc"
			CGPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			Texture2D _FluidHeight;
			Texture2D _Flux;
			float _DT;
			int _Resolution;
			float _A;
			float _L;
			float _G;
			float _D;

			struct frag_input
			{
         		float4 pos : POSITION;
         		float4 flux : FLUX;
			};

			frag_input vert (uint id : SV_VertexID)
			{
				frag_input o;
				int2 idm = int2(id % _Resolution, id / _Resolution);
				int2 idl = int2(idm.x - 1, idm.y);
				int2 idr = int2(idm.x + 1, idm.y);
				int2 idt = int2(idm.x, idm.y + 1);
				int2 idb = int2(idm.x, idm.y - 1);

				float4 outFlux = float4(0, 0, 0, 0);
				if(idm.x > 0 && idm.y > 0 && idm.x < _Resolution -1 && idm.y < _Resolution - 1)
				{
					// flux calculation
					float x = max(0, _Flux[idm].x + _DT * _A * (_G * (_FluidHeight[idm].x - _FluidHeight[idl].x)) / _L); // left
					float y = max(0, _Flux[idm].y + _DT * _A * (_G * (_FluidHeight[idm].x - _FluidHeight[idr].x)) / _L); // right
					float z = max(0, _Flux[idm].z + _DT * _A * (_G * (_FluidHeight[idm].x - _FluidHeight[idt].x)) / _L); // top
					float w = max(0, _Flux[idm].w + _DT * _A * (_G * (_FluidHeight[idm].x - _FluidHeight[idb].x)) / _L); // bot

					outFlux = float4(x, y, z, w);


					float K = 0;
			        if(any(outFlux)) // true if there is at least one "not- zero" value
			        {
			        	K = min(1, _FluidHeight[idm].x / (length(outFlux) * _DT));
			        }

					// scale flux and damp it
					K = K * (1.0 - _D);
			        outFlux = outFlux * K;
				}
				o.pos = float4(-1.0 + float(idm.x * 2) / (_Resolution), -1.0 + float(idm.y * 2) / (_Resolution), 0, 1);
				o.flux = outFlux;
				return o;
			}			
			
			float4 frag (frag_input i) : COLOR
			{
				return i.flux;
			}
			ENDCG
		}

		Pass
		{
			Name "VolumeAssignment"
			CGPROGRAM
			#pragma target 3.5
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			Texture2D _FluidHeight;
			Texture2D _Flux;
			float _DT;
			int _Resolution;
			float _A;
			float _L;
			float _G;
			float _D;

			struct frag_input
			{
         		float4 pos : POSITION;
         		float4 height : Height;
			};

			frag_input vert (uint id : SV_VertexID)
			{
				frag_input o;
				int2 idm = int2(id % _Resolution, id / _Resolution);
				int2 idl = int2(idm.x - 1, idm.y);
				int2 idr = int2(idm.x + 1, idm.y);
				int2 idt = int2(idm.x, idm.y + 1);
				int2 idb = int2(idm.x, idm.y - 1);

				float dV = 0;
				if(idm.x > 0 && idm.y > 0 && idm.x < _Resolution -1 && idm.y < _Resolution - 1)
				{
					dV = ((_Flux[idl].y + _Flux[idr].x + _Flux[idt].w + _Flux[idb].z) - (_Flux[idm].x + _Flux[idm].y + _Flux[idm].z + _Flux[idm].w)) * _DT;
				}
				o.pos = float4(-1.0 + float(idm.x * 2) / (_Resolution), -1.0 + float(idm.y * 2) / (_Resolution), 0, 1);
				o.height = float4(_FluidHeight[idm].x + dV, 0, 0, 0);
				return o;
			}			
			
			float4 frag (frag_input i) : COLOR
			{
				return i.height;
			}
			ENDCG
		}
	}
}

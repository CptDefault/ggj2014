Shader "Custom/DrawInFront" {
	Properties {
		_MainColor ("Main Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
			Tags { "Queue"="Overlay" }
			LOD 200

		Pass
		{

			ZTest Always 
			ZWrite On
			ColorMask 0

		}
			ZTest LEqual
			ZWrite On

			CGPROGRAM
			#pragma surface surf Lambert

			float4 _MainColor;

			struct Input {
				float2 uv_MainTex;
			};

			void surf (Input IN, inout SurfaceOutput o) {
				o.Albedo = _MainColor.rgb;
				o.Alpha = _MainColor.a;
			}
			ENDCG
		
	}


	FallBack "Diffuse"
}

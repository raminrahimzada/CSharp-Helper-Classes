Shader "MMSOFT Shaders/FirstShader" {
	Properties {
		_EmissiveColor ("First Emissive Color",Color)=(1,1,1,1)
		_AmbientColor ("Second Ambient Color",Color)=(1,1,1,1)
		_Value("Power Value",Range(1,5))=1
		}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		float4 _EmissiveColor;
		float4 _AmbientColor;
		float _Value;
		
		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float4 c=pow((_EmissiveColor+_AmbientColor),_Value);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

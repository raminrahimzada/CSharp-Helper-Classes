Shader "Custom/first" {
	Properties {
		_EmissiveColor("Birinci Reng",Color)=(1,1,1,1)
		_AmbientColor("Ikinci Reng",Color)=(1,1,1,0)
		_Quvvet("Quvvet",Range(1,10))=4
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float4 _EmissiveColor; 
		float4 _AmbientColor;
		float _Quvvet;
		
		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float4 c = pow((_EmissiveColor+_AmbientColor)/2.0+0.5,_Quvvet);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

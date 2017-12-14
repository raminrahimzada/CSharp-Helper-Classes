Shader "Selection Projector" { 
	Properties {
		_MainTex ("Base", 2D) = "fresnel.png" {
			TexGen ObjectLinear 	
		}
		_FalloffTex ("FallOff", 2D) = "fresnel.png" {
			TexGen ObjectLinear	
		}
	}

	Subshader {
		Pass {
			ZWrite off
			Offset -1, -1
			Fog { Color (1, 1, 1) }

			//Blend DstColor Zero  
			Blend SrcAlpha OneMinusSrcAlpha 
			//Blend DstColor OneMinusSrcAlpha 

			SetTexture [_MainTex] {				
				Matrix [_Projector]
			}

			SetTexture [_FalloffTex] {
				combine previous lerp (texture) constant
				Matrix [_ProjectorClip]
			}
		}
	}
}
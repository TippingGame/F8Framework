Shader "Custom/BackBlur"
{
	Properties
	{
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
        _Size ("Size", Range(0, 20)) = 1
	}
	Category {
	    
        Tags { 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}  
  
        SubShader {
            GrabPass {                      
                Tags { "LightMode" = "Always" }  
            }  
            Pass {
                Tags { "LightMode" = "Always" }  
 
                Name "BackBlurHor"
                CGPROGRAM  
                #pragma vertex vert  
                #pragma fragment frag  
                #pragma fragmentoption ARB_precision_hint_fastest  
                #include "UnityCG.cginc"  
  
                struct appdata_t {  
                    float4 vertex : POSITION;  
                    float2 texcoord : TEXCOORD0;
					float4 color    : COLOR;
                };  
  
                struct v2f {  
                    float4 vertex : POSITION;  
                    float4 uvgrab : TEXCOORD0;
					float4 color    : COLOR;
                };
  
                v2f vert (appdata_t v) {  
                    v2f o; 
                    o.vertex = UnityObjectToClipPos(v.vertex);  
                    #if UNITY_UV_STARTS_AT_TOP  
                    float scale = -1.0;  
                    #else  
                    float scale = 1.0;  
                    #endif  
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;  
                    o.uvgrab.zw = o.vertex.zw;  
 
					o.color = v.color;
                    return o;  
                }  
  
                sampler2D _GrabTexture;  
                float4 _GrabTexture_TexelSize;
			    float4 _MainTex_TexelSize;
                float _Size;
                uniform float4 _Color;
 
                half4 GrabPixel(v2f i, float weight, float kernelx){
                    if (i.uvgrab.x == 0 && i.uvgrab.y == 0){
                        kernelx = 0;
                    }
                    return tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x*kernelx*_Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight;
                }
                half4 frag( v2f i ) : COLOR {  
                 	half4 sum = half4(0,0,0,0);
                    sum += GrabPixel(i, 0.05, -4.0);
                    sum += GrabPixel(i, 0.09, -3.0);
                    sum += GrabPixel(i, 0.12, -2.0);
                    sum += GrabPixel(i, 0.15, -1.0);
                    sum += GrabPixel(i, 0.18,  0.0);
                    sum += GrabPixel(i, 0.15, +1.0);
                    sum += GrabPixel(i, 0.12, +2.0);
                    sum += GrabPixel(i, 0.09, +3.0);
                    sum += GrabPixel(i, 0.05, +4.0);
 
                    float4 col5 = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
					float decayFactor = 1.0f;
					if (i.uvgrab.x == 0 && i.uvgrab.y == 0){
						decayFactor = 0;
					}
					sum = lerp(col5, sum, decayFactor) * i.color * _Color;
  
                    return sum;  
                }  
                ENDCG  
            }  
            
            GrabPass {                          
                Tags { "LightMode" = "Always" }  
            }  
            Pass {  
                Tags { "LightMode" = "Always" }
 
                Name "BackBlurVer"
                CGPROGRAM  
                #pragma vertex vert  
                #pragma fragment frag  
                #pragma fragmentoption ARB_precision_hint_fastest  
                #include "UnityCG.cginc"  
  
                struct appdata_t {  
                    float4 vertex : POSITION;  
                    float2 texcoord: TEXCOORD0;  
					float4 color    : COLOR;
                };  
  
                struct v2f {  
                    float4 vertex : POSITION;  
                    float4 uvgrab : TEXCOORD0;  
					float4 color    : COLOR;
                };  
  
                v2f vert (appdata_t v) {  
                    v2f o;  
                    o.vertex = UnityObjectToClipPos(v.vertex);  
                    #if UNITY_UV_STARTS_AT_TOP  
                    float scale = -1.0;  
                    #else  
                    float scale = 1.0;  
                    #endif  
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;  
                    o.uvgrab.zw = o.vertex.zw;  
 
					o.color = v.color;
                    return o;  
                }  
  
                sampler2D _GrabTexture;  
                float4 _GrabTexture_TexelSize;  
                float _Size;
                uniform float4 _Color;
 
                half4 GrabPixel(v2f i, float weight, float kernely){
                    if (i.uvgrab.x == 0 && i.uvgrab.y == 0){
                        kernely = 0;
                    }
                    return tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y*kernely*_Size, i.uvgrab.z, i.uvgrab.w))) * weight;
                }
  
                half4 frag( v2f i ) : COLOR {
                    half4 sum = half4(0,0,0,0);
                    sum += GrabPixel(i, 0.05, -4.0);
                    sum += GrabPixel(i, 0.09, -3.0);
                    sum += GrabPixel(i, 0.12, -2.0);
                    sum += GrabPixel(i, 0.15, -1.0);
                    sum += GrabPixel(i, 0.18,  0.0);
                    sum += GrabPixel(i, 0.15, +1.0);
                    sum += GrabPixel(i, 0.12, +2.0);
                    sum += GrabPixel(i, 0.09, +3.0);
                    sum += GrabPixel(i, 0.05, +4.0);
 
					float4 col5 = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
					float decayFactor = 1.0f;
					if (i.uvgrab.x == 0 && i.uvgrab.y == 0){
						decayFactor = 0;
					}
					sum = lerp(col5, sum, decayFactor) * i.color * _Color;
  
                    return sum;  
                }  
                ENDCG  
            }
        }  
    } 
}
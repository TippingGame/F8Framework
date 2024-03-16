Shader "UI/UIBackgroundBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _CenterSize ("center size", float) = 0.2
        _CenterStartPos ("center Start Pos", float) = 0.4
        
        _BlurSize ("blur size", float) = 5.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv: TEXCOORD0;
                float4 vertex: SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CenterSize, _CenterStartPos;
            float _BlurSize = 10;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }
            
            fixed4 frag(v2f i): SV_Target
            {
                float2 uv = i.uv;
                uv.y = uv.y * 0.5 + 0.25;
                fixed4 blurCol = tex2D(_MainTex, uv);
                fixed4 blurCol2 = tex2D(_MainTex, uv + float2(0, 0.001 * _BlurSize));
                fixed4 blurCol3 = tex2D(_MainTex, uv - float2(0, 0.001 * _BlurSize));
                fixed4 blurCol4 = tex2D(_MainTex, uv + float2(0.001 * _BlurSize, 0));
                fixed4 blurCol5 = tex2D(_MainTex, uv - float2(0.001 * _BlurSize, 0));
                
                blurCol += blurCol2;
                blurCol += blurCol3;
                blurCol += blurCol4;
                blurCol += blurCol5;
                blurCol /= 5.0;
                
                fixed4 clearCol = tex2D(_MainTex, i.uv);
                
                float result = step(_CenterStartPos + _CenterSize, i.uv.y);
                float result2 = step(i.uv.y, _CenterStartPos);
                
                float4 finalCol = blurCol * result2 + blurCol * result + clearCol * clamp(1.0 - result - result2, 0, 1);
                
                return finalCol;
            }
            ENDCG
            
        }
    }
}
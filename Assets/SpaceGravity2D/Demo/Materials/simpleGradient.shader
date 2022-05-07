// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "SpaceGravity2D/SimpleGradient" {
 Properties {
     _Color ("Left Color", Color) = (1,1,1,1)
     _Scale ("Left Scale", Range(0, 1.0) ) = 0
     _Color2 ("Right Color", Color) = (1,1,1,1)
     _Scale2 ("Right Scale", Range(0, 1.0) ) = 0
	 }
  
 SubShader {
     Tags {"Queue"="Background"  "IgnoreProjector"="True"}
     LOD 100
  
     ZWrite On
  
     Pass {
         CGPROGRAM
         #pragma vertex vert  
         #pragma fragment frag
         #include "UnityCG.cginc"
  
         fixed4 _Color;
         fixed4 _Color2;
         float  _Scale;
         float  _Scale2;
  
         struct v2f {
             float4 pos : SV_POSITION;
             fixed4 col : COLOR;
         };
  
         v2f vert (appdata_full v)
         {
             v2f o;
             o.pos = UnityObjectToClipPos(v.vertex);

			 //v.texcoord.y is in range from -1 to 1

			 if (v.texcoord.y + 1 <= _Scale){	
				o.col = _Color;
				return o;
			 }
			 if (v.texcoord.y >= 1 - _Scale2){
				o.col = _Color2;
				return o;
			 }
             o.col = lerp(_Color,_Color2, (v.texcoord.y + 1) / (2 - _Scale - _Scale2) );
             return o;
         }
        
  
         float4 frag (v2f i) : COLOR {
             float4 c = i.col;
             c.a = 1;
             return c;
         }
             ENDCG
         }
     }
 }
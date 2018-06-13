Shader "Unlit/Pano360Shader"{
   Properties{
       _MainTex ("Base (RGB)", 2D) = "white" {}
       _Color ("Main Color", Color) = (1,1,1,0.5)
   }
   SubShader {
      Tags { "RenderType" = "Opaque" }
      Cull Front
      CGPROGRAM
      #pragma surface surf Lambert
      sampler2D _MainTex;
      struct Input{
         float2 uv_MainTex;
         float4 myColor : COLOR;
      };
 
      fixed4 _Color;
      void surf (Input IN, inout SurfaceOutput o){
         fixed4 t = tex2D(_MainTex, IN.uv_MainTex) * _Color;
         o.Albedo = t.rgb;
         o.Alpha = t.a;
      }
      ENDCG
   }
   Fallback "Diffuse"
}
Shader "TSDv2p5/TexCutoff" {
	Properties {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}

		_Glossiness("Smoothness", Range(0.0,1.0)) = 0.8
		[Gamma] _Metallic("Metallic", Range(0.0,1.0)) = 1.0
		_MetallicGlossMap("Metallic n Smoothness", 2D) = "grey" {}

		[MaterialToggle(_MATCAP_ON)] _MatcapS("Matcap map switch", Float) = 1
		[NoScaleOffset]
		_Matcap("Matcap Map ", 2D) = "white" {}
		_MatcapIn("Matcap intensity", Float) = 1.0

		_BumpMap("Bumpmap", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1.0

		[MaterialToggle(_RIM_ON)] _RimS("Rim switch", Float) = 0
		_RimColor("Rim Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimPow("Rim Power", Float) = 1.0
		_RimIn("Rim Intensity", Float) = 1.0

		_OcclusionMap("Occlusion", 2D) = "white" {}
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0

		_EmissionMap("Emission", 2D) = "black" {}
		_EmissIn("Emission intensity", Float) = 1.0

		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
	}
	
	SubShader{
			Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" "PerformanceChecks" = "False" }

			LOD 300
			Cull Off
			CGPROGRAM

	#include "UnityCG.cginc"
	#pragma surface surf Standard  fullforwardshadows  alphatest:_Cutoff
	#pragma shader_feature _MATCAP_ON
	#pragma shader_feature _RIM_ON
	#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _BumpScale;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _MetallicGlossMap;
		sampler2D _OcclusionMap;
		half _OcclusionStrength;
		sampler2D _EmissionMap;
		half _EmissIn;
	
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			half3 viewDir;
			INTERNAL_DATA
			float3 worldNormal;
		};

#if _MATCAP_ON
		half _MatcapIn;
		sampler2D _Matcap;
#endif

#if _RIM_ON
		fixed4 _RimColor;
		fixed _RimPow;
		fixed _RimIn;
#endif

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 m = tex2D(_MetallicGlossMap, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			o.Metallic = m.r * _Metallic;
			o.Smoothness = m.a * _Glossiness;

			o.Emission = tex2D(_EmissionMap, IN.uv_MainTex) * _EmissIn;
			half3 tNormal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpScale);
			normalize(tNormal);
			o.Normal = tNormal;

#if (SHADER_TARGET < 30)
			o.Occlusion = tex2D(_OcclusionMap, IN.uv_MainTex).g;
#else
			half occ = tex2D(_OcclusionMap, IN.uv_MainTex).g;
			o.Occlusion = LerpOneTo(occ, _OcclusionStrength);
#endif

#if _MATCAP_ON
			float3 worldNormal = WorldNormalVector(IN, o.Normal);
			worldNormal = mul((float3x3)UNITY_MATRIX_V, worldNormal);
			worldNormal = worldNormal *0.5 + 0.5;
			float4 matcap = tex2D(_Matcap, worldNormal.xy);
			o.Albedo *= matcap * _MatcapIn;
#endif

#if _RIM_ON
			fixed rim = 1.0 - saturate(dot(IN.viewDir, o.Normal));
			o.Albedo += _RimColor.rgb * pow(rim, _RimPow) * _RimIn;
#endif

		}
		ENDCG
	}
		Fallback "Transparent/Cutout/Diffuse"
}
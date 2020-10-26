// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "TSDv2p5/PlainOutline" {
    Properties 
    {
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}

		_Glossiness("Smoothness", Range(0.0,1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0,1.0)) = 0.

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

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionMap("Emission", 2D) = "black" {}
		_EmissIn("Emission intensity", Float) = 1.0
		[MaterialToggle(_ASYM_ON)] _Asym ("Enable Asymmetry", Float) = 0
		_Asymmetry ("OutlineAsymmetry", Vector) = (0.0,0.25,0.5,0.0)
		_OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1.0)
		_Outline ("Outline width", Float) = 0.01
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		LOD 2000 
        Lighting Off
        Fog { Mode Off }
        
        UsePass "TSDv2p5/Plain/FORWARD"

		Pass
        {
			Cull Front
			ZWrite On
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma multi_compile _ASYM_OFF _ASYM_ON
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
			#pragma vertex vert
			#pragma fragment frag

		struct appdata_t 
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f 
		{
			float4 pos : SV_POSITION;
		};

		fixed _Outline;
		#if _ASYM_ON
			float4 _Asymmetry;
		#endif
            
		v2f vert (appdata_t v) 
		{
			v2f o;
			o.pos = v.vertex;

		#if _ASYM_ON
			o.pos.xyz += (v.normal.xyz + _Asymmetry.xyz) *_Outline*0.01;
		#else
			o.pos.xyz += v.normal.xyz *_Outline*0.01;
		#endif

			o.pos = UnityObjectToClipPos(o.pos);
			return o;
	}

	fixed4 _OutlineColor;
	
	fixed4 frag(v2f i) :COLOR 
	{
		return _OutlineColor;
	}
	
	ENDCG
	}
	}
	
	Fallback "Standard"
}

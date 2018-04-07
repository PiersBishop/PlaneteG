Shader "Custom/DistanceGradient" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Color1 ("Color1", Color) = (1,1,1,1)
		_Color2 ("Color2", Color) = (1,1,1,1)

		_GradientDist ("Gradient dist", Range(0,0.1)) = 0.05

		_GradPos1 ("Gradient pos1", Range(0.5,5)) = 1.1
		_GradPos2 ("Gradient pos2", Range(1,5)) = 1.3
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		struct Input {
			float3 worldPos;
		};

		float _GradientDist;
		float _GradPos1;
		float _GradPos2;
		fixed4 _Color;
		fixed4 _Color1;
		fixed4 _Color2;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		//UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		//UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float3 pos = IN.worldPos;
			float3 z =(0.0,0.0,0.0);
			float distFromCenter = distance(z , pos);


			fixed4 c0 = _Color;
			fixed4 c1 = _Color1;
			fixed4 c2 = _Color2;


			float aa =  (_GradPos1 - _GradientDist);
			float bb =  (_GradPos1 + _GradientDist);
			float cc =  (_GradPos2 - _GradientDist);
			float dd =  (_GradPos2 + _GradientDist);


			float lerp1 = clamp( (distFromCenter - aa) / (bb - aa), 0, 1);
			float lerp2 = clamp( (distFromCenter - cc) / (dd - cc), 0, 1);

			fixed4 c = lerp (_Color, _Color1, lerp1);
			c = lerp (c, _Color2, lerp2);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

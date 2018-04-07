Shader "Custom/WaterGradient" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Color1 ("Color1", Color) = (1,1,1,1)
		_Color2 ("Color2", Color) = (1,1,1,1)

		_GradientDist ("Gradient dist", Range(0,0.1)) = 0.05

		_GradPos ("Gradient pos", Range(0,1)) = 0.5
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
		float _GradPos;
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
			float distFromCenter = pos.y;

			float aa =  (_GradPos - _GradientDist);
			float bb =  (_GradPos + _GradientDist);
			float aaa = -aa;
			float bbb = -bb;

			float lerp1 = clamp( (distFromCenter - aa) / (bb - aa), 0, 1);
			float lerp2 = clamp( (distFromCenter - aaa) / (bbb - aaa), 0, 1);
			float lerp3 = clamp( (distFromCenter - aa*1.75f) / (bb - aa), 0, 1);
			float lerp4 = clamp( (distFromCenter - aaa*1.75f) / (bbb - aaa), 0, 1);

			fixed4 c = lerp (_Color, _Color1, lerp1);
			c = lerp (c, _Color1, lerp2);
			c = lerp (c, _Color2, lerp3);
			c = lerp (c, _Color2, lerp4);
			o.Albedo = c.rgb;
			o.Metallic = 0.5;
			o.Smoothness = 0.5;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

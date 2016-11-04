
Shader "Custom/ClipShader_Trans" {
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		//_a("_a", Float) = 0.0
		//_b("_b", Float) = 0.0
		//_c("_c", Float) = 0.0
		//_d("_d", Float) = 0.0
		//_clip("_clip", Float) = 1
	}


	SubShader{

		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent"}

		Cull Off

		CGPROGRAM

#pragma surface surf Lambert alpha

		struct Input {
		float2 uv_MainTex;
		float3 worldPos;
	};

	sampler2D _MainTex;
	float _a,_b,_c,_d,_clip;

	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutput o)
	{
		

		if (_clip == 1)
		{
			clip(_a *IN.worldPos.x +
				_b *IN.worldPos.y +
				_c *IN.worldPos.z +
				_d > 0 ? -1 : 1);
		}

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;

		//o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
	}
	ENDCG
	}
	Fallback "Diffuse"
}

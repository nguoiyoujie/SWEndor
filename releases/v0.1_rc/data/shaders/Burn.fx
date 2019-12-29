// Global-Level Semantics
float4x4 matWorldViewProj : WORLDVIEWPROJECTION;
float4x4 matWorld : WORLD;
float3 viewPosition : VIEWPOSITION;
//float time : TIME;

float3 dirLightDir : LIGHTDIR0_DIRECTION;
float3 dirLightColor : LIGHTDIR0_COLOR;

texture texTexture : TEXTURE0;
sampler sampTexture = sampler_state {
	Texture = (texTexture);
};

texture texDissolve;
sampler dissolveTexture = sampler_state {
	Texture = (texDissolve);
};

texture texBurn;
sampler burnTexture = sampler_state {
	Texture = (texBurn);
};

float dissolve_burn_ratio = 0.6f;
float period = 5;
float time;

struct VS_INPUT {
	float4 position : POSITION;
	float2 texCoord : TEXCOORD;
};

struct VS_OUTPUT {
	float4 position : POSITION;
	float2 texCoord : TEXCOORD0;
};
#define	PS_INPUT VS_OUTPUT

// Vertex Shader Function
VS_OUTPUT VS(VS_INPUT IN) {
	VS_OUTPUT OUT;

	OUT.position = mul(IN.position, matWorldViewProj);
	OUT.texCoord = IN.texCoord;

	return OUT;
}

// Pixel Shader Function
float4 PS(PS_INPUT IN) : COLOR {
	float2 texCoord = IN.texCoord;
	float4 texColor = tex2D(sampTexture, texCoord);
	float dissolve = tex2D(dissolveTexture, texCoord).r;
	float limitD = 1 - time / period;
	clip(dissolve - limitD);

	float limitB = (1 - time / period) / dissolve_burn_ratio;
	float2 burnPos = float2(smoothstep(limitD, limitB, dissolve), 0);
	float burnStep = step(dissolve, limitB);
	float4 color = tex2D(burnTexture, burnPos) * burnStep + texColor * (1.0 - burnStep);
	return float4(color);
}

technique TSM3 {
	pass P {
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}
technique TSM2a {
	pass P0 {
		VertexShader = compile vs_2_a VS();
		PixelShader = compile ps_2_a PS();
	}
}
technique TSM2b {
	pass P0 {
		VertexShader = compile vs_2_0 VS();
		PixelShader = compile ps_2_b PS();
	}
}
technique TSM2 {
	pass P {
		VertexShader = compile vs_2_0 VS();
		PixelShader = compile ps_2_0 PS();
	}
}
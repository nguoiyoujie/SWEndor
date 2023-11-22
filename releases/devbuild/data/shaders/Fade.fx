// Global-Level Semantics
float4x4 matWorldViewProj : WORLDVIEWPROJECTION;
float4x4 matWorld : WORLD;
float3 viewPosition : VIEWPOSITION;

float period = 1.5;
float time;

texture texTexture : TEXTURE0;
sampler sampTexture = sampler_state {
	Texture = (texTexture);
};

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
	float opacity = time / period;
    texColor = texColor * opacity;
	return texColor;
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
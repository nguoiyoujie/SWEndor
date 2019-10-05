// Global-Level Semantics
float4x4 matWorldViewProj : WORLDVIEWPROJECTION;
float4x4 matWorld : WORLD;
float3 viewPosition : VIEWPOSITION;
float3 emissive : EMISSIVE;

float non_decay_dist = 3000;
float decay_dist = 500;

struct VS_INPUT {
	float4 position : POSITION;
	float2 texCoord : TEXCOORD;
};

struct VS_OUTPUT {
	float4 position : POSITION;
	float2 texCoord : TEXCOORD0;
	float3 worldPos : TEXCOORD3;
};
#define	PS_INPUT VS_OUTPUT

// Vertex Shader Function
VS_OUTPUT VS(VS_INPUT IN) {
	VS_OUTPUT OUT;

	OUT.position = mul(IN.position, matWorldViewProj);
	OUT.texCoord = IN.texCoord;
	OUT.worldPos = mul(IN.position, matWorld).xyz;
	return OUT;
}

// Pixel Shader Function
float4 PS(PS_INPUT IN) : COLOR {
	float2 texCoord = IN.texCoord;
	float dist = distance(IN.worldPos.xyz, viewPosition);
	clip(non_decay_dist + decay_dist - dist);

	float alpha = 1.0 - (dist - non_decay_dist) / decay_dist;
	float3 color = emissive;
	return float4(color, alpha);
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
// Global-Level Semantics
float4x4 matWorldViewProj : WORLDVIEWPROJECTION;
float4x4 matWorld : WORLD;
float3 viewPosition : VIEWPOSITION;

float3 dirLightDir : LIGHTDIR0_DIRECTION;
float3 dirLightColor : LIGHTDIR0_COLOR;

texture texTexture : TEXTURE0;
sampler sampTexture = sampler_state {
	Texture = (texTexture);
};

float din = 1;
float tin = 0.5;
float tout = 0.5;
float creep = 0.25;
float ctime;
float gtime;
float dmaxtime = 0.5;
float dtime;
float maxdist = 1600;
float spinperiod = 2.5;

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
	float df = saturate(dist / maxdist);
	float dy = saturate(dtime / dmaxtime);

	// spin
	texCoord.x = frac(texCoord.x + gtime / spinperiod);
	texCoord.y = frac(texCoord.y - gtime / creep);

	float cin = -din + df + (gtime - ctime) / tin;
	float cout = (1 - df) * dy / tout;
	float4 texColor = tex2D(sampTexture, texCoord);
	texColor.rgb += (saturate(1 - cout)).rrr;

	float t1 = min(1, cin);
	float t2 = min(t1, cout);
	texColor.a = t2;

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
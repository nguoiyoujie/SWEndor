// Reference Sample


// Global-Level Semantics
float4x4 matWorldViewProj : WORLDVIEWPROJECTION;
float4x3 matWorldIT : WORLDINVERSETRANSPOSE;
float4x4 matWorld : WORLD;
float3 viewPosition : VIEWPOSITION;


// Vertex Shader Input Structure
struct VS_INPUT {
	float4 position : POSITION;   // Vertex position in object space
	float3 normal : NORMAL;       // Vertex normal in object space
	float2 texCoord : TEXCOORD;   // Vertex texture coordinates
};

// Vertex Shader Output Structure
struct VS_OUTPUT {
	float4 position : POSITION;   // Pixel position in clip space	
	float2 texCoord : TEXCOORD0;  // Pixel texture coordinates
	float3 normal : TEXCOORD1;    // Pixel normal vector
	float3 view : TEXCOORD2;      // Pixel view vector
};
#define	PS_INPUT VS_OUTPUT            // What comes out of VS goes into PS!

// Vertex Shader Function
VS_OUTPUT VS(VS_INPUT IN) {
	VS_OUTPUT OUT;
	// Basic transformation of untransformed vertex into clip-space
	OUT.position = mul(IN.position, matWorldViewProj);

	// No scaling or translation is done, simply assign them and let the GPU interpolate
	OUT.texCoord = IN.texCoord;

	// Calculate the normal vector
	OUT.normal = mul(IN.normal, matWorldIT);
	// Calculate the view vector
	float3 worldPos = mul(IN.position, matWorld).xyz;
	OUT.view = viewPosition - worldPos;

	return OUT;
}

// Light direction global-level semantic
float3 dirLightDir : LIGHTDIR0_DIRECTION;
float3 materialEmissive : EMISSIVE;
float3 materialAmbient : AMBIENT;
float4 materialDiffuse : DIFFUSE;
float3 materialSpecular : SPECULAR;
float materialPower : SPECULARPOWER;
texture texTexture : TEXTURE0;
float3 dirLightColor : LIGHTDIR0_COLOR;

sampler sampTexture = sampler_state {
	Texture = (texTexture);
};

// Pixel Shader Function
float4 PS(PS_INPUT IN) : COLOR{
	// Normalize all vectors in pixel shader to get phong shading
	// Normalizing in vertex shader would provide gouraud shading
	float3 light = normalize(-dirLightDir);
	float3 view = normalize(IN.view);
	float3 normal = normalize(IN.normal);

	// Calculate the half vector
	float3 halfway = normalize(light + view);
	// Calculate the emissive lighting
	float3 emissive = materialEmissive;
	// Calculate the ambient reflection
	float3 ambient = materialAmbient;

	// Calculate the diffuse reflection
	float3 diffuse = saturate(dot(normal, light)) * materialDiffuse.rgb;

	// Calculate the specular reflection
	float3 specular = pow(saturate(dot(normal, halfway)), materialPower) * materialSpecular;

	// Fetch the texture coordinates
	float2 texCoord = IN.texCoord;

	// Sample the texture
	float4 texColor = tex2D(sampTexture, texCoord);

	// Combine all the color components
	float3 color = (saturate((saturate(ambient + diffuse) + specular) * dirLightColor + emissive)) * texColor;
	//float3 color = (saturate(ambient + diffuse) * texColor + specular) * dirLightColor + emissive;
	// Calculate the transparency
	float alpha = materialDiffuse.a * texColor.a;
	// Return the pixel's color
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
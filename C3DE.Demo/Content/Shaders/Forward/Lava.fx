// Matrix
float4x4 World;
float4x4 View;
float4x4 Projection;

// Material
float3 DiffuseColor;
float3 EmissiveColor;
float EmissiveIntensity;

// Misc
float2 TextureTiling;
float Time;

texture MainTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (MainTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture NormalTexture;
sampler2D bumpSampler = sampler_state
{
    Texture = (NormalTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture EmissiveTexture;
sampler2D emissiveSampler = sampler_state
{
    Texture = (EmissiveTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
#if SM4
	float4 Position : SV_Position;
#else
    float4 Position : POSITION0;
#endif
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.UV = float2(0.5, 0.5) * input.UV;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 noise = tex2D(bumpSampler, input.UV * TextureTiling);
    float2 T1 = input.UV + float2(1.5, -1.5) * Time * 0.02;
    float2 T2 = input.UV + float2(-0.5, 2.0) * Time * 0.01;
	
    T1.x += noise.x * 2.0;
    T1.y += noise.y * 2.0;
    T2.x -= noise.y * 0.2;
    T2.y += noise.z * 0.2;
	
    float p = tex2D(bumpSampler, (T1 * 3.0) * TextureTiling).a;
	
    float3 color = tex2D(textureSampler, (T2 * 4.0) * TextureTiling);
    float3 temp = color * (float3(p, p, p) * 2.0) + (color * color - 0.1);
	
    if (temp.r > 1.0)
        temp.bg += clamp(temp.r - 2.0, 0.0, 100.0);
	
    if (temp.g > 1.0)
        temp.rb += temp.g - 1.0;
	
    if (temp.b > 1.0)
        temp.rg += temp.b - 1.0;
		
    return float4(DiffuseColor * temp * EmissiveIntensity, 1.0);
}

float4 PixelShaderEmissive(VertexShaderOutput input) : COLOR0
{
    float3 emission = EmissiveColor * tex2D(emissiveSampler, input.UV * TextureTiling).xyz;
    return float4(emission * EmissiveIntensity, 1);
}

technique Basic
{
    pass AmbientPass
    {
#if SM4
		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
#else
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
#endif
    }
	
    pass EmissivePass
    {
#if SM4
		PixelShader = compile ps_4_0_level_9_3 PixelShaderEmissive();
#else
        PixelShader = compile ps_3_0 PixelShaderEmissive();
#endif
    }
}
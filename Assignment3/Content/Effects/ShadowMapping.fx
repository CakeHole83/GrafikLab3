﻿#if OPENGL
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 LightViewProj;

float3 LightDirection;
float4 AmbientColor = float4(0.15, 0.15, 0.15, 0);
float DepthBias = 0.001f;

texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = (Texture);
};

texture ShadowMap;
sampler ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;
};

struct DrawWithShadowMap_VSIn
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct DrawWithShadowMap_VSOut
{
    float4 Position : POSITION0;
    float3 Normal   : TEXCOORD0;
    float2 TexCoord : TEXCOORD1;
    float4 WorldPos : TEXCOORD2;
};

struct CreateShadowMap_VSOut
{
    float4 Position : POSITION;
    float Depth     : TEXCOORD0;
};

// Transforms the model into light space an renders out the depth of the object
CreateShadowMap_VSOut CreateShadowMap_VertexShader(float4 Position: POSITION)
{
    CreateShadowMap_VSOut Out;
    Out.Position = mul(Position, mul(World, LightViewProj)); 
    Out.Depth = Out.Position.z / Out.Position.w;    
    return Out;
}

// Saves the depth value out to the 32bit floating point texture
float4 CreateShadowMap_PixelShader(CreateShadowMap_VSOut input) : COLOR
{ 
    return float4(input.Depth, 0, 0, 0);
}

// Draws the model with shadows
DrawWithShadowMap_VSOut DrawWithShadowMap_VertexShader(DrawWithShadowMap_VSIn input)
{
    DrawWithShadowMap_VSOut Output;

    float4x4 WorldViewProj = mul(mul(World, View), Projection);
    
    // Transform the models verticies and normal
    Output.Position = mul(input.Position, WorldViewProj);
    Output.Normal =  normalize(mul(input.Normal, World));
    Output.TexCoord = input.TexCoord;
    
    // Save the vertices postion in world space
    Output.WorldPos = mul(input.Position, World);
    
    return Output;
}

// Determines the depth of the pixel for the model and checks to see 
// if it is in shadow or not
float4 DrawWithShadowMap_PixelShader(DrawWithShadowMap_VSOut input) : COLOR
{ 
    float4 diffuseColor = tex2D(TextureSampler, input.TexCoord); 
    float diffuseIntensity = saturate(dot(LightDirection, input.Normal));
    float4 diffuse = diffuseIntensity * diffuseColor + AmbientColor;
    float4 lightingPosition = mul(input.WorldPos, LightViewProj);
    
    float2 ShadowTexCoord = 0.5 * lightingPosition.xy / 
                            lightingPosition.w + float2( 0.5, 0.5 );
    ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

	float shadowdepth = tex2D(ShadowMapSampler, ShadowTexCoord).r;    
    
    float ourdepth = (lightingPosition.z / lightingPosition.w) - DepthBias;

	if (shadowdepth < ourdepth)
    {
        // Shadow the pixel by lowering the intensity
    diffuse *= float4(0.5,0.5,0.5,0);
    };

    return diffuse;
}

// Technique for creating the shadow map
technique CreateShadowMap
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL CreateShadowMap_VertexShader();
        PixelShader = compile PS_SHADERMODEL CreateShadowMap_PixelShader();
    }
}

// Technique for drawing with the shadow map
technique DrawWithShadowMap
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL DrawWithShadowMap_VertexShader();
        PixelShader = compile PS_SHADERMODEL DrawWithShadowMap_PixelShader();
    }
}

#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float4 TintColor = float4(1, 1, 1, 1);
float3 CameraPosition;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

Texture SkyboxTexture;
samplerCUBE SkyboxSampler = sampler_state
{
	texture = <SkyboxTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 Reflection : TEXCOORD0;
	float3 ViewDirection : POSITION1;
	float4 Normal : NORMAL0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 VertexPosition = mul(input.Position, World);
	float3 ViewDirection = VertexPosition - CameraPosition;

	float4 Normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.Normal = Normal;
	output.ViewDirection = ViewDirection;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 reflections = normalize(reflect(normalize(input.ViewDirection), normalize(input.Normal)));
	reflections.x = -reflections.x;

	float4 color = texCUBE(SkyboxSampler, reflections);

	return TintColor * color.bgra;
}

technique reflection
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};
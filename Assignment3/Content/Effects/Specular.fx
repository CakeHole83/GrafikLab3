﻿#if OPENGL
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

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

float4x4 WorldInverseTranspose;

float3 CameraPosition;

float3 DiffuseLightDirection = float3(1, 1, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float Shininess = 200;
float4 SpecularColor = float4(1, 1, 1, 1);
float SpecularIntensity = 1;
float3 ViewVector = float3(1, 0, 0);

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float3 Normal : TEXCOORD0;
	float3 ViewDirection : POSITION1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 VertexPosition = mul(input.Position, World);
	float3 ViewDirection = VertexPosition - CameraPosition;

	float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
	
	output.ViewDirection = normalize(ViewDirection);
	output.Normal = normal;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 light = normalize(DiffuseLightDirection);
	float3 normal = normalize(input.Normal);
	float3 r = normalize(2 * dot(light, normal) * normal - light);
	float3 v = normalize(mul(normalize(ViewVector), World));

	float lightIntensity = dot(normal, DiffuseLightDirection);
	float Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	float dotProduct = dot(r, v);
	float4 specular = SpecularIntensity * SpecularColor * max(pow(abs(dotProduct), Shininess), 0) * length(Color);

	return saturate(input.Color + AmbientColor * AmbientIntensity + specular);
}

technique TextureShader
{
    pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}
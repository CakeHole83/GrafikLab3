#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif
float4x4 matWorldViewProj;
float4x4 matWorld;
float4x4 matWorldIT;
float4 AmbientColor;
float4 PlayerPos;
float AmbientIntensity = 0.6;
float StartFog = 112;
float4 ColorFog = { 0.96,0.96,0.96,1 };
float3 LightDirection = { 1,1,1 };

struct VS_INPUT

{
	float4 Position : POSITION;
	float3 Normal : NORMAL;
};



struct VS_OUTPUT

{
	float4 Position: POSITION;
	float3 Light : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float Fog : FOG;
};

VS_OUTPUT vs_main(VS_INPUT In)
{
	VS_OUTPUT Out;
	Out.Position = mul(In.Position, matWorldViewProj);
	Out.Light = normalize(LightDirection);
	Out.Normal = normalize(mul(matWorld, In.Normal));
	float4 PlayerPosWorld = mul(PlayerPos, matWorld);

	float DistFog = distance(In.Position.xy, PlayerPosWorld.xy);
	Out.Fog = saturate(exp((StartFog - DistFog)*0.33));
	return Out;
}

struct PS_OUTPUT
{
	float4 Color : COLOR;
};

PS_OUTPUT ps_main(VS_OUTPUT In) : COLOR
{
	PS_OUTPUT Out;
float4 ColorBase = saturate(AmbientIntensity * AmbientColor + dot(In.Light,In.Normal));
Out.Color = lerp(ColorFog, ColorBase, In.Fog);
return Out;
}

technique Fog
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL vs_main();
		PixelShader = compile PS_SHADERMODEL ps_main();
	}
}

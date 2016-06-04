/*
* Shader genérico para TgcMesh.
* Hay 3 Techniques, una para cada MeshRenderType:
*	- VERTEX_COLOR
*	- DIFFUSE_MAP
*	- DIFFUSE_MAP_AND_LIGHTMAP
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float time = 0;
float4 posMesh;
//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
	Texture = (texLightMap);
};


//Glow
float screen_dx;					// tamaño de la pantalla en pixels
float screen_dy;
texture g_RenderTarget;
sampler RenderTarget = 
sampler_state
{
    Texture = <g_RenderTarget>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture g_GlowMap;
sampler GlowMap = 
sampler_state
{
    Texture = <g_GlowMap>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};
//Fin glow



/**************************************************************************************/
/* VERTEX_COLOR */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_VERTEX_COLOR 
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
};

//Output del Vertex Shader
struct VS_OUTPUT_VERTEX_COLOR
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
};


//Vertex Shader
VS_OUTPUT_VERTEX_COLOR vs_VertexColor(VS_INPUT_VERTEX_COLOR input)
{
	VS_OUTPUT_VERTEX_COLOR output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;
	  
	return output;
}

//Input del Pixel Shader
struct PS_INPUT_VERTEX_COLOR 
{
	float4 Color : COLOR0; 
};

//Pixel Shader
float4 ps_VertexColor(PS_INPUT_VERTEX_COLOR input) : COLOR0
{      
	return input.Color;
}

/*
* Technique VERTEX_COLOR
*/
technique VERTEX_COLOR
{
   pass Pass_0
   {
		VertexShader = compile vs_3_0 vs_VertexColor();
		PixelShader = compile ps_3_0 ps_VertexColor();
   }
}


/**************************************************************************************/
/* DIFFUSE_MAP */
/**************************************************************************************/


//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
};


//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input)
{
	VS_OUTPUT_DIFFUSE_MAP output;
	
	float4 color = tex2Dlod(diffuseMap, float4(input.Texcoord,0,0));
	if(input.Texcoord.y < 0.7 /*&& input.Texcoord.x < 0.3*/&& color.r >0.5){
		input.Position.x = input.Position.x+sin(time*3 + input.Position.y);
		//input.Position.z = input.Position.z+sin(time*3 + input.Position.y);
		//output.Position.x = 2*sin(10*time*output.Position.x);
		//output.Position.x = output.Position.x + sin(time);
	}
	output.WorldPosition = mul(input.Position, matWorld);
	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);
	//Enviar color directamente
	output.Color = input.Color;
	
	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;
	  
	return output;
}


//Input del Pixel Shader
struct PS_DIFFUSE_MAP
{
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float4 Position : POSITION0;
	float3 WorldPosition : TEXCOORD1;
};

//Pixel Shader
float blur_intensity = 0.05;
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{      
	//Modular color de la textura por color del mesh
	float4 color =  tex2D(diffuseMap, input.Texcoord) * input.Color;
	//float4 Color = color;
	if(input.Texcoord.y < 0.7 /*&& input.Texcoord.x < 0.3*/&& color.r >0.5){
	
		color.r = 1;
		color.g = 0.4;
		color.b = 0.4;
		//Obtener color de textura

	
		//Obtener color segun textura
		//float4 color2 = tex2D( RenderTarget, Input.Texcoord );
		//Escalar el color para oscurecerlo
		//float value = ((color2.r + color2.g + color2.b) / 3) * scaleFactor; 
		//color2.rgb = color2.rgb * (1 - scaleFactor) + value * scaleFactor;
		//Escalar el color para oscurecerlo
		//Promediar todos
		//color = color1;
	}
	
	return color;
}



/*
* Technique DIFFUSE_MAP
*/
technique DIFFUSE_MAP
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 vs_DiffuseMap();
	  PixelShader = compile ps_3_0 ps_DiffuseMap();
   }
}

/**************************************************************************************/
/* DIFFUSE_MAP_AND_LIGHTMAP */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP vs_diffuseMapAndLightmap(VS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP input)
{
	VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;
	
	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;
	output.TexcoordLightmap = input.TexcoordLightmap;

	return output;
}



//Input del Pixel Shader
struct PS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
};

//Pixel Shader
float4 ps_diffuseMapAndLightmap(PS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP input) : COLOR0
{      
	//Obtener color de diffuseMap y de Lightmap
	float4 albedo = tex2D(diffuseMap, input.Texcoord);
	float4 lightmapColor = tex2D(lightMap, input.TexcoordLightmap);

	//Modular ambos colores por color del mesh
	return albedo * lightmapColor * input.Color;
}


//technique DIFFUSE_MAP_AND_LIGHTMAP
technique DIFFUSE_MAP_AND_LIGHTMAP
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 vs_diffuseMapAndLightmap();
	  PixelShader = compile ps_3_0 ps_diffuseMapAndLightmap();
   }
}


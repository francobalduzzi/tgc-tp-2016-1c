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

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
	Texture = (texLightMap);
};
// Gaussian Blur

static const int kernel_r = 6;
static const int kernel_size = 13;
static const float Kernel[kernel_size] = 
{
    0.002216,    0.008764,    0.026995,    0.064759,    0.120985,    0.176033,    0.199471,    0.176033,    0.120985,    0.064759,    0.026995,    0.008764,    0.002216,
};


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
	
	output.Position = input.Position;
	float4 color = tex2Dlod(diffuseMap, float4(input.Texcoord,0,0));
	if(input.Texcoord.y < 0.7 /*&& input.Texcoord.x < 0.3*/&& color.r >0.5){
		output.Position.x = 2*sin(0.1*output.Position.y*time)/*+  cos(time*output.Position.z)*/;
		//output.Position.x = 2*sin(10*time*output.Position.x);
		//output.Position.x = output.Position.x + sin(time);
	}
	output.WorldPosition = mul(input.Position, matWorld);
	//Proyectar posicion
	output.Position = mul(output.Position, matWorldViewProj);

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
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{      
	//Modular color de la textura por color del mesh
	float4 color =  tex2D(diffuseMap, input.Texcoord) * input.Color;
	//float4 Color = color;
	if(input.Texcoord.y < 0.7 /*&& input.Texcoord.x < 0.3*/&& color.r >0.5){
	
		color.r = 1;
		color.g = 0;
		color.b = 0;
		//Aca vamos a hacer cosas locas de BLUR
		//Color = 0;
		//for(int i=0;i<3;++i)
		//for(int j=0;j<3;++j)
		//Color += tex2D(diffuseMap, input.Texcoord/*+float2((float)(i-kernel_r)/10,(float)(j-kernel_r)/10)*/) /** Kernel[i]*Kernel[j]*/; //Pasams diffuse en vez del render target
		//Color.a = 1;
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



//BLUR


void VSCopy( float4 vPos : POSITION, float2 vTex : TEXCOORD0,out float4 oPos : POSITION,out float2 oScreenPos: TEXCOORD0)
{
    oPos = vPos;
	oScreenPos = vTex;
	oPos.w = 1;
}




void Blur(float2 screen_pos  : TEXCOORD0,out float4 Color : COLOR)
{ 
    Color = 0;
	for(int i=0;i<kernel_size;++i)
	for(int j=0;j<kernel_size;++j)
		Color += tex2D(RenderTarget, screen_pos+float2((float)(i-kernel_r)/screen_dx,(float)(j-kernel_r)/screen_dy)) * Kernel[i]*Kernel[j];
	Color.a = 1;

}

//GaussianBlurSeparable

void BlurH(float2 screen_pos  : TEXCOORD0,out float4 Color : COLOR)
{ 
    Color = 0;
	for(int i=0;i<kernel_size;++i)
		Color += tex2D(RenderTarget, screen_pos+float2((float)(i-kernel_r)/screen_dx,0)) * Kernel[i];
	Color.a = 1;
}

void BlurV(float2 screen_pos  : TEXCOORD0,out float4 Color : COLOR)
{ 
    Color = 0;
	for(int i=0;i<kernel_size;++i)
		Color += tex2D(RenderTarget, screen_pos+float2(0,(float)(i-kernel_r)/screen_dy)) * Kernel[i];
	Color.a = 1;

}

technique GaussianBlurSeparable
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 VSCopy();
	  PixelShader = compile ps_3_0 BlurH();
   }
   pass Pass_1
   {
	  VertexShader = compile vs_3_0 VSCopy();
	  PixelShader = compile ps_3_0 BlurV();
   }

}

float4 PSDownFilter4( in float2 Tex : TEXCOORD0 ) : COLOR0
{
    float4 Color = 0;
    for (int i = 0; i < 4; i++)
    for (int j = 0; j < 4; j++)
		Color += tex2D(RenderTarget, Tex+float2((float)i/screen_dx,(float)j/screen_dy));

	return Color / 16;
}



technique DownFilter4
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 VSCopy();
	  PixelShader = compile ps_3_0 PSDownFilter4();
   }

}


technique GaussianBlur
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 VSCopy();
	  PixelShader = compile ps_3_0 Blur();
   }

}


//FIN BLUR

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


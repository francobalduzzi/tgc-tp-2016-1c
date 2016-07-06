/*
* Shaders para efectos de Post Procesadosss
*/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))
/**************************************************************************************/
/* DEFAULT */
/**************************************************************************************/


//Input del Vertex Shader
struct VS_INPUT_DEFAULT 
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DEFAULT
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	//float3 WorldPosition : TEXCOORD1;
};


//Vertex Shader
VS_OUTPUT_DEFAULT vs_default( VS_INPUT_DEFAULT Input )
{
   VS_OUTPUT_DEFAULT Output;

   //Proyectar posicion
   Output.Position = float4(Input.Position.xy, 0, 1);
  // Output.WorldPosition =  mul(input.Position, matWorld);
   //Las Texcoord quedan igual
   Output.Texcoord = Input.Texcoord;

   return( Output );
}
//Textura del Render target 2D
texture render_target2D;
sampler RenderTarget = sampler_state
{
    Texture = (render_target2D);
    MipFilter = NONE;
    MinFilter = NONE;
    MagFilter = NONE;
	/*AddressU = BORDER;
    AddressV = BORDER;*/
	AddressU = CLAMP;
    AddressV = CLAMP;
};


//Input del Pixel Shader
struct PS_INPUT_DEFAULT 
{
	float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   //float3 WorldPosition : TEXCOORD1;
};

//Pixel Shader
float4 ps_default( PS_INPUT_DEFAULT Input ) : COLOR0
{      
	float4 color = tex2D( RenderTarget, Input.Texcoord );
	return color;
}



/**************************************************************************************/
/* BLUR */
/**************************************************************************************/

float blur_intensity;
float time;
//Pixel Shader de Blur
float4 ps_blur( PS_INPUT_DEFAULT Input ) : COLOR0
{     
	//Obtener color de textura
	float4 color = tex2D( RenderTarget, Input.Texcoord );
	
	//Tomar samples adicionales de texels vecinos y sumarlos (formamos una cruz)
	color += tex2D( RenderTarget, float2(Input.Texcoord.x + blur_intensity*time, Input.Texcoord.y));
	color += tex2D( RenderTarget, float2(Input.Texcoord.x - blur_intensity*time, Input.Texcoord.y));
	color += tex2D( RenderTarget, float2(Input.Texcoord.x, Input.Texcoord.y + blur_intensity*time));
	color += tex2D( RenderTarget, float2(Input.Texcoord.x, Input.Texcoord.y - blur_intensity*time));
	
	//Promediar todos
	color = color / 5;
	return color;
}




technique BlurTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_default();
	  PixelShader = compile ps_2_0 ps_blur();
   }
}
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
};


//Input del Pixel Shader
struct PS_INPUT_DEFAULT 
{
	float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   //float3 WorldPosition : TEXCOORD1;
};






float4 posicion;// Posicion del objeto que provoca la difuminacion, la copa en nuestro caso
float4 posCam; // Posicion de la camara

float4 ps_oscurecer( PS_INPUT_DEFAULT Input ) : COLOR0
{     
	//Obtener color segun textura
	float4 color = tex2D( RenderTarget, Input.Texcoord );
	float distanciaPunto = length(posicion.xyz - posCam.xyz);
	float distanciaTextura = length(Input.Texcoord -0.5);
	//Escalar el color para oscurecerlo
	float value = 0.5*distanciaTextura;
	color.rgb = (color.rgb + value/(distanciaPunto/500));

	return color;
}




technique OscurecerTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_default();
	  PixelShader = compile ps_2_0 ps_oscurecer();
   }
}
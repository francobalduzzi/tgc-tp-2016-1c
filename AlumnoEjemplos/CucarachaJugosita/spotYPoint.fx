//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

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

//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialAmbientColor; //Color RGB
float4 materialDiffuseColor; //Color ARGB (tiene canal Alpha)
float3 materialSpecularColor; //Color RGB
float materialSpecularExp; //Exponente de specular


// SPOT 
//Parametros de la Luz
float3 lightColor; //Color RGB de la luz
float4 lightPosition; //Posicion de la luz
float4 eyePosition; //Posicion de la camara
float lightIntensity; //Intensidad de la luz
float lightAttenuation; //Factor de atenuacion de la luz

//Parametros de Spot
float3 spotLightDir; //Direccion del cono de luz
float spotLightAngleCos; //Angulo de apertura del cono de luz (en radianes)
float spotLightExponent; //Exponente de atenuacion dentro del cono de luz



//POINT

float3 lightColorP; //Color RGB de la luz
float4 lightPositionP; //Posicion de la luz
float lightIntensityP; //Intensidad de la luz
float lightAttenuationP; //Factor de atenuacion de la luz


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
	float3 WorldPosition : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 SpotLightVec	: TEXCOORD2;
	float3 SpotHalfAngleVec	: TEXCOORD3;
	float3 PointLightVec	: TEXCOORD4;
	float3 PointHalfAngleVec	: TEXCOORD5;
};


//Vertex Shader
VS_OUTPUT_VERTEX_COLOR vs_VertexColor(VS_INPUT_VERTEX_COLOR input)
{
	VS_OUTPUT_VERTEX_COLOR output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space 
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.SpotLightVec = lightPosition.xyz - output.WorldPosition;
	
	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;
	
	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.SpotHalfAngleVec = viewVector + output.SpotLightVec;
	
	//Matrices de Point
	output.PointLightVec = lightPositionP.xyz - output.WorldPosition;
	output.PointHalfAngleVec = viewVector + output.PointLightVec;
	return output;
}

//Input del Pixel Shader
struct PS_INPUT_VERTEX_COLOR 
{
	float4 Color : COLOR0; 
	float3 WorldPosition : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 SpotLightVec	: TEXCOORD2;
	float3 SpotHalfAngleVec	: TEXCOORD3;
	float3 PointLightVec	: TEXCOORD4;
	float3 PointHalfAngleVec	: TEXCOORD5;
};

//Pixel Shader
float4 ps_VertexColor(PS_INPUT_VERTEX_COLOR input) : COLOR0
{      
	//Normalizar vectores
	float3 Nn = normalize(input.WorldNormal);
	float3 Ln = normalize(input.SpotLightVec);
	float3 Hn = normalize(input.SpotHalfAngleVec);
	
	//Calcular atenuacion por distancia
	float distAtten = length(lightPosition.xyz - input.WorldPosition) * lightAttenuation;
	
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	float spotAtten = dot(-spotLightDir, Ln);
	spotAtten = (spotAtten > spotLightAngleCos) 
					? pow(spotAtten, spotLightExponent)
					: 0.0;
	
	//Calcular intensidad de la luz segun la atenuacion por distancia y si esta adentro o fuera del cono de luz
	float intensity = lightIntensity * spotAtten / distAtten;
	
	//Componente Ambient
	float3 ambientLight = intensity * lightColor * materialAmbientColor;
	
	//Componente Diffuse: N dot L
	float3 n_dot_l = dot(Nn, Ln);
	float3 diffuseLight = intensity * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_l); //Controlamos que no de negativo
	
	//Componente Specular: (N dot H)^exp
	float3 n_dot_h = dot(Nn, Hn);
	float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity * lightColor * materialSpecularColor * pow(max( 0.0, n_dot_h), materialSpecularExp));
	
	
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color del mesh, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
	float4 finalColorS = float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * input.Color + specularLight , materialDiffuseColor.a);
	
	
	
	
	//POINT
	
	
	//Normalizar vectores
	float3 LnP = normalize(input.PointLightVec);
	float3 HnP = normalize(input.PointHalfAngleVec);
	
	//Calcular intensidad de luz, con atenuacion por distancia
	float distAttenP = length(lightPositionP.xyz - input.WorldPosition) * lightAttenuationP;
	float intensityP = lightIntensityP / distAttenP; //Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	
	//Componente Ambient
	float3 ambientLightP = intensityP * lightColorP * materialAmbientColor;
	
	//Componente Diffuse: N dot L
	float3 n_dot_lP = dot(Nn, LnP);
	float3 diffuseLightP = intensity * lightColorP * materialDiffuseColor.rgb * max(0.0, n_dot_lP); //Controlamos que no de negativo
	
	//Componente Specular: (N dot H)^exp
	float3 n_dot_hP = dot(Nn, HnP);
	float3 specularLightP = n_dot_lP <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensityP * lightColorP * materialSpecularColor * pow(max( 0.0, n_dot_hP), materialSpecularExp));
	
	
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color del mesh, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
	float4 finalColorP = float4(saturate(materialEmissiveColor + ambientLightP + diffuseLightP) * input.Color + specularLightP , materialDiffuseColor.a);
	
	return finalColorS + finalColorP;
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
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
	float3 SpotLightVec	: TEXCOORD3;
	float3 SpotHalfAngleVec	: TEXCOORD4;
	float3 PointLightVec	: TEXCOORD5;
	float3 PointHalfAngleVec	: TEXCOORD6;
};


//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input)
{ 
	VS_OUTPUT_DIFFUSE_MAP output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space 
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
	
	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;
	
	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.SpotLightVec = lightPosition.xyz - output.WorldPosition;

	

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.SpotHalfAngleVec = viewVector + output.SpotLightVec;
	
	//POINT
	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.PointLightVec = lightPositionP.xyz - output.WorldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.PointHalfAngleVec = viewVector + output.PointLightVec;

	return output;
}


//Input del Pixel Shader
struct PS_DIFFUSE_MAP
{
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
	float3 SpotLightVec	: TEXCOORD3;
	float3 SpotHalfAngleVec	: TEXCOORD4;
	float3 PointLightVec	: TEXCOORD5;
	float3 PointHalfAngleVec	: TEXCOORD6;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{
	//Normalizar vectores
	float3 Nn = normalize(input.WorldNormal);
	float3 Ln = normalize(input.SpotLightVec);
	float3 Hn = normalize(input.SpotHalfAngleVec);
	
	//Calcular atenuacion por distancia
	float distAtten = length(lightPosition.xyz - input.WorldPosition) * lightAttenuation;
	
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	float spotAtten = dot(-spotLightDir, Ln);
	spotAtten = (spotAtten > spotLightAngleCos) 
					? pow(spotAtten, spotLightExponent)
					: 0.0;
	
	//Calcular intensidad de la luz segun la atenuacion por distancia y si esta adentro o fuera del cono de luz
	float intensity = lightIntensity * spotAtten / distAtten;
	
	//Obtener texel de la textura
	float4 texelColor = tex2D(diffuseMap, input.Texcoord);
	
	//Componente Ambient
	float3 ambientLight = intensity * lightColor * materialAmbientColor;
	
	//Componente Diffuse: N dot L
	float3 n_dot_l = dot(Nn, Ln);
	float3 diffuseLight = intensity * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_l); //Controlamos que no de negativo
	
	//Componente Specular: (N dot H)^exp
	float3 n_dot_h = dot(Nn, Hn);
	float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity * lightColor * materialSpecularColor * pow(max( 0.0, n_dot_h), materialSpecularExp));
	
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
	float4 finalColorS = float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * texelColor + specularLight, materialDiffuseColor.a);
	
	//POINT
	
	//Normalizar vectores
	float3 LnP = normalize(input.PointLightVec);
	float3 HnP = normalize(input.PointHalfAngleVec);
	
	//Calcular intensidad de luz, con atenuacion por distancia
	float distAttenP = length(lightPositionP.xyz - input.WorldPosition) * lightAttenuationP;
	float intensityP = lightIntensityP / distAttenP; //Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	
	//Obtener texel de la textura
	float4 texelColorP = tex2D(diffuseMap, input.Texcoord);
	
	//Componente Ambient
	float3 ambientLightP = intensityP * lightColorP * materialAmbientColor;
	
	//Componente Diffuse: N dot L
	float3 n_dot_lP = dot(Nn, LnP);
	float3 diffuseLightP = intensityP * lightColorP * materialDiffuseColor.rgb * max(0.0, n_dot_lP); //Controlamos que no de negativo
	
	//Componente Specular: (N dot H)^exp
	float3 n_dot_hP = dot(Nn, HnP);
	float3 specularLightP = n_dot_lP <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensityP * lightColorP * materialSpecularColor * pow(max( 0.0, n_dot_h), materialSpecularExp));
	
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
	float4 finalColorP = float4(saturate(materialEmissiveColor + ambientLightP + diffuseLightP) * texelColorP + specularLightP, materialDiffuseColor.a);
	return finalColorS + finalColorP;
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
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
	float3 WorldNormal : TEXCOORD3;
	float3 SpotLightVec	: TEXCOORD4;
	float3 SpotHalfAngleVec	: TEXCOORD5;
	float3 PointLightVec	: TEXCOORD6;
	float3 PointHalfAngleVec	: TEXCOORD7;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP vs_diffuseMapAndLightmap(VS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP input)
{
	VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;
	output.TexcoordLightmap = input.TexcoordLightmap;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space 
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
	
	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;
	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.SpotLightVec = lightPosition.xyz - output.WorldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.SpotHalfAngleVec = viewVector + output.SpotLightVec;
	
	//Point
	
	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.PointLightVec = lightPositionP.xyz - output.WorldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.PointHalfAngleVec = viewVector + output.PointLightVec;

	return output;
}



//Input del Pixel Shader
struct PS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
	float3 WorldNormal : TEXCOORD3;
	float3 SpotLightVec	: TEXCOORD4;
	float3 SpotHalfAngleVec	: TEXCOORD5;
	float3 PointLightVec	: TEXCOORD6;
	float3 PointHalfAngleVec	: TEXCOORD7;
};

//Pixel Shader
float4 ps_diffuseMapAndLightmap(PS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP input) : COLOR0
{		
	//Normalizar vectores
	float3 Nn = normalize(input.WorldNormal);
	float3 Ln = normalize(input.SpotLightVec);
	float3 Hn = normalize(input.SpotHalfAngleVec);
	
	//Calcular atenuacion por distancia
	float distAtten = length(lightPosition.xyz - input.WorldPosition) * lightAttenuation;
	
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	float spotAtten = dot(-spotLightDir, Ln);
	spotAtten = (spotAtten > spotLightAngleCos) 
					? pow(spotAtten, spotLightExponent)
					: 0.0;
	
	//Calcular intensidad de la luz segun la atenuacion por distancia y si esta adentro o fuera del cono de luz
	float intensity = lightIntensity * spotAtten / distAtten;
	
	//Obtener color de diffuseMap y de Lightmap
	float4 texelColor = tex2D(diffuseMap, input.Texcoord);
	float4 lightmapColor = tex2D(lightMap, input.TexcoordLightmap);
	
	//Componente Ambient
	float3 ambientLight = intensity * lightColor * materialAmbientColor;
	
	//Componente Diffuse: N dot L
	float3 n_dot_l = dot(Nn, Ln);
	float3 diffuseLight = intensity * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_l); //Controlamos que no de negativo
	
	//Componente Specular: (N dot H)^exp
	float3 n_dot_h = dot(Nn, Hn);
	float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity * lightColor * materialSpecularColor * pow(max( 0.0, n_dot_h), materialSpecularExp));
	
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
	float4 finalColorS = float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * (texelColor * lightmapColor) + specularLight, materialDiffuseColor.a);
	
	//Point
	
	//Normalizar vectores
	float3 LnP = normalize(input.PointLightVec);
	float3 HnP = normalize(input.PointHalfAngleVec);
	
	//Calcular intensidad de luz, con atenuacion por distancia
	float distAttenP = length(lightPositionP.xyz - input.WorldPosition) * lightAttenuationP;
	float intensityP = lightIntensityP / distAttenP; //Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	
	//Obtener color de diffuseMap y de Lightmap
	float4 texelColorP = tex2D(diffuseMap, input.Texcoord);
	float4 lightmapColorP = tex2D(lightMap, input.TexcoordLightmap);
	
	//Componente Ambient
	float3 ambientLightP = intensityP * lightColorP * materialAmbientColor;
	
	//Componente Diffuse: N dot L
	float3 n_dot_lP = dot(Nn, LnP);
	float3 diffuseLightP = intensityP * lightColorP * materialDiffuseColor.rgb * max(0.0, n_dot_lP); //Controlamos que no de negativo
	
	//Componente Specular: (N dot H)^exp
	float3 n_dot_hP = dot(Nn, HnP);
	float3 specularLightP = n_dot_lP <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensityP * lightColorP * materialSpecularColor * pow(max( 0.0, n_dot_hP), materialSpecularExp));
	
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
	float4 finalColorP = float4(saturate(materialEmissiveColor + ambientLightP + diffuseLightP) * (texelColorP * lightmapColorP) + specularLightP, materialDiffuseColor.a);
	
	
	
	return finalColorS + finalColorP;
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
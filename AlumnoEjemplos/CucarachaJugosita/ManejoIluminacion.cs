using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class ManejoIluminacion
    {
        Effect effectSpotYPoint;
        Effect effectPointYPoint;
        ArrayList enemigos;
        List<LuzNormal> listaLuces;
        TgcScene escena;
        ArrayList todosLosElementos;
        ArrayList enemigosARenderizar;
        ArrayList elementosDesaparecedores;
        //Cosas para sombras
        readonly int SHADOWMAP_SIZE = 1024;
        Texture g_pShadowMap;    // Texture to which the shadow map is rendered
        Surface g_pDSShadow;     // Depth-stencil buffer for rendering to shadow map
        Matrix g_mShadowProj;    // Projection matrix for shadow map
        Matrix g_LightView;						// matriz de view del light
        public void setEscena(TgcScene escena)
        {
            this.escena = escena;
        }
        public void setTodosLosElementos(ArrayList todos)
        {
            todosLosElementos = todos;
        }
        public void setEnemigosARenderizar(ArrayList todos)
        {
            enemigosARenderizar = todos;
        }
        public void setListaLuces(List<LuzNormal> listaLuces)
        {
            this.listaLuces = listaLuces;
        }
        public ManejoIluminacion()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            effectSpotYPoint = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\spotYPoint.fx");
            effectPointYPoint = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\pointYPoint.fx");
            //effectSpotYPoint = GuiController.Instance.Shaders.TgcMeshPointLightShader; ;
            //effect.Technique = "MultiplesLuces";


            //Cargamos texturas auxiliares y matrices para pruyectar las sombras
            g_pShadowMap = new Texture(d3dDevice, SHADOWMAP_SIZE, SHADOWMAP_SIZE,
                                       1, Usage.RenderTarget, Format.R32F,
                                       Pool.Default);
            // tengo que crear un stencilbuffer para el shadowmap manualmente
            // para asegurarme que tenga la el mismo tamaño que el shadowmap, y que no tenga 
            // multisample, etc etc.
            g_pDSShadow = d3dDevice.CreateDepthStencilSurface(SHADOWMAP_SIZE,
                                                             SHADOWMAP_SIZE,
                                                             DepthFormat.D24S8,
                                                             MultiSampleType.None,
                                                             0,
                                                             true);
            // por ultimo necesito una matriz de proyeccion para el shadowmap, ya 
            // que voy a dibujar desde el pto de vista de la luz.
            // El angulo tiene que ser mayor a 45 para que la sombra no falle en los extremos del cono de luz
            // de hecho, un valor mayor a 90 todavia es mejor, porque hasta con 90 grados es muy dificil
            // lograr que los objetos del borde generen sombras
            Control panel3d = GuiController.Instance.Panel3d;
            float aspectRatio = (float)panel3d.Width / (float)panel3d.Height;
            g_mShadowProj = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(80),
                aspectRatio, 50, 5000);
            d3dDevice.Transform.Projection =
                Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                aspectRatio, 2f, 1500f);
        }
        public void setElementosDesaparecedores(ArrayList elem)
        {
            elementosDesaparecedores = elem;
        }
        public LuzNormal luzMasCercana(Vector3 pos)
        {
            float minDist = float.MaxValue;
            LuzNormal luzCerca = null;
            foreach(LuzNormal luz in listaLuces)
            {
                float distanciaSq = Vector3.LengthSq(pos - luz.Posicion);
                if(distanciaSq < minDist)
                {
                    minDist = distanciaSq;
                    luzCerca = luz;
                }
            }
            return luzCerca;

        }

        /*public void titilar()
        {
            foreach(LuzNormal luz in listaLuces)
            {
                luz.titilar();
            }
        }*/
        public void iluminar(TipoIluminador objeto, Camara camara)
        {
            //titilar();
            Device device = GuiController.Instance.D3dDevice;
            foreach (LuzNormal luz in listaLuces)
            {
                luz.getMesh().render();
            }

            Vector3 lightDir;
            lightDir = camara.getLookAt() - camara.getPosition();
            lightDir.Normalize();
            LuzNormal luzDelMesh;
            if(objeto.tipo == TipoIluminador.Tipo.Linterna)
            {
               
                foreach(ElementoDesaparecedor elemento in elementosDesaparecedores)
                {
                    if (elemento.desaparecer())
                    {
                        luzDelMesh = luzMasCercana(elemento.getMesh().BoundingBox.calculateBoxCenter());
                        elemento.getMesh().Effect = effectSpotYPoint;
                        //Inicio cosas de sombras
                        elemento.getMesh().Technique = "RenderShadow";
                        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                        Vector3 dir = elemento.getMesh().Position - luzDelMesh.Posicion;
                        dir.Normalize();
                        g_LightView = Matrix.LookAtLH(luzDelMesh.Posicion, luzDelMesh.Posicion + dir, new Vector3(0, 0, 1));
                        elemento.getMesh().Effect.SetValue("g_mViewLightProj", g_LightView * g_mShadowProj);
                        Surface pOldRT = device.GetRenderTarget(0);
                        Surface pShadowSurf = g_pShadowMap.GetSurfaceLevel(0);
                        device.SetRenderTarget(0, pShadowSurf);
                        Surface pOldDS = device.DepthStencilSurface;
                        device.DepthStencilSurface = g_pDSShadow;
                        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                        device.BeginScene();
                        elemento.getMesh().render();

                        device.EndScene();
                        // restuaro el render target y el stencil
                        device.DepthStencilSurface = pOldDS;
                        device.SetRenderTarget(0, pOldRT);
                        //Fin cosas de sombras
                        elemento.getMesh().Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(elemento.getMesh().RenderType);
                    }
                }
                foreach (ElementoDesaparecedor elemento in elementosDesaparecedores)
                {
                    if (elemento.desaparecer())
                    {
                        luzDelMesh = luzMasCercana(elemento.getMesh().BoundingBox.calculateBoxCenter());
                        //Inicio Seteo cosas sombras
                        elemento.getMesh().Effect.SetValue("g_txShadow", g_pShadowMap);
                        //Fin seteo cosas de sombras
                        elemento.getMesh().Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                        elemento.getMesh().Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                        elemento.getMesh().Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                        elemento.getMesh().Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                        elemento.getMesh().Effect.SetValue("materialSpecularExp", 9f);
                        elemento.getMesh().Effect.SetValue("lightColor", ColorValue.FromColor(objeto.color));
                        elemento.getMesh().Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                        elemento.getMesh().Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                        elemento.getMesh().Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat4Array(lightDir));
                        if (objeto.Encendida)
                        {
                            elemento.getMesh().Effect.SetValue("lightIntensity", objeto.Intensity);
                        }
                        else
                        {
                            elemento.getMesh().Effect.SetValue("lightIntensity", 0);
                        }
                        elemento.getMesh().Effect.SetValue("lightAttenuation", objeto.Attenuation);
                        elemento.getMesh().Effect.SetValue("spotLightAngleCos", FastMath.ToRad(objeto.SpotAngle));
                        elemento.getMesh().Effect.SetValue("spotLightExponent", objeto.SpotExponent);
                        elemento.getMesh().Effect.SetValue("lightColorP", ColorValue.FromColor(luzDelMesh.lightColor));
                        elemento.getMesh().Effect.SetValue("lightPositionP", TgcParserUtils.vector3ToFloat4Array(luzDelMesh.Posicion));
                        elemento.getMesh().Effect.SetValue("lightIntensityP", luzDelMesh.Intensity);
                        elemento.getMesh().Effect.SetValue("lightAttenuationP", luzDelMesh.Attenuation);
                        elemento.getMesh().render();
                    }
                   
                }
                foreach (TgcMesh mesh in todosLosElementos) //Esto lo pongo aca, ya que las sombras tengo que hacerlas primero si no se vuelve loquito
                {
                    mesh.Effect = effectSpotYPoint;

                    mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
                }
                foreach (TgcMesh mesh in todosLosElementos)
                {
                    luzDelMesh = luzMasCercana(mesh.BoundingBox.calculateBoxCenter());

                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", 9f);
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(objeto.color));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                    mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat4Array(lightDir));
                    if (objeto.Encendida)
                    {
                        mesh.Effect.SetValue("lightIntensity", objeto.Intensity);
                    }
                    else
                    {
                        mesh.Effect.SetValue("lightIntensity", 0);
                    }
                    mesh.Effect.SetValue("lightAttenuation", objeto.Attenuation);
                    mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(objeto.SpotAngle));
                    mesh.Effect.SetValue("spotLightExponent", objeto.SpotExponent);
                    mesh.Effect.SetValue("lightColorP", ColorValue.FromColor(luzDelMesh.lightColor));
                    mesh.Effect.SetValue("lightPositionP", TgcParserUtils.vector3ToFloat4Array(luzDelMesh.Posicion));
                    mesh.Effect.SetValue("lightIntensityP", luzDelMesh.Intensity);
                    mesh.Effect.SetValue("lightAttenuationP", luzDelMesh.Attenuation);
                    mesh.render();
                }
                /*foreach (TgcSkeletalMesh mesh in todosLosElementos)
                {
                    mesh.Effect = effectSpotYPoint;
                    mesh.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(mesh.RenderType);
                }
                foreach (TgcSkeletalMesh mesh in todosLosElementos)
                {
                    luzDelMesh = luzMasCercana(mesh.BoundingBox.calculateBoxCenter());
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", 9f);
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(objeto.color));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                    mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat4Array(lightDir));
                    if (objeto.Encendida)
                    {
                        mesh.Effect.SetValue("lightIntensity", objeto.Intensity);
                    }
                    else
                    {
                        mesh.Effect.SetValue("lightIntensity", 0);
                    }
                    mesh.Effect.SetValue("lightAttenuation", objeto.Attenuation);
                    mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(objeto.SpotAngle));
                    mesh.Effect.SetValue("spotLightExponent", objeto.SpotExponent);
                    mesh.Effect.SetValue("lightColorP", ColorValue.FromColor(luzDelMesh.lightColor));
                    mesh.Effect.SetValue("lightPositionP", TgcParserUtils.vector3ToFloat4Array(luzDelMesh.Posicion));
                    mesh.Effect.SetValue("lightIntensityP", luzDelMesh.Intensity);
                    mesh.Effect.SetValue("lightAttenuationP", luzDelMesh.Attenuation);
                    mesh.render();
                }*/
            }
            else
            {
                foreach (ElementoDesaparecedor elemento in elementosDesaparecedores)
                {
                    if (elemento.desaparecer())
                    {
                        elemento.getMesh().Effect = effectPointYPoint;
                       // elemento.getMesh().Effect = GuiController.Instance.Shaders.TgcMeshShader;
                        elemento.getMesh().Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(elemento.getMesh().RenderType);
                    }
                    
                }
                foreach (ElementoDesaparecedor elemento in elementosDesaparecedores)
                {
                    if (elemento.desaparecer())
                    {
                        luzDelMesh = luzMasCercana(elemento.getMesh().BoundingBox.calculateBoxCenter());
                    elemento.getMesh().Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    elemento.getMesh().Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    elemento.getMesh().Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    elemento.getMesh().Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    elemento.getMesh().Effect.SetValue("materialSpecularExp", 0f);
                    elemento.getMesh().Effect.SetValue("lightColor", ColorValue.FromColor(objeto.color));
                    elemento.getMesh().Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                        elemento.getMesh().Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                    if (objeto.Encendida)
                    {
                            elemento.getMesh().Effect.SetValue("lightIntensity", objeto.Intensity);
                    }
                    else
                    {
                            elemento.getMesh().Effect.SetValue("lightIntensity", 0);
                    }
                        elemento.getMesh().Effect.SetValue("lightAttenuation", objeto.Attenuation);
                        elemento.getMesh().Effect.SetValue("lightColorP", ColorValue.FromColor(luzDelMesh.lightColor));
                        elemento.getMesh().Effect.SetValue("lightPositionP", TgcParserUtils.vector3ToFloat4Array(luzDelMesh.Posicion));
                        elemento.getMesh().Effect.SetValue("lightIntensityP", luzDelMesh.Intensity);
                        elemento.getMesh().Effect.SetValue("lightAttenuationP", luzDelMesh.Attenuation);
                        elemento.getMesh().render();
                    }
                    
                }
                foreach (TgcMesh mesh in todosLosElementos)
                {
                    mesh.Effect = effectPointYPoint;
                    //mesh.Effect = GuiController.Instance.Shaders.TgcMeshPointLightShader;
                    //mesh.Effect = GuiController.Instance.Shaders.TgcMeshShader;
                    mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
                }
                foreach (TgcMesh mesh in todosLosElementos)
                {
                    luzDelMesh = luzMasCercana(mesh.BoundingBox.calculateBoxCenter());
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", 9f);
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(objeto.color));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                    if (objeto.Encendida)
                    {
                        mesh.Effect.SetValue("lightIntensity", objeto.Intensity);
                    }
                    else
                    {
                        mesh.Effect.SetValue("lightIntensity", 0);
                    }
                    mesh.Effect.SetValue("lightAttenuation", objeto.Attenuation);
                    mesh.Effect.SetValue("lightColorP", ColorValue.FromColor(luzDelMesh.lightColor));
                    mesh.Effect.SetValue("lightPositionP", TgcParserUtils.vector3ToFloat4Array(luzDelMesh.Posicion));
                    mesh.Effect.SetValue("lightIntensityP", luzDelMesh.Intensity);
                    mesh.Effect.SetValue("lightAttenuationP", luzDelMesh.Attenuation);
                    mesh.AlphaBlendEnable = true;
                    mesh.render();
                }
            }
        }
        
    }
}

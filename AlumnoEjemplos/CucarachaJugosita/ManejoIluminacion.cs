using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
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
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            effectSpotYPoint = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\spotYPoint.fx");
            effectPointYPoint = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\pointYPoint.fx");
            //effectSpotYPoint = GuiController.Instance.Shaders.TgcMeshPointLightShader; ;
            //effect.Technique = "MultiplesLuces";
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
            foreach(LuzNormal luz in listaLuces)
            {
                luz.getMesh().render();
            }

            Vector3 lightDir;
            lightDir = camara.getLookAt() - camara.getPosition();
            lightDir.Normalize();
            LuzNormal luzDelMesh;
            if(objeto.tipo == TipoIluminador.Tipo.Linterna)
            {
                foreach (TgcMesh mesh in todosLosElementos)
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
                foreach(ElementoDesaparecedor elemento in elementosDesaparecedores)
                {
                    if (elemento.desaparecer())
                    {
                        elemento.getMesh().Effect = effectSpotYPoint;
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
                foreach (TgcMesh mesh in escena.Meshes)
                {
                    mesh.Effect = effectPointYPoint;
                    //mesh.Effect = GuiController.Instance.Shaders.TgcMeshPointLightShader;
                    mesh.Effect = GuiController.Instance.Shaders.TgcMeshShader;
                    mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
                }
                foreach (TgcMesh mesh in escena.Meshes)
                {
                    /*luzDelMesh = luzMasCercana(mesh.BoundingBox.calculateBoxCenter());
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
                    mesh.Effect.SetValue("lightAttenuationP", luzDelMesh.Attenuation);*/
                    mesh.render();
                }
            }
        }
    }
}

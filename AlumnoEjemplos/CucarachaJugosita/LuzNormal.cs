﻿using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;
using AlumnoEjemplos.CucarachaJugosita;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class LuzNormal
    {
        public Vector3 Posicion { get; set; }
        public float Intensity { get; set; }
        public float Attenuation { get; set; }
        public Color lightColor { get; set; }
        public TgcMesh mesh;
        public TgcScene antorcha;
        public Effect efectoAntorcha;
        public float time;
        //Cosas glow

        VertexBuffer screenQuadVB;
        Texture renderTarget2D;
        Texture g_pGlowMap;
        Texture g_pRenderTarget4;
        Texture g_pRenderTarget4Aux;
        Surface pOldRT;
        Surface g_pDepthStencil;     // Depth-stencil buffer
        Device d3dDevice;
        

        public virtual void titilar()
        {

        }

        public LuzNormal(Vector3 pos)
        {
            d3dDevice = GuiController.Instance.D3dDevice;          
            time = 2;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            antorcha = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\Antorcha-TgcScene.xml");
            efectoAntorcha = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\efectoAntorcha.fx");
            //efectoAntorcha.Technique = "MoverAntorcha";
            mesh = antorcha.Meshes[0];
            mesh.Position = pos;
            Posicion = pos;
            mesh.Effect = efectoAntorcha;
            mesh.Effect.SetValue("posMesh", TgcParserUtils.vector3ToFloat4Array(mesh.Position));
            mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);

        }

        public void render()
        {

                time += GuiController.Instance.ElapsedTime;
            GuiController.Instance.CustomRenderEnabled = true;

            mesh.Effect.SetValue("time", time);
            mesh.Effect.Technique = "DIFFUSE_MAP";

            mesh.render();


        }

        public void rotateY(float grados)
        {
            mesh.rotateY(grados);
        }

        public void escalar(Vector3 v)
        {
            mesh.Scale = v;
        }

        public TgcMesh getMesh()
        {
            return this.mesh;
        }
    }
}

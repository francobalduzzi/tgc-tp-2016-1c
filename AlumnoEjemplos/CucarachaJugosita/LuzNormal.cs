using Microsoft.DirectX;
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

        public virtual void titilar()
        {

        }

        public LuzNormal(Vector3 pos)
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            antorcha = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\Antorcha-TgcScene.xml");
            mesh = antorcha.Meshes[0];
            mesh.Position = pos;
        }

        public void render()
        {
            antorcha.renderAll();
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

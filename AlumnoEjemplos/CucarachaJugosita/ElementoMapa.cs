using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class ElementoMapa
    {
        private TgcScene escena;
        public TgcMesh mesh;

        public ElementoMapa (String escena,Vector3 posicion,Vector3 direccion)
        {
            var loader = new TgcSceneLoader();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            this.escena = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\" + escena);
            mesh = this.escena.Meshes[0];
            mesh.Position = posicion;
            Vector3 direc = direccion - posicion;
            direc.Y = 0;
            direc.Normalize();
            mesh.rotateY((float)Math.Atan2(direc.X, direc.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
        }
        public void render()
        {
            escena.renderAll();
        }

        public void rotateY(float grados)
        {
            mesh.rotateY(grados);
        }

        public void escalar (Vector3 v)
        {
            mesh.Scale = v;
        }

    }
}

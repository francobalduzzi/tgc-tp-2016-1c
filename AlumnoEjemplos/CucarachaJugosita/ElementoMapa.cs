using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class ElementoMapa
    {
        private TgcScene escena;
        TgcMesh mesh;



        public ElementoMapa (String escena,Vector3 posicion)
        {
            var loader = new TgcSceneLoader();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            this.escena = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\" + escena);
            mesh = this.escena.Meshes[0];
            mesh.Position = posicion;
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

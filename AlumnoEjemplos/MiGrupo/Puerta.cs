using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils._2D;


namespace AlumnoEjemplos.MiGrupo
{
    public class Puerta
    {
        private TgcScene puerta1;
        TgcMesh meshP;
        private float contador = 0;
        public void init (Camara camara)
        {
            var loader = new TgcSceneLoader();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            puerta1 = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\Component_2-TgcScene.xml");
            meshP = puerta1.Meshes[0];
            meshP.Position = new Vector3(15f, 17f, 90f);
        }

        public void moverPuerta(float elapsedTime)
        {
            if (contador < 1000)
            {
                Vector3 vector = new Vector3(0, 0, 1) * (elapsedTime * 50);
                meshP.move(vector);
                contador++;
            }
        }

        public void render()
        {
            puerta1.renderAll();
        }


    }
}

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
        private TgcScene cobertura1;
        TgcMesh meshC;
        TgcMesh meshP;
        private float contador = 0;
        public void init ()
        {
            var loader = new TgcSceneLoader();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            puerta1 = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\Component_1-TgcScene.xml");
            meshP = puerta1.Meshes[0];
            meshP.Position = new Vector3(229f, 60f, 201f);
            cobertura1 = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\cobertura-TgcScene.xml");
            meshC = cobertura1.Meshes[0];
            meshC.Position = new Vector3(229f, 60f, 201f);
        }

        public void moverPuerta(float elapsedTime)
        {
            if (contador < 80)
            {
                meshP.rotateY(-1f*elapsedTime);
                contador++;
            }
        }

        public void render()
        {
            puerta1.renderAll();
            cobertura1.renderAll();
        }


    }
}

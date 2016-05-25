using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using System.Drawing;
using System.IO;

namespace AlumnoEjemplos.CucarachaJugosita
{
    public class PuertaFinal:Puerta
    {
        private NumerosLlaves llaves; //lo usa para saber cuándo se agarraron todas las llaves

        public new void init(Vector3 posP)
        {
            base.init(posP);
            var loader = new TgcSceneLoader();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            /*puerta1 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\Component_1-TgcScene.xml");
            meshP = puerta1.Meshes[0];
            meshP.Position = posP;
            cobertura1 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\cobertura1-TgcScene.xml");
            cobertura2 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\cobertura2-TgcScene.xml");
            cobertura3 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\cobertura3-TgcScene.xml");
            meshC1 = cobertura1.Meshes[0];
            meshC1.Position = posP;
            meshC2 = cobertura2.Meshes[0];
            meshC2.Position = posP;
            meshC3 = cobertura3.Meshes[0];
            meshC3.Position = posP;
            estado = Estado.Cerrado;
            contadorAbierta = 500f;*/
            //todo lo de arriba lo comento. Se va a usar luego para cargar los meshes propios de puerta y coberturas de la Puerta Final (distintos a Puerta comun)
        }
    }
}

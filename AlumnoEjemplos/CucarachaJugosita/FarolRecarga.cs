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
    class FarolRecarga
    {
        public Vector3 posicion;
        public TgcBox colision;
        public TgcScene linternaRe;
        public TgcMesh meshLR;
        public Boolean bandera = false;
        public FarolRecarga(Vector3 pos)
        {
            this.init();
            this.posicion = pos;
            this.colision = TgcBox.fromSize(posicion, new Vector3(30, 200, 30));
            this.meshLR.Position = posicion;

        }
        public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            linternaRe = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\FaroleteRecarga-TgcScene.xml");
            meshLR = linternaRe.Meshes[0];

        }
        public void render(float elapsedTime)
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            if (bandera == false)
            {
                meshLR.rotateY(2f * elapsedTime);
                meshLR.render();
            }

        }
        public Boolean verificarColision(Camara camara)
        {
            if ((TgcCollisionUtils.testSphereAABB(camara.camaraColision, colision.BoundingBox)) && (bandera == false))
            {
                bandera = true;
                return true;
            }
            return false;
        }
    }
}

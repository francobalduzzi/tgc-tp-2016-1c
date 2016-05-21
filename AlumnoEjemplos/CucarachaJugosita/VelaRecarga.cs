using AlumnoEjemplos.CucarachaJugosita;
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
    class VelaRecarga : Recarga
    {
        public Vector3 posicion;
        public TgcBox colision;
        public TgcScene linternaRe;
        public TgcMesh meshLR;
        public Vela vela;
        public Boolean bandera = false;
        public VelaRecarga(Vector3 pos, Vela vela)
        {
            this.init();
            this.vela = vela;
            this.posicion = pos;
            this.colision = TgcBox.fromSize(posicion, new Vector3(30, 100, 30));
            this.meshLR.Position = posicion;

        }
        override public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            linternaRe = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\VelaRecarga-TgcScene.xml");
            meshLR = linternaRe.Meshes[0];

        }
        override public void render(float elapsedTime)
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            if (bandera == false)
            {
                meshLR.rotateY(2f * elapsedTime);
                meshLR.render();
            }

        }
        override public Boolean verificarColision(Camara camara)
        {
            if ((TgcCollisionUtils.testSphereAABB(camara.camaraColision, colision.BoundingBox)) && (bandera == false))
            {
                bandera = true;
                return true;
            }
            return false;
        }
        override public void recarga()
        {
            vela.recargar();
        }
    }
}

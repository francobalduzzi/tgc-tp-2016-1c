using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using AlumnoEjemplos.CucarachaJugosita;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class NightRecarga : ElementoDesaparecedor
    {
        public Vector3 posicion;
        public TgcBox colision;
        public TgcScene nightRe;
        public TgcMesh mesh;
        public Boolean bandera = false;
        public NightRecarga(Vector3 pos)
        {
            this.init();
            this.posicion = pos;
            this.colision = TgcBox.fromSize(posicion, new Vector3(30, 100, 30));
            this.mesh.Position = posicion;

        }

        public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            nightRe = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\yukon+ranger-TgcScene.xml");
            mesh = nightRe.Meshes[0];

        }
        public override TgcMesh getMesh()
        {
            return mesh;
        }
        public void render(float elapsedTime)
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            if (bandera == false)
            {
                mesh.rotateY(2f * elapsedTime);
                //mesh.render();
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

        public override Boolean desaparecer()
        {
            return bandera == false;
        }
        public override void reiniciar()
        {
            bandera = false;
        }
    }
}

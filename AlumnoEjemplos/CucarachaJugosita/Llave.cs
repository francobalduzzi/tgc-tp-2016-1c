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
    class Llave
    {
        public Vector3 posicion;
        public TgcBox colision;
        public TgcScene llave;
        public TgcMesh meshLlave;
        public Boolean bandera = false;
        


        public Llave(Vector3 pos)
        {
            this.init();
            this.posicion = pos;
            this.colision = TgcBox.fromSize(posicion, new Vector3(30, 100, 30));
            this.meshLlave.Position = posicion;
            this.meshLlave.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            
        }
        public TgcMesh getMesh()
        {
            return meshLlave;
        }
        public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            llave = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\llave-TgcScene.xml");
            meshLlave = llave.Meshes[0];
            

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
        public void render(float elapsedTime)
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;

            if (bandera == false){
                meshLlave.rotateY(2f * elapsedTime);
                meshLlave.render();
            }

        }
    }
    
}

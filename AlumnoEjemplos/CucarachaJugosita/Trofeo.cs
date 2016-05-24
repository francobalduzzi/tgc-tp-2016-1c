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
using AlumnoEjemplos.MiGrupo;

namespace AlumnoEjemplos
{
    class Trofeo
    {
        public Vector3 posicion;
        public TgcBox colision;
        public TgcScene llave;
        public TgcMesh meshTrofeo;
        public Boolean bandera = false;
        public float velocidad = 0;


        public Trofeo(Vector3 pos)
        {
            this.init();
            this.posicion = pos;
            this.colision = TgcBox.fromSize(posicion, new Vector3(300, 100, 300));
            this.meshTrofeo.Position = posicion;
            
        }
        public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            llave = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\trofeo-TgcScene.xml");
            meshTrofeo = llave.Meshes[0];
        }
        public Boolean verificarColision(Camara camara)
        {
            if ((TgcCollisionUtils.testSphereAABB(camara.camaraColision, colision.BoundingBox)) && (bandera == false))
            {
                bandera = true;
                camara.bloqueadaPermanente();
                return true;
            }
            return false;
        }
      
        public void render(float elapsedTime)
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            if (bandera == false)
            {
                meshTrofeo.render();
            }
            else
            {
                meshTrofeo.rotateY(elapsedTime *velocidad);
                velocidad=velocidad + (2*elapsedTime);
                meshTrofeo.move(new Vector3(0,0.001f*velocidad,0));
                meshTrofeo.render();

            }
        }
    }
    
}

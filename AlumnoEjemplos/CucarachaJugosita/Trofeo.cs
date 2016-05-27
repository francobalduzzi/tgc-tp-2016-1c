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
    class Trofeo
    {
        public Vector3 posicion;
        public TgcBox colision;
        public TgcScene trofeo;
        public TgcScene mesa;
        public TgcMesh meshTrofeo;
        public TgcMesh meshMesa;
        public Boolean bandera = false;
        public float velocidad = 0;



        public Trofeo(Vector3 pos)
        {
            this.init();
            this.posicion = pos;
            this.colision = TgcBox.fromSize(posicion, new Vector3(300, 100, 300));
            this.meshTrofeo.Position = posicion;
            this.meshMesa.Position = posicion;
            
            meshMesa.move(new Vector3(0,-60f,0));
            meshTrofeo.move(new Vector3(0,5,0));
            
        }
        public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            trofeo = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\trophy-TgcScene.xml");
            meshTrofeo = trofeo.Meshes[0];
            meshTrofeo.Scale=(new Vector3(0.2f, 0.2f, 0.2f));
            mesa= loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\mesaRedonda-TgcScene.xml");
            meshMesa = mesa.Meshes[0];
            meshMesa.Scale=(new Vector3(0.9f,0.9f,0.9f));

        }

        public Boolean verificarColision(Camara camara)
        {
            if ((TgcCollisionUtils.testSphereAABB(camara.camaraColision, colision.BoundingBox)) && (bandera == false))
            {
                bandera = true;
                //camara.bloqueadaPermanente();
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
                meshTrofeo.rotateY(elapsedTime * velocidad);
                velocidad = velocidad + (2 * elapsedTime);
                meshTrofeo.move(new Vector3(0, 0.001f * velocidad, 0));
                meshTrofeo.render();

            }
            meshMesa.render();
        }
    }
    
}

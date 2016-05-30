using Microsoft.DirectX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class EnemigoAnimacion : Enemigo  //Hacemos que herede para que cuando el pj lo vea merlusee un rato, MERLUSA MERLUSA
    {
        public Vector3 cercania;
        public Boolean animada;
        public Boolean activarAnimacion;
        public TgcBox box; //caja para ver cuando lanzar la animacion
        /*public new void reiniciar()
        {
            base.reiniciar();
            animada = false;
            activarAnimacion = false;
        }*/
        public void reiniciarAnimacion()
        {
            animada = false;
            activarAnimacion = false;
        }
        public EnemigoAnimacion(Vector3 cercania, Vector3 dimensionesCaja)
        {
            this.cercania = cercania;
              animada = false;
              activarAnimacion = false;
              //Paths para archivo XML de la malla
              pathMesh = alumnoMediaFolder + "CucarachaJugosita\\Media\\SkeletalAnimations\\Robot\\Robot-TgcSkeletalMesh.xml";

            //Path para carpeta de texturas de la malla
            mediaPath = alumnoMediaFolder + "CucarachaJugosita\\Media\\SkeletalAnimations\\Robot\\";

            //Lista de animaciones disponibles
            animationList = new string[]{
                "Parado",
                "Caminando",
                "Correr",
                "PasoDerecho",
                "PasoIzquierdo",
                "Empujar",
                "Patear",
                "Pegar",
                "Arrojar",
            };

            //Crear rutas con cada animacion
            animationsPath = new string[animationList.Length];
            for (int i = 0; i < animationList.Length; i++)
            {
                animationsPath[i] = mediaPath + animationList[i] + "-TgcSkeletalAnim.xml";
            }

            //Cargar mesh y animaciones
            TgcSkeletalLoader loader2 = new TgcSkeletalLoader();
            mesh = loader2.loadMeshAndAnimationsFromFile(pathMesh, mediaPath, animationsPath);
            //mesh.Position = new Vector3(105, 5.06f, 660);
            mesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            //Crear esqueleto a modo Debug
            mesh.buildSkletonMesh();
            //Elegir animacion Caminando
            // mesh.BoundingBox.move(new Vector3(15,0,-170));
            //mesh.BoundingBox.scaleTranslate(mesh.BoundingBox.Position, new Vector3(4f,0.8f,4f)); // este sera el rango de vision
            bounding = new TgcBox();
            bounding = TgcBox.fromSize(mesh.Position, new Vector3(100f, 100f, 300f));
            bounding.move(new Vector3(15, 0, -170));
            box = new TgcBox();
            box.Position = cercania;
            box.Size = dimensionesCaja;
            
        }

        public override void init()
        {
            mesh.Position = caminoOriginal[0];
            caminoIda = caminoOriginal;
            caminoVuelta = getCaminoOriginalClonado();
            Array.Reverse(caminoVuelta, 0, cantidadWP);
            contador = 0;
            selectedAnim = animationList[2];
            mesh.playAnimation(selectedAnim, true);
        }

        public void recorrerCamino()
        {
            if (activarAnimacion)
            {
                Boolean i = true;
                float elapsedTime = GuiController.Instance.ElapsedTime;
                while (caminoIda[contador] != null && i)
                {
                    if ((mesh.Position - caminoIda[contador]).Length() < 1f)
                    {
                        contador++;
                        if (contador == cantidadWP)
                        {
                            activarAnimacion = false;
                            animada = true;
                        }
                    }
                    else
                    {
                        this.seguirA(caminoIda[contador], elapsedTime, 200f);
                    }
                    i = false;
                }
            }
            
        }

        public void animacion(Vector3 posCam)
        {
            TgcBox cajita = new TgcBox();
            cajita.Position = posCam;
            cajita.Size = new Vector3(10f, 10f, 10f);
            if(TgcCollisionUtils.testAABBAABB(cajita.BoundingBox,box.BoundingBox))
            {
                activarAnimacion = true;
            }
        }
        public override void render(Vector3 posCam)
        {
            //bounding.Rotation = ((Vector3)GuiController.Instance.Modifiers.getValue("rotation"));
            //verSiPerseguir(posCam);
            if (!animada)
            {
                animacion(posCam);
                recorrerCamino();
            }          
             if (activarAnimacion)
             {
                 mesh.animateAndRender();
             }
            if (animada)
            {
                morir();
            }
            //box.BoundingBox.render();   //-->>> Descomentar cuando se quiera ubicar un plano de deteccion de efecto fantasmal
            //bounding.render();
            //bounding.BoundingBox.render();
            //mesh.BoundingBox.render();
        }
        public void morir()
        {
            mesh.Position = new Vector3(0, -200, 0); //Hacemos esto, ya que la posicion del mesh, por mas que no se renderize, afecta al comportamiento de muchos algoritmos, y sacarlo de la lista de enemigos mientras la misma se esta ejecutando, rompe
        }
    }
}

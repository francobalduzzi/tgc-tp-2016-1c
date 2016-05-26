using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class EnemigoAnimacion : Enemigo
    {
        public Vector3 cercania;
        public Boolean animada;
        public Boolean activarAnimacion;
        public EnemigoAnimacion(Vector3 cercania)
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
                    if ((mesh.Position - caminoIda[contador]).Length() < 0.5f)
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
            if((cercania - posCam).Length() < 30f)
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
            //bounding.render();
            //bounding.BoundingBox.render();
            //mesh.BoundingBox.render();
        }
    }
}

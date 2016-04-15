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

namespace AlumnoEjemplos.MiGrupo
{
    class Enemigo
    {
        string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
        string pathMesh;
        string mediaPath;
        string[] animationList;
        string[] animationsPath;
        string selectedAnim;
        const float VELOCIDAD_MOVIMIËNTO = 50f;
        TgcSkeletalMesh mesh;
        TgcScene escena;
        TgcBox bounding;
        Vector3[] caminoOriginal = new Vector3[20];
        Vector3[] caminoIda = new Vector3[20];
        Vector3[] caminoVuelta = new Vector3[20];
        int contador;
        private Estado estado;
        public Vector3[] getCaminoOriginal()
        {
            return caminoOriginal;
        }
        public void setEstado(Estado estado)
        {
            this.estado = estado;
        }
        public void setEscena(TgcScene escena)
        {
            this.escena = escena;
        }
        public enum Estado
        {
            Parado = 0,
            RecorriendoIda = 1,
            RecorriendoVuelta = 2,
            Persiguiendo = 3,
        }
        public Enemigo()
        {
            //Paths para archivo XML de la malla
            pathMesh = alumnoMediaFolder + "MiGrupo\\SkeletalAnimations\\Robot\\Robot-TgcSkeletalMesh.xml";

            //Path para carpeta de texturas de la malla
            mediaPath = alumnoMediaFolder + "MiGrupo\\SkeletalAnimations\\Robot\\";

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
            mesh.Scale = new Vector3(0.5f,0.5f,0.5f);
            //Crear esqueleto a modo Debug
            mesh.buildSkletonMesh();
            //Elegir animacion Caminando
            selectedAnim = animationList[1];
            mesh.playAnimation(selectedAnim, true);
            // mesh.BoundingBox.move(new Vector3(15,0,-170));
            //mesh.BoundingBox.scaleTranslate(mesh.BoundingBox.Position, new Vector3(4f,0.8f,4f)); // este sera el rango de vision
            bounding = new TgcBox();
            bounding = TgcBox.fromSize(mesh.Position, new Vector3(100f, 100f, 300f));
            bounding.move(new Vector3(15, 0, -170));
        }
        public void init()
        {
            mesh.Position = caminoOriginal[0];
            caminoIda = caminoOriginal;
            caminoOriginal.Reverse();
            caminoVuelta = caminoOriginal;
            caminoOriginal.Reverse();
            contador = 0;
        }
        public void seguirA(Vector3 posJugador, float elapsedTime)
        {
            Matrix matrixBox;
            Vector3 direccion = posJugador - mesh.Position;
            direccion.Normalize();
            direccion.Y = 0;
            mesh.rotateY((float)Math.Atan2(direccion.X, direccion.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
            bounding.rotateY((float)Math.Atan2(direccion.X, direccion.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
            direccion *= VELOCIDAD_MOVIMIËNTO * elapsedTime;
            mesh.move(direccion);

            bounding.move(direccion);
        }
        public void recorrerCamino()
        {
            float elapsedTime = GuiController.Instance.ElapsedTime;
            Boolean i = true;
            switch (estado)
            {
                case Estado.RecorriendoIda:
                    while(caminoIda[contador] != null && i )
                    {
                        if(mesh.Position == caminoIda[contador])
                        {
                            contador++;
                        }
                        else
                        {
                            this.seguirA(caminoIda[contador], elapsedTime);
                            i = false;
                        }
                    }
                    if(caminoIda[contador] == null)
                    {
                        contador = 0;
                        estado = Estado.RecorriendoVuelta;
                    }

                    break;
                case Estado.RecorriendoVuelta:
                    while (caminoVuelta[contador] != null && i)
                    {
                        if (mesh.Position == caminoVuelta[contador])
                        {
                            contador++;
                        }
                        else
                        {
                            this.seguirA(caminoVuelta[contador], elapsedTime);
                            i = false;
                        }
                    }
                    if (caminoVuelta[contador] == null)
                    {
                        contador = 0;
                        estado = Estado.RecorriendoVuelta;
                    }

                    break;

            }
        }
        public void render()
        {
            //bounding.Rotation = ((Vector3)GuiController.Instance.Modifiers.getValue("rotation"));
            recorrerCamino();
            mesh.animateAndRender();
            //bounding.render();
            bounding.BoundingBox.render();
            //mesh.BoundingBox.render();
        }
    }
}

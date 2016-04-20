﻿using Microsoft.DirectX;
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

namespace AlumnoEjemplos.MiGrupo
{
    public class Enemigo
    {
        string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
        string pathMesh;
        string mediaPath;
        string[] animationList;
        string[] animationsPath;
        string selectedAnim;
        const float VELOCIDAD_MOVIMIËNTO = 50f;
        const float VELOCIDAD_MOVIMIENTO_CORRER = 200f;
        private Boolean bloqueadoMov = false;
        private float tiempoBloqueado = 100f;
        TgcSkeletalMesh mesh;
        TgcScene escena;
        TgcBox bounding;
        Vector3[] caminoOriginal = new Vector3[20];
        Vector3[] caminoIda = new Vector3[20];
        Vector3[] caminoVuelta = new Vector3[20];
        int contador;
        int cantidadWP;
        private Estado estado;
        public void bloqueado()
        {
            bloqueadoMov = true;
        }
        public void setCantidadWP(int numero)
        {
            this.cantidadWP = numero;
        }
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
            Parado = 0, // Hay que pasarle 2 viewpoints: posicion y direccion donde mira.
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
            caminoVuelta = getCaminoOriginalClonado();
            Array.Reverse(caminoVuelta, 0, cantidadWP);
            contador = 0;
            switch (estado)
            {
                case Estado.Parado:
                    selectedAnim = animationList[0];
                    break;
                case Estado.RecorriendoIda:
                    selectedAnim = animationList[1];
                    break;
                case Estado.RecorriendoVuelta:
                    selectedAnim = animationList[1];
                    break;
                case Estado.Persiguiendo:
                    selectedAnim = animationList[2];
                    break;
            }
            mesh.playAnimation(selectedAnim, true);
        }
        public Vector3[] getCaminoOriginalClonado()
        {
            Vector3[] aux = new Vector3[20];
            int auxContador = cantidadWP;
            int i = 0;
            for (; i< auxContador; i++)
            {
                aux[i] = caminoOriginal[i];
            }
            return aux;
        }
        public Vector3 getPosicion()
        {
            return mesh.Position;
        }
        public Vector3 getDirector()
        {
            Vector3 director;
            switch (estado)
            {
                case Estado.Parado:
                    director = caminoOriginal[1] - mesh.Position;
                    director.Normalize();
                    return director;
                    break;
                case Estado.RecorriendoIda:
                    director = caminoIda[contador] - mesh.Position;
                    director.Normalize();
                    return director;
                    break;
                case Estado.RecorriendoVuelta:
                    director = caminoVuelta[contador] - mesh.Position;
                    director.Normalize();
                    return director;
                    break;
                default:
                    return mesh.Position;
            }
        }
        public void seguirA(Vector3 posJugador, float elapsedTime, float velocidad)
        {
            Vector3 direccion = posJugador - mesh.Position;
            direccion.Normalize();
            direccion.Y = 0;
            mesh.rotateY((float)Math.Atan2(direccion.X, direccion.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
            bounding.rotateY((float)Math.Atan2(direccion.X, direccion.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
            direccion *= velocidad * elapsedTime;
            mesh.move(direccion);

            bounding.move(direccion);
        }
        public void recorrerCamino(Vector3 posCam)
        {
            float elapsedTime = GuiController.Instance.ElapsedTime;
            Boolean i = true;
            switch (estado)
            {
                case Estado.RecorriendoIda:
                    while(caminoIda[contador] != null && i )
                    {
                        if((mesh.Position - caminoIda[contador]).Length() < 0.5f)
                        {
                            contador++;
                            if (contador == cantidadWP)
                            {
                                contador = 0;
                                estado = Estado.RecorriendoVuelta;
                            }
                        }
                        else
                        {
                            if (bloqueadoMov)
                            {
                                tiempoBloqueado -= 50f * elapsedTime;
                                selectedAnim = animationList[0];
                                mesh.playAnimation(selectedAnim, true);
                                if (tiempoBloqueado < 0f)
                                {
                                    tiempoBloqueado = 100f;
                                    bloqueadoMov = false;
                                    selectedAnim = animationList[1];
                                    mesh.playAnimation(selectedAnim, true);
                                }
                            }
                            else
                            {
                                this.seguirA(caminoIda[contador], elapsedTime, VELOCIDAD_MOVIMIËNTO);
                            }
                            i = false;
                        }
                    }
                    

                    break;
                case Estado.RecorriendoVuelta:
                    while (caminoVuelta[contador] != null && i)
                    {
                        if ((mesh.Position - caminoVuelta[contador]).Length() < 0.5f)
                        {
                            contador++;
                            if (contador == cantidadWP)
                            {
                                contador = 0;
                                estado = Estado.RecorriendoIda;
                            }
                        }
                        else
                        {
                            if (bloqueadoMov)
                            {
                                tiempoBloqueado -= 50f * elapsedTime;
                                selectedAnim = animationList[0];
                                mesh.playAnimation(selectedAnim, true);
                                if (tiempoBloqueado < 0f)
                                {
                                    tiempoBloqueado = 100f;
                                    bloqueadoMov = false;
                                    selectedAnim = animationList[1];
                                    mesh.playAnimation(selectedAnim, true);
                                }
                            }
                            else
                            {
                                this.seguirA(caminoVuelta[contador], elapsedTime, VELOCIDAD_MOVIMIËNTO);
                            }
                            i = false;
                        }
                    }
                    

                    break;
                case Estado.Parado:
                    Vector3 direccion = caminoOriginal[1] - mesh.Position;
                    direccion.Normalize();
                    direccion.Y = 0;
                    mesh.rotateY((float)Math.Atan2(direccion.X, direccion.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
                    break;
                case Estado.Persiguiendo:
                    selectedAnim = animationList[2];
                    mesh.playAnimation(selectedAnim, true);
                    seguirA(posCam, elapsedTime, VELOCIDAD_MOVIMIENTO_CORRER);
                    dejarDePerseguir(posCam);
                    break;
            }
        }
        public void dejarDePerseguir(Vector3 posCam)
        {
            if ((mesh.Position - posCam).Length() > 500f)
            {
                selectedAnim = animationList[1];
                mesh.playAnimation(selectedAnim, true);
                estado = Estado.RecorriendoIda;
            }
        }
        public void verSiPerseguir(Vector3 posCam)
        {
            if (calculo(posCam))
            {
                estado = Estado.Persiguiendo;
            }
        }
        public Boolean calculo(Vector3 posicion)
        {
            Vector3 director = this.getDirector();
            Vector3 versorY = new Vector3(0, 1, 0);
            Vector3 perpendicularDir1;
            Vector3 perpendicularDir2;
            Vector3 arista1;
            Vector3 arista2;
            Vector3 arista3;
            float orientacionO;
            float orientacion1;
            float orientacion2;
            float orientacion3;
            director.Y = 0;
            director.Normalize();
            perpendicularDir1 = Vector3.Cross(director, versorY);
            perpendicularDir1.Normalize();
            perpendicularDir1 *= 1.3f;
            perpendicularDir2 = perpendicularDir1 * -1;
            arista1 = mesh.Position;
            arista2 = director + perpendicularDir1;
            arista2 *= 500;
            arista3 = director + perpendicularDir2;
            arista3 *= 500;
            orientacionO = (arista1.X - arista3.X) * (arista2.Z - arista3.Z) - (arista1.Z - arista3.Z) * (arista2.X - arista3.X);
            orientacion1 = (arista1.X - posicion.X) * (arista2.Z - posicion.Z) - (arista1.Z - posicion.Z) * (arista2.X - posicion.X);
            orientacion2 = (arista2.X - posicion.X) * (arista3.Z - posicion.Z) - (arista2.Z - posicion.Z) * (arista3.X - posicion.X);
            orientacion3 = (arista3.X - posicion.X) * (arista1.Z - posicion.Z) - (arista3.Z - posicion.Z) * (arista1.X - posicion.X);
            GuiController.Instance.UserVars.setValue("Inclusion0", orientacionO);
            GuiController.Instance.UserVars.setValue("Inclusion1", orientacion1);
            GuiController.Instance.UserVars.setValue("Inclusion2", orientacion2);
            GuiController.Instance.UserVars.setValue("Inclusion3", orientacion3);
            if (orientacion1 >= 0 && orientacion2 >= 0 && orientacion3 >= 0 && orientacionO >= 0)
            {
                return true;
            }
            if (orientacion1 <= 0 && orientacion2 <= 0 && orientacion3 <= 0 && orientacionO <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void render(Vector3 posCam)
        {
            //bounding.Rotation = ((Vector3)GuiController.Instance.Modifiers.getValue("rotation"));
            verSiPerseguir(posCam);
            recorrerCamino(posCam);
            mesh.animateAndRender();
            //bounding.render();
            bounding.BoundingBox.render();
            //mesh.BoundingBox.render();
        }
    }
}

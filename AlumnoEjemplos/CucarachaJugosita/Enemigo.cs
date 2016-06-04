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
using System.Drawing;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.CucarachaJugosita
{
    public class Enemigo
    {
        protected string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
        protected string pathMesh;
        protected string mediaPath;
        protected string[] animationList;
        protected string[] animationsPath;
        protected string selectedAnim;
        protected const float VELOCIDAD_MOVIMIËNTO = 50f;
        protected const float VELOCIDAD_MOVIMIENTO_CORRER = 150f;
        protected Boolean bloqueadoMov = false;
        protected float tiempoBloqueado = 100f;
        protected TgcSkeletalMesh mesh;
        protected TgcScene escena;
        protected TgcBox bounding;
        protected Vector3[] caminoOriginal = new Vector3[20];
        protected Vector3[] caminoIda = new Vector3[20];
        protected Vector3[] caminoVuelta = new Vector3[20];
        protected int contador;
        protected int cantidadWP;
        protected Estado estado;
        protected Estado estadoAux;
        protected Sliding slidin = new Sliding();
        protected Camara camara;
        protected Tgc3dSound sonidoCaminar;
        protected Tgc3dSound sonidoPerseguir;

        public void reiniciar()
        {
            mesh.Position = caminoOriginal[0];
            contador = 0;
        }
        public void setCamara(Camara camara)
        {
            this.camara = camara;
        }
        public TgcSkeletalMesh getMesh()
        {
            return this.mesh;
        }
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
        public Estado getEstado()
        {
            return estado;
        }
        public Enemigo()
        {
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
            mesh.Scale = new Vector3(0.5f,0.5f,0.5f);
            //Crear esqueleto a modo Debug
            mesh.buildSkletonMesh();
            //Elegir animacion Caminando
            // mesh.BoundingBox.move(new Vector3(15,0,-170));
            //mesh.BoundingBox.scaleTranslate(mesh.BoundingBox.Position, new Vector3(4f,0.8f,4f)); // este sera el rango de vision
            bounding = new TgcBox();
            bounding = TgcBox.fromSize(mesh.Position, new Vector3(100f, 100f, 300f));
            bounding.move(new Vector3(15, 0, -170));
            sonidoCaminar = new Tgc3dSound(GuiController.Instance.AlumnoEjemplosDir + "CucarachaJugosita\\Media\\pasos_16.wav", mesh.Position);
            sonidoCaminar.MinDistance = 30f;
            sonidoPerseguir = new Tgc3dSound(GuiController.Instance.AlumnoEjemplosDir + "CucarachaJugosita\\Media\\monstruoMuyEnojado.wav", mesh.Position);
            sonidoPerseguir.MinDistance = 300f;

        }

        public virtual void init()
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
        public Vector3 meDirijoA()
        {
            switch (estado)
            {
                case Estado.Parado:
                    return  caminoOriginal[1];
                    break;
                case Estado.RecorriendoIda:
                    return caminoIda[contador];
                    break;
                case Estado.RecorriendoVuelta:
                    return caminoVuelta[contador];
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
                    sonidoPerseguir.stop();
                    sonidoCaminar.play(true);
                    while (caminoIda[contador] != null && i )
                    {
                        if((mesh.Position - caminoIda[contador]).LengthSq() < 1f)
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
                    sonidoPerseguir.stop();
                    sonidoCaminar.play(true);
                    while (caminoVuelta[contador] != null && i)
                    {
                        if ((mesh.Position - caminoVuelta[contador]).LengthSq() < 1f)
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
                    sonidoCaminar.stop();
                    Vector3 direccion = caminoOriginal[1] - mesh.Position;
                    direccion.Normalize();
                    direccion.Y = 0;
                    mesh.rotateY((float)Math.Atan2(direccion.X, direccion.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
                    break;
                case Estado.Persiguiendo:
                    sonidoCaminar.stop();
                    sonidoPerseguir.play(true);
                    selectedAnim = animationList[2];
                    mesh.playAnimation(selectedAnim, true);
                    seguirASlider(posCam, elapsedTime, VELOCIDAD_MOVIMIENTO_CORRER);
                    dejarDePerseguir(posCam);
                    break;
            }
        }

        public void seguirASlider(Vector3 posJugador, float elapsedTime, float velocidad)
        {
            Vector3 direccion = posJugador - mesh.Position;
            direccion.Normalize();
            direccion.Y = 0;
            mesh.rotateY(((float)Math.Atan2(direccion.X, direccion.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f)));
            bounding.rotateY((float)Math.Atan2(direccion.X, direccion.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
            direccion *= velocidad;
            TgcBoundingSphere characterSphere = new TgcBoundingSphere(mesh.BoundingBox.calculateBoxCenter(), 10f);
            List<TgcBoundingBox> lista= new List<TgcBoundingBox>();
            foreach(TgcMesh meshito in escena.Meshes)
            {
                lista.Add(meshito.BoundingBox);
            }
            Vector3 realMovement = slidin.moveCharacter(characterSphere, direccion, lista);
            characterSphere.moveCenter(realMovement*elapsedTime);
            mesh.move(realMovement * elapsedTime);
            mesh.Position = new Vector3(mesh.Position.X, 5.02f, mesh.Position.Z);
            
            bounding.move(realMovement);
        }


        public Vector3 calculoNormalPared(TgcMesh mesh)
        {
            Vector3 punto1 = mesh.getVertexPositions()[0];
            Vector3 punto2 = mesh.getVertexPositions()[1];
            Vector3 punto3 = mesh.getVertexPositions()[2];
            Vector3 vectorDir1 = punto2 - punto1;
            Vector3 vectorDir2 = punto3 - punto1;
            Vector3 normalPared = Vector3.Cross(vectorDir1, vectorDir2);
            return normalPared;
        }


        public void dejarDePerseguir(Vector3 posCam)
        {
            if ((mesh.Position - posCam).LengthSq() > 250000f)
            {
                selectedAnim = animationList[1];
                mesh.playAnimation(selectedAnim, true);
                estado = Estado.RecorriendoIda;
                mesh.Position = new Vector3(mesh.Position.X, 5.02f, mesh.Position.Z);
            }
        }
        public void verSiPerseguir(Vector3 posCam)
        {
            if (calculo(posCam) && !camara.escondido)
            {
                estado = Estado.Persiguiendo;
            }
        }
        public Boolean calculo(Vector3 posicion)
        {
            Vector3 director = this.getDirector();
            director.Y = 0;
            director.Normalize();
            Vector3 calculito;
            Vector3 distancia;
            calculito = posicion - mesh.Position;
            calculito.Y = 0;
            calculito.Normalize();
            distancia = posicion - mesh.Position;
            return (Vector3.Dot(director, calculito) >= 0.5 && distancia.LengthSq() <= 250000 && calculoParedesEnMedio(posicion));
        }

        public Boolean calculoParedesEnMedio(Vector3 posicion)
        {
            int contador = 0;
            Ray rayo = new Ray(this.mesh.Position, posicion);
            foreach (TgcMesh mesh in escena.Meshes)
            {
                if (rayo.intersectAABB(mesh.BoundingBox))
                {
                    contador++;
                }
            }
            if (contador == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public virtual void render(Vector3 posCam)
        {
            sonidoCaminar.Position = mesh.Position;
            //bounding.Rotation = ((Vector3)GuiController.Instance.Modifiers.getValue("rotation"));
            verSiPerseguir(posCam);
            recorrerCamino(posCam);
            mesh.animateAndRender();
            //bounding.render();
            //mesh.BoundingBox.render();
        }
    }
}

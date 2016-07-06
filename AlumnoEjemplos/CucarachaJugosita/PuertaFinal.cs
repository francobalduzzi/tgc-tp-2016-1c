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
using System.IO;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.CucarachaJugosita
{
    public class PuertaFinal
    {
        private NumerosLlaves llaves; //lo usa para saber cuándo se agarraron todas las llaves
        private TgcScene puerta1;
        private TgcScene cobertura1;
        private TgcScene cobertura2;
        private TgcScene cobertura3;
        private TgcMesh meshC1;
        private TgcMesh meshC2;
        private TgcMesh meshC3;
        private TgcMesh meshP;
        private Tgc3dSound sonidoAbrir;
        private float contador = 0;
        public Estado estado;
        private TgcText2d text2;
        private float contadorAbierta;
        private Boolean abiertaJugador;
        public enum Estado
        {
            Cerrado = 0,
            Abierta = 1,
        }
        public TgcMesh getMeshP()
        {
            return meshP;
        }
        public TgcMesh getMeshC3()
        {
            return meshC3;
        }
        public TgcMesh getMeshC2()
        {
            return meshC2;
        }
        public TgcMesh getMeshC1()
        {
            return meshC1;
        }
        public Estado getEstado()
        {
            return estado;
        }
        public void init(Vector3 posP, NumerosLlaves llaves)
        {
            this.llaves = llaves;
            //sonidoAbrir = new Tgc3dSound(GuiController.Instance.AlumnoEjemplosDir + "CucarachaJugosita\\Media\\puerta ruidosa, abrir.wav", meshP.Position);
            /*sonidoAbrir = new Tgc3dSound();
            sonidoAbrir.loadSound(GuiController.Instance.AlumnoEjemplosDir + "CucarachaJugosita\\Media\\puerta ruidosa, abrir.wav");
            sonidoAbrir.Position = meshP.Position;*/
            // this.llaves = llaves;
            text2 = new TgcText2d();
            text2.Text = "";
            text2.Color = Color.DarkRed;
            text2.Align = TgcText2d.TextAlign.CENTER;
            text2.Size = new Size(300, 100);
            Size screenSize = GuiController.Instance.Panel3d.Size;
            text2.Position = new Point(screenSize.Width / 2 - text2.Size.Width / 2, screenSize.Height - text2.Size.Height * 3); text2.Size = new Size(300, 100);
            text2.changeFont(new System.Drawing.Font("Chiller", /*30*/GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth / 45, FontStyle.Regular));
            var loader = new TgcSceneLoader();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            puerta1 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\Component_2-TgcScene.xml");
            meshP = puerta1.Meshes[0];
            meshP.Position = posP;
            cobertura1 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\cobertura1-TgcScene.xml");
            cobertura2 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\cobertura2-TgcScene.xml");
            cobertura3 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\cobertura3-TgcScene.xml");
            meshC1 = cobertura1.Meshes[0];
            meshC1.Position = posP;
            meshC2 = cobertura2.Meshes[0];
            meshC2.Position = posP;
            meshC3 = cobertura3.Meshes[0];
            meshC3.Position = posP;
            estado = Estado.Cerrado;
            contadorAbierta = 500f;
            sonidoAbrir = new Tgc3dSound(GuiController.Instance.AlumnoEjemplosDir + "CucarachaJugosita\\Media\\puerta ruidosa, abrir.wav", meshP.Position);
            sonidoAbrir.MinDistance = 10f;
        }

        public void moverPuerta()
        {
            float elapsedTime = GuiController.Instance.ElapsedTime;
            if (estado == Estado.Abierta && !abiertaJugador)
            {
                contadorAbierta -= 80f * elapsedTime;
            }
            if (contadorAbierta < 0f)
            {
                contadorAbierta = 500f;
                estado = Estado.Cerrado;
            }
            if (contador < 90 && estado == Estado.Abierta)
            {
                meshP.rotateY(Geometry.DegreeToRadian(-1f));
                contador++;
            }
            if (contador > 0 && estado == Estado.Cerrado)
            {
                meshP.rotateY(Geometry.DegreeToRadian(1f));
                contador--;
            }
        }

        public void seAbrioJugador()
        {
            if (llaves.juntoTodas())
            {
                sonidoAbrir.play();
                estado = Estado.Abierta;
                abiertaJugador = true;
            }
            
        }
        public void seCerroJugador()
        {
            if (llaves.juntoTodas())
            {
                estado = Estado.Cerrado;
                abiertaJugador = false;
            }           
        }
        public Boolean verificarColision(Camara camara)
        {
            Vector3 direccion = camara.getLookAt() - camara.getPosition();
            direccion.Normalize();
            if ((meshP.Position - camara.getPosition()).Length() <= 100f)
            {
                if (!llaves.juntoTodas())
                {
                    text2.Text = "Debes juntar todas las llaves";
                }
                else
                {
                    if (this.estado == Estado.Cerrado)
                    {
                        text2.Text = "Presiona E para abrir la puerta";
                    }
                    else
                    {
                        text2.Text = "Presiona E para cerrar la puerta";
                    }
                }
                

                return true;
            }
            else
            {
                text2.Text = "";
                return false;
            }
        }

        public void render()
        {
            text2.render();
            this.moverPuerta();
            puerta1.renderAll();
            cobertura1.renderAll();
            cobertura3.renderAll();
            cobertura2.renderAll();
            meshP.BoundingBox.transform(meshP.Transform);
            meshC1.BoundingBox.transform(meshC1.Transform);
            meshC2.BoundingBox.transform(meshC2.Transform);
            meshC3.BoundingBox.transform(meshC3.Transform);
        }

        public void escalar(Vector3 v)
        {
            meshP.Scale = v;
            meshC1.Scale = v;
            meshC2.Scale = v;
            meshC3.Scale = v;
            meshP.BoundingBox.transform(meshP.Transform);
            meshC1.BoundingBox.transform(meshC1.Transform);
            meshC2.BoundingBox.transform(meshC2.Transform);
            meshC3.BoundingBox.transform(meshC3.Transform);
        }

        public void rotateY(float angulo)
        {
            meshP.rotateY(angulo);
            meshC1.rotateY(angulo);
            meshC2.rotateY(angulo);
            meshC3.rotateY(angulo);
            meshP.BoundingBox.transform(meshP.Transform);
            meshC1.BoundingBox.transform(meshC1.Transform);
            meshC2.BoundingBox.transform(meshC2.Transform);
            meshC3.BoundingBox.transform(meshC3.Transform);
        }
    }
}

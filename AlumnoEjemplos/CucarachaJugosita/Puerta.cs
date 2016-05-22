﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils._2D;


namespace AlumnoEjemplos.MiGrupo
{
    public class Puerta
    {
        private TgcScene puerta1;
        private TgcScene cobertura1;
        private TgcScene cobertura2;
        private TgcScene cobertura3;
        TgcMesh meshC1;
        TgcMesh meshC2;
        TgcMesh meshC3;
        TgcMesh meshP;
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

        public void init(Vector3 posP)
        {
            text2 = new TgcText2d();
            text2.Text = "";
            text2.Color = Color.DarkRed;
            text2.Align = TgcText2d.TextAlign.CENTER;
            text2.Position = new Point(500, 500);
            text2.Size = new Size(300, 100);
            text2.changeFont(new System.Drawing.Font("Chiller", 30, FontStyle.Regular));
            var loader = new TgcSceneLoader();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            puerta1 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\Component_1-TgcScene.xml");
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
                meshP.BoundingBox.transform(meshP.Transform);
            }
            if (contador > 0 && estado == Estado.Cerrado)
            {
                meshP.rotateY(Geometry.DegreeToRadian(1f));
                contador--;
                meshP.BoundingBox.transform(meshP.Transform);
            }
        }

        public Estado getEstado()
        {
            return estado;
        }
        public void seAbrio()
        {
            estado = Estado.Abierta;
        }
        public void seAbrioJugador()
        {
            estado = Estado.Abierta;
            abiertaJugador = true;
        }
        public void seCerroJugador()
        {
            estado = Estado.Cerrado;
            abiertaJugador = false;
        }
        public Boolean verificarColision(Camara camara)
        {
            Vector3 direccion = camara.getLookAt() - camara.getPosition();
            direccion.Normalize();
            if ((meshP.Position-camara.getPosition()).Length() <= 50f)
            {
                if(this.estado == Estado.Cerrado)
                {
                    text2.Text = "Presiona E para abrir la puerta";
                }
                else
                {
                    text2.Text = "Presiona E para cerrar la puerta";
                }
                
                return true;
            }
            else
            {
                text2.Text = "";
                return false;
            }
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
        public Boolean interseccionRayoPlano(Vector3 Origen, Vector3 Destino, TgcMesh mesh)
        {
            Vector3 normalPared = calculoNormalPared(mesh);
            Vector3 puntoPared = mesh.getVertexPositions()[0];
            Vector3 calculo1 = puntoPared - Origen;
            Vector3 calculo2 = Destino;
            if (Vector3.Dot(normalPared, calculo2) == 0)
            {
                return false;
            }
            else
            {
                float r1 = Vector3.Dot(normalPared, calculo1) / Vector3.Dot(normalPared, calculo2);
                if (r1 >= 0 )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Boolean verificarColision(Enemigo enemigo)
        {
            Vector3 direccion = enemigo.getDirector();
            direccion = direccion * 2;
            if (estado == Estado.Cerrado && enemigo.getEstado() != Enemigo.Estado.Persiguiendo)
            {
                if (TgcCollisionUtils.intersectSegmentAABB(enemigo.getPosicion(), direccion, meshP.BoundingBox, out direccion))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
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
        }

        public void escalar(Vector3 v)
        {
            meshP.Scale = v;
            meshC1.Scale = v;
            meshC2.Scale = v;
            meshC3.Scale = v;
        }
    }
}
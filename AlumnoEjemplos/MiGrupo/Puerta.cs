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
        TgcMesh meshC;
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
        public TgcMesh getMeshC()
        {
            return meshC;
        }
        public void init(Vector3 posP)
        {
            text2 = new TgcText2d();
            text2.Text = "";
            text2.Color = Color.DarkSalmon;
            text2.Align = TgcText2d.TextAlign.RIGHT;
            text2.Position = new Point(500, 500);
            text2.Size = new Size(300, 100);
            text2.changeFont(new System.Drawing.Font("TimesNewRoman", 25, FontStyle.Bold | FontStyle.Italic));
            var loader = new TgcSceneLoader();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            puerta1 = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\Component_1-TgcScene.xml");
            meshP = puerta1.Meshes[0];
            meshP.Position = posP;
            cobertura1 = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\cobertura-TgcScene.xml");
            meshC = cobertura1.Meshes[0];
            meshC.Position = posP;
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
            direccion = direccion * 1;
            if (TgcCollisionUtils.intersectSegmentAABB(camara.getPosition(), direccion, meshP.BoundingBox, out direccion))
            {
                text2.Text = "Presiona la tecla e para abrir la puerta";
                return true;
            }
            else
            {
                text2.Text = "";
                return false;
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
            meshP.BoundingBox.render();
            meshC.BoundingBox.render();
            this.moverPuerta();
            puerta1.renderAll();
            cobertura1.renderAll();
        }

        public void escalar(Vector3 v)
        {
            meshP.Scale = v;
            meshC.Scale = v;
        }
    }
}

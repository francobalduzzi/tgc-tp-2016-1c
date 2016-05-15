using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class Escondite
    {
        List<Enemigo> enemigos;
        TgcMesh mesh;
        private TgcText2d text2;

        public void init(Vector3 posicion)
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            var loader = new TgcSceneLoader();
            mesh = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\LockerMetal-TgcScene.xml").Meshes[0];
            mesh.Position = posicion;
            text2 = new TgcText2d();
            text2.Text = "";
            text2.Color = Color.DarkRed;
            text2.Align = TgcText2d.TextAlign.CENTER;
            text2.Position = new Point(500, 500);
            text2.Size = new Size(300, 100);
            text2.changeFont(new System.Drawing.Font("Chiller", 30, FontStyle.Regular));
        }
        public void setEnemigos(List<Enemigo> enemigos)
        {
            this.enemigos = enemigos;
        }

        public Boolean verificarColision(Camara camara)
        {
            if ((mesh.Position - camara.getPosition()).Length() <= 100f)
            {
                if (camara.getEscondido())
                {
                    text2.Text = "Presiona E para salir";
                }
                else
                {
                    text2.Text = "Presiona E para esconderse";
                }
                
                return true;
            }
            else
            {
                text2.Text = "";
                return false;
            }
        }

        public void esconder(Camara camara)
        {
            camara.esconder(mesh.Position);
        }

        public void render()
        {
            text2.render();
            mesh.render();
        }
    }
}

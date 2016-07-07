using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class Escondite
    {
        List<Enemigo> enemigos;
        TgcMesh mesh;
        private TgcText2d text2;
        Vector3 direccion;
        Tipo tipo;
        public void init(Vector3 posicion, Vector3 direccion, String escena, Tipo tipo)
        {
            this.direccion = direccion;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            mesh = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\" + escena).Meshes[0];
            mesh.Position = posicion;
            mesh.AlphaBlendEnable = true;
            Vector3 direc = direccion - posicion;
            direc.Y = 0;
            direc.Normalize();
            mesh.rotateY((float)Math.Atan2(direc.X, direc.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
            text2 = new TgcText2d();
            text2.Text = "";
            text2.Color = Color.DarkRed;
            text2.Align = TgcText2d.TextAlign.CENTER;
            text2.Size = new Size(300, 100);
            Size screenSize = GuiController.Instance.Panel3d.Size;
            text2.Position = new Point(screenSize.Width / 2 - text2.Size.Width / 2, screenSize.Height * 6 / 10);
            text2.changeFont(new System.Drawing.Font("Chiller", /*30*/GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth / 45, FontStyle.Regular));
            this.tipo = tipo;
        }
        public enum Tipo
        {
            Casillero = 0,
            Mesa = 1,
        }
        public TgcMesh getMesh()
        {
            return mesh;
        }
        public void setEnemigos(List<Enemigo> enemigos)
        {
            this.enemigos = enemigos;
        }

        public Boolean verificarColision(Camara camara, Boolean perseguido)
        {
            if ((mesh.Position - camara.getPosition()).Length() <= 100f)
            {
                if (perseguido)
                {
                    text2.Text = "No puedes esconderte mientras estas siendo perseguido";
                    return false;
                }
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
            Vector3 posicion;
            switch (tipo)
            {
                case Tipo.Mesa:
                    posicion = mesh.Position;
                    posicion.Y = 15f;
                    break;
                case Tipo.Casillero:
                    posicion = mesh.Position;
                    posicion.Y = camara.getPosition().Y;
                    break;
                default:
                    posicion = mesh.Position;
                    posicion.Y = camara.getPosition().Y;
                    break;
            }
            camara.esconder(posicion, direccion);
        }

        public void render()
        {
            text2.render();
            
        }
    }
}

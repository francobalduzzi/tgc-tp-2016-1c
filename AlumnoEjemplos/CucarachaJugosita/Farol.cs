using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class Farol : TipoIluminador
    {
        
        private TgcText2d text2;
        public TgcScene farol;
        private float movimientoFarol;
        const float VALORMAXIMOINTENSIDAD = 12;
        Barra barra;
        override public void mover(float elapsedTime)
        {
            movimientoFarol += elapsedTime;
            var random = FastMath.Cos(6 * movimientoFarol);
            var random2 = FastMath.Cos(12 * movimientoFarol);
            farol.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(0f + random2), Geometry.DegreeToRadian(0f + random), Geometry.DegreeToRadian(0f));
        }
        override public void init()
        {
            tipo = Tipo.Farol;
            text2 = new TgcText2d();
            text2.Text = "100%";
            text2.Color = Color.DarkSalmon;
            text2.Align = TgcText2d.TextAlign.RIGHT;
            text2.Position = new Point(950, 700);
            text2.Size = new Size(300, 100);
            text2.changeFont(new System.Drawing.Font("TimesNewRoman", 25, FontStyle.Bold | FontStyle.Italic));
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            farol = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\FaroleteRecarga-TgcScene.xml");
            farol.Meshes[0].Scale = new Vector3(0.8f, 0.8f, 0.8f);
            farol.Meshes[0].Position = Posicion + new Vector3(-10f, -20f, 17f);
            Intensity = VALORMAXIMOINTENSIDAD;
            Attenuation = 0.3f;
            this.color = Color.Yellow;
            barra = new Barra();
            Encendida = true;
        }
        public void recargar()
        {
            this.Intensity += VALORMAXIMOINTENSIDAD * 0.25f;
            if (this.Intensity > VALORMAXIMOINTENSIDAD)
            {
                this.Intensity = VALORMAXIMOINTENSIDAD;
            }
        }

        override public void render()
        {
           // text2.render();
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            GuiController.Instance.D3dDevice.Transform.View = Matrix.Identity;
            farol.renderAll();
            GuiController.Instance.D3dDevice.Transform.View = matrizView;
            barra.render(this.damePorcentaje());
        }
        override public void CambiarEstadoLuz()
        {
            this.Encendida = !this.Encendida;
        }
        public float damePorcentaje()
        {
            return ((this.Intensity) / VALORMAXIMOINTENSIDAD);
        }

        public override void bajarIntensidad(float elapsedTime)
        {
            if (this.Intensity > -0.05f && this.Intensity < 0.05f)
            {
                this.Encendida = false;
            }
            if (this.Encendida)
            {
                this.Intensity = (Intensity - (0.2f * elapsedTime));//si es necesario modificar velocidad de reduccion  
            }
        }
        public void reiniciar()
        {
            this.Intensity = VALORMAXIMOINTENSIDAD;
            this.Encendida = true;
        }
       
    }

}

using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.CucarachaJugosita
{
    public class NumerosLlaves
    {
        TgcText2d textoActual;
        int numeroDeLlaves;
        public int recolectadas;
        public NumerosLlaves()
        {
            this.init();
        }
        public void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            numeroDeLlaves = 0;
            recolectadas = 0;
           textoActual = new TgcText2d();
           textoActual.Text = "Llaves:"+recolectadas+"/"+numeroDeLlaves;
            Size screenSize = GuiController.Instance.Panel3d.Size;
            //(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth/13000f, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight/ 13000f
            textoActual.Size = new Size(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth/13000, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight/13000);
            textoActual.Position = new Point(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth *17/20, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight * 83 / 100); 
           // textoActual.Position = new Point(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth * 11/32, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight * 8/10); -->> Esta estaba antes por las dudas
            new Vector2();
            //textoActual.Position = new Point(screenSize.Width - textoActual.Size.Width*31/20, screenSize.Height - textoActual.Size.Height*3/2);
            textoActual.Color = Color.DarkRed;
           textoActual.changeFont(new System.Drawing.Font("Chiller", 30, FontStyle.Regular));
        }
        public void setNumeroLLaves(int tamañoLista)
        {
            numeroDeLlaves = tamañoLista;
            textoActual.Text= "Llaves:" + recolectadas + "/" + numeroDeLlaves;
        }
        public void recolectoLlave()
        {
            recolectadas++;
            textoActual.Text = "Llaves:" + recolectadas + "/" + numeroDeLlaves;
        }
        public Boolean juntoTodas()
        {
            return recolectadas == numeroDeLlaves;
        }
        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            textoActual.render();
        }
        public void reiniciar()
        {
            recolectadas = 0;
            textoActual.Text = "Llaves:" + recolectadas + "/" + numeroDeLlaves;
        }
    }
}

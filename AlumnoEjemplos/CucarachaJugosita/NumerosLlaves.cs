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
            //textoActual.Position = new Point(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth * 3/16, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight * 18/20);  <-- Relacion si se quiere a la izquierda de la barra
            textoActual.Position = new Point(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth * 11/32, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight * 8/10);
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

using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using System.Text;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class NumerosLlaves
    {
        TgcText2d textoActual;
        int numeroDeLlaves = 0;
        int recolectadas = 0;
        public NumerosLlaves()
        {
            this.init();
        }
        public void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

           textoActual = new TgcText2d();
           textoActual.Text = "Llaves:"+recolectadas+"/"+numeroDeLlaves;
            textoActual.Align = TgcText2d.TextAlign.LEFT;
           textoActual.Position = new Point(100, 530);
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
    }
}

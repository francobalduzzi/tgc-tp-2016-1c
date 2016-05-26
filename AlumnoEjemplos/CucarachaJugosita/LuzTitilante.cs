using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class LuzTitilante : LuzNormal
    {
        public float contador;
        public Boolean encendida;
        public Boolean cambio;
        public float intensidadAux;
        public LuzTitilante()
        {
            encendida = true;
            contador = 0;
            cambio = false;
        }
        public override void titilar()
        {
            contador += GuiController.Instance.ElapsedTime;
            if(contador > 0.5f)
            {
                contador = 0;
                encendida = !encendida;
                cambio = true;
            }
            if (cambio)
            {
                if (!encendida)
                {
                    intensidadAux = Intensity;
                    Intensity = 0;
                }
                else
                {
                    Intensity = intensidadAux;
                }
                cambio = false;
            }
            
        }
    }
}

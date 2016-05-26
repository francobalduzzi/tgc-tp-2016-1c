using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class LuzNormal
    {
        public Vector3 Posicion { get; set; }
        public float Intensity { get; set; }
        public float Attenuation { get; set; }
        public Color lightColor { get; set; }
        public virtual void titilar()
        {

        }
    }
}

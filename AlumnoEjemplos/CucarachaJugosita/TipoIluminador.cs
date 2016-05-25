using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CucarachaJugosita
{
    public abstract class TipoIluminador
    {
        public Vector3 Direccion { get; set; } //Posicion y direccion de la camara no de la linterna
        public Vector3 Posicion { get; set; }
        public float SpotAngle { get; set; }
        public float SpotExponent { get; set; }
        public float Intensity { get; set; }
        public float Attenuation { get; set; }
        public bool Encendida { get; set; }
        public float SpecularEx { get; set; }
        public Color color { get; set; }
        public Tipo tipo;
        public int Duracion;
        public abstract void mover(float elapsedTime);
        public abstract void init();
        public abstract void render();
        public abstract void CambiarEstadoLuz();
        public abstract void bajarIntensidad(float elapsedTime);
        public enum Tipo
        {
            Linterna = 0,
            Farol = 1,
            Vela = 2,
        }
    }
}

using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
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
        public int Duracion;
        public abstract void actualizarEscenario(TgcScene escena, Camara camara);
        public abstract void mover(float elapsedTime);
        public abstract void init();
        public abstract void render();
        public abstract void CambiarEstadoLuz();
        public abstract void bajarIntensidad(float elapsedTime);
    }
}

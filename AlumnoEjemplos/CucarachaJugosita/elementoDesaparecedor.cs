using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CucarachaJugosita
{
    public abstract class ElementoDesaparecedor
    {
        public abstract Boolean desaparecer();
        public abstract TgcMesh getMesh();
        public abstract void reiniciar();
    }
}

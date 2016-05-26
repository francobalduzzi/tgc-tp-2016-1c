using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CucarachaJugosita
{
    abstract class Recarga
    {
        public abstract void init();
        public abstract void render(float elapsedTime);
        public abstract Boolean verificarColision(Camara camara);
        public abstract void recarga();
        public abstract TgcMesh getMesh();
    }
}

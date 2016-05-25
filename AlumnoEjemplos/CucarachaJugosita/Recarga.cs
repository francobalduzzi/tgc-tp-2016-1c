using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.CucarachaJugosita
{
    abstract class Recarga
    {
        public abstract void init();
        public abstract void render(float elapsedTime);
        public abstract Boolean verificarColision(Camara camara);
        public abstract void recarga();
    }
}

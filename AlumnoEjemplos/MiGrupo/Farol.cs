using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class Farol : TipoIluminador
    {
        override public void mover(float elapsedTime)
        {

        }

        override public void init()
        {
            
        }
        override public void render()
        {
            
        }
        override public void actualizarEscenario(TgcScene escena, Camara camara)
        {

        }
        override public void CambiarEstadoLuz()
        {
            this.Encendida = !this.Encendida;
        }
    }

}

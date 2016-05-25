using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils._2D;
using System.IO;
using System.Collections;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class Sonidos
    {
        //TgcStaticSound pasoDerecho;
        //TgcStaticSound pasoIzquierdo;
        TgcStaticSound respiro;
        bool tipoPaso = true;
        string respiracionMerlusa;
        string respiracionPersecucion;
        public Sonidos()
        {
            init();
            Device d3dDevice = GuiController.Instance.D3dDevice;
        }
        public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            respiracionMerlusa = alumnoMediaFolder + "CucarachaJugosita\\Media\\respiracion1.wav";
            respiracionPersecucion = alumnoMediaFolder + "CucarachaJugosita\\Media\\respiracion2.wav";
            respiro = new TgcStaticSound();
            respiro.loadSound(respiracionMerlusa);
        }
        public void playMerlusa()
        {
            respiro.play(true);
        }
        public void stopMerlusa()
        {
            respiro.stop();
        }


    }

}

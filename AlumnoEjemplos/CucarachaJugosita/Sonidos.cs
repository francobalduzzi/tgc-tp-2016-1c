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
        TgcStaticSound respiroPersecucion;
        TgcStaticSound monstruo;
        bool tipoPaso = true;
        string respiracionMerlusa;
        string respiracionPersecucion;
        string dirMonstruo;
        float contador;
        public Sonidos()
        {
            init();
            contador = 300f;
            Device d3dDevice = GuiController.Instance.D3dDevice;
        }
        public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            respiracionMerlusa = alumnoMediaFolder + "CucarachaJugosita\\Media\\juego felipe respiracion corta susto .wav";
            respiracionPersecucion = alumnoMediaFolder + "CucarachaJugosita\\Media\\respiracion_corriendo_16.wav";
            dirMonstruo = alumnoMediaFolder + "CucarachaJugosita\\Media\\monstruoAlgoEnojado_16.wav";
            respiro = new TgcStaticSound();
            respiro.loadSound(respiracionMerlusa);
            respiroPersecucion = new TgcStaticSound();
            respiroPersecucion.loadSound(respiracionPersecucion);
            monstruo = new TgcStaticSound();
            monstruo.loadSound(dirMonstruo);
            //monstruo.SoundBuffer.
        }
        public void playMerlusa()
        {
            respiro.play();
        }
        public void stopMerlusa()
        {
            respiro.stop();
        }
        public void playPersecucion()
        {
            respiroPersecucion.play();
        }
        public void stopPersecucion()
        {
            respiroPersecucion.stop();
        }
        public void sonidoMonstruo(float elapsedTime)
        {
            contador -= elapsedTime;
            if(contador <= 0)
            {
                contador = 300f;
                monstruo.play();
            }
        }

    }

}

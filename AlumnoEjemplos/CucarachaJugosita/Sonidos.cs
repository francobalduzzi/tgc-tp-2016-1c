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
        TgcStaticSound fondoSonido;
        TgcStaticSound respiracionCadaTanto;
        bool tipoPaso = true;
        string alumnoMediaFolder;
        string respiracionMerlusa;
        string respiracionPersecucion;
        string dirMonstruo;
        string dirRespCadaTanto;
        string fondo;
        float contador;
        float contador2;
        public Sonidos()
        {
            init();
            
            Device d3dDevice = GuiController.Instance.D3dDevice;
        }
        public void init()
        {
            contador = 180f;
            contador2 = 30f;
            alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            respiracionMerlusa = alumnoMediaFolder + "CucarachaJugosita\\Media\\juego felipe respiracion corta susto .wav";
            respiracionPersecucion = alumnoMediaFolder + "CucarachaJugosita\\Media\\respiracion_corriendo_16.wav";
            dirMonstruo = alumnoMediaFolder + "CucarachaJugosita\\Media\\monstruo_lamento.wav";
            fondo = alumnoMediaFolder + "CucarachaJugosita\\Media\\Juego felipe sonido ambiente terror.wav";
            dirRespCadaTanto = alumnoMediaFolder + "CucarachaJugosita\\Media\\juego felipe respiracion normal cada tanto.wav";
            respiro = new TgcStaticSound();
            respiro.loadSound(respiracionMerlusa);
            respiroPersecucion = new TgcStaticSound();
            respiroPersecucion.loadSound(respiracionPersecucion);
            monstruo = new TgcStaticSound();
            monstruo.loadSound(dirMonstruo);
            fondoSonido = new TgcStaticSound();
            fondoSonido.loadSound(fondo);
            respiracionCadaTanto = new TgcStaticSound();
            respiracionCadaTanto.loadSound(dirRespCadaTanto);
            //monstruo.SoundBuffer.
        }
        public void sonidoRespiracionCadaTanto(float elapsedTime)
        {
            contador2 -= elapsedTime;
            if (contador2 <= 0)
            {
                contador2 = 30f;
                respiracionCadaTanto.play();
            }
        }
        public void stopSonidoRespiracionCadaTanto()
        {
            respiracionCadaTanto.stop();
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
        public void playFondo()
        {
            fondoSonido.play(true);
        }
        public void stopFondo()
        {
            fondoSonido.stop();
        }
        public void sonidoMonstruo(float elapsedTime)
        {
            contador -= elapsedTime;
            if(contador <= 0)
            {
                contador = 180f;
                monstruo.play();
            }
        }
        public void stopSonidoMonstruo()
        {
            monstruo.stop();
        }
        public void dispose()
        {
            respiro.dispose();
            respiroPersecucion.dispose();
            monstruo.dispose();
            fondoSonido.dispose();
            respiracionCadaTanto.dispose();
        }

    }

}

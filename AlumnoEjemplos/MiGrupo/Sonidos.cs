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


namespace AlumnoEjemplos.MiGrupo
{
    class Sonidos
    {
        TgcStaticSound pasoDerecho;
        TgcStaticSound pasoIzquierdo;
        TgcStaticSound puerta;
        bool tipoPaso = true;
        public Sonidos()
        {
            init();
            Device d3dDevice = GuiController.Instance.D3dDevice;
        }
        public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            GuiController.Instance.Modifiers.addFile("WAV-FilePD", alumnoMediaFolder + "MiGrupo\\pisada calle dcha.wav", "WAVs|*.wav");
            GuiController.Instance.Modifiers.addBoolean("PlayLoopPD", "Play LoopPD", false);
            GuiController.Instance.Modifiers.addFile("WAV-FilePI", alumnoMediaFolder + "MiGrupo\\pisada calle izda.wav", "WAVs|*.wav");
            GuiController.Instance.Modifiers.addBoolean("PlayLoopPI", "Play LoopPI", false);
            GuiController.Instance.Modifiers.addFile("WAV-FilePu", alumnoMediaFolder + "MiGrupo\\puerta ruidosa,abrir.wav", "WAVs|*.wav");
            GuiController.Instance.Modifiers.addBoolean("PlayLoopPu", "Play LoopPu", false);
        }
        public void play()
        {
            
            if ((GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W) || GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A) || GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D) || GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.S)) && (tipoPaso==true))
            {
                 Device d3dDevice = GuiController.Instance.D3dDevice;
                 string filePath = (string)GuiController.Instance.Modifiers["WAV-FilePD"];
                 pasoDerecho = new TgcStaticSound();
                 pasoDerecho.loadSound(filePath);
                 bool playLoopPD = (bool)GuiController.Instance.Modifiers["PlayLoopPD"];
                 pasoDerecho.play(playLoopPD);
            }
            if ((GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W) || GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A) || GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D) || GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.S)) && (tipoPaso == false))
            {
                Device d3dDevice = GuiController.Instance.D3dDevice;
                string filePath = (string)GuiController.Instance.Modifiers["WAV-FilePI"];
                pasoIzquierdo = new TgcStaticSound();
                pasoIzquierdo.loadSound(filePath);
                bool playLoopPI = (bool)GuiController.Instance.Modifiers["PlayLoopPI"];
                pasoIzquierdo.play(playLoopPI);
                
            }
            tipoPaso = !tipoPaso;
        }

    }
    
}

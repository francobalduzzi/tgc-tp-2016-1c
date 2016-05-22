using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MiGrupo
{
    class Barra
    {
        TgcSprite sprite;
        TgcSprite spriteAmarillo;


        public Barra()
        {
            init();
        }
        public void init()
        {
            //Device d3dDevice = GuiController.Instance.D3dDevice;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;

            //Crear Sprite
            sprite = new TgcSprite();
            spriteAmarillo = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(alumnoMediaFolder + "CucarachaJugosita\\Media\\barraRoja.png");
            spriteAmarillo.Texture = TgcTexture.createTexture(alumnoMediaFolder + "CucarachaJugosita\\Media\\barraAmarilla.png");




            sprite.Scaling = new Vector2(0.1f,0.05f);
            spriteAmarillo.Scaling = new Vector2(0.1f, 0.05f);

            sprite.Position = new Vector2(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth* 4/5, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight *  9/10);
            spriteAmarillo.Position = new Vector2(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth * 4 / 5, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight * 9 / 10);


        }
        public void render(float cargaLinterna)
        {
            float elapsedTime = GuiController.Instance.ElapsedTime;
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            GuiController.Instance.Drawer2D.beginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            sprite.render();
            if(cargaLinterna>0.02)
            {
                spriteAmarillo.Scaling = new Vector2(0.1f * cargaLinterna, 0.05f);
                spriteAmarillo.render();
            }

            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();

        }
    }
}

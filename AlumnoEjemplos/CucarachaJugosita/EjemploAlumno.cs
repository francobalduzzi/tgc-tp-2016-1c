using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcSkeletalAnimation;
using System.Collections;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils;
using AlumnoEjemplos.CucarachaJugosita;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        TgcScene escena;
        Camara camara;
        Vela vela1;
        Linterna linterna;
        Farol farol;
        TipoIluminador objeto; //Este sera el objeto que tenga en la mano el jugador
        Enemigo enemigo; // Uno solo para las pruebas dsps abra que hacer una lista. Hay que pasarla las coordenadas para que pueda arrancar y el estado. Camino ida necesita minimo 1 parametro aunque se quede quieto
        Enemigo enemigo2;
        LinternaRecarga recarga;
        VelaRecarga recargaVela;
        FarolRecarga recargaFarol;
        Puerta puerta1;
        Puerta puerta2;
        Sonidos pasos;
        float contador = 0;
        Barra barra;
        ArrayList listaEnemigos;
        ArrayList listaPuertas;
        ArrayList listaRecargas;
        ArrayList listaElementoMapa;
        ElementoMapa esqueleto;
        ElementoMapa antorcha1;
        ArrayList listaEscondites;
        Escondite escondite1;
        VertexBuffer screenQuadVB;
        Texture renderTarget2D;
        Surface pOldRT;
        Effect effect;
        TgcSprite menu;
        TgcSprite ganado;
        TgcSprite objetivo;
        TgcSprite manual;
        TgcSprite gameOver;
        float time = 0;

        /// <summary>
        /// Categor�a a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el �rbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "CucarachaJugosita";
        }

        /// <summary>
        /// Completar con la descripci�n del TP
        /// </summary>
        public override string getDescription()
        {
            return "Survival horror - Amnesia";
        }

        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            
            cargarShaderPostProcesado();
            camara = new Camara();
            camara.setCamera(new Vector3(359f, 60f, 1000f), new Vector3(289f, 30f, 90f));
            camara.MovementSpeed = 200f;
            camara.RotationSpeed = 5f;
            camara.JumpSpeed = 80f;
            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas

            //Carpeta de archivos Media del alumno

            cargarImagenes2D();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcSceneLoader loader = new TgcSceneLoader();
            escena = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\mapaFinal-TgcScene.xml");
            foreach (TgcMesh mesh in escena.Meshes) //escalamos el mapa
            {
                mesh.Scale = new Vector3(0.3f, 0.3f, 0.3f);
            }
            vela1 = new Vela();
            vela1.init();
            camara.SetEscena(escena);//cargamos la escena en la camara para que detecte colisiones

            //Cargamos la linterna
            linterna = new Linterna(camara.getLookAt(), camara.getPosition());
            farol = new Farol();
            farol.init();
            objeto = farol; //Empieza con la linterna en mano.
            //vela.Meshes[0].Rotation = new Vector3(-5f, -14f, 0f);
            //Camara en primera persona:
            Vector3 mira = new Vector3(0,0,0);
            Vector3 vector = new Vector3(0,0,20);
            listaPuertas = new ArrayList();
            puerta1 = new Puerta();
            puerta1.init(new Vector3(260f, 57f, 770f));
            puerta2 = new Puerta();
            puerta2.init(new Vector3(1400f, 57f, 1363f));
            puerta2.escalar(new Vector3(1.3f, 1f, 1f));

            //A�adimos puertas a la lista
            listaPuertas.Add(puerta1);
            listaPuertas.Add(puerta2);

            foreach (Puerta puerta in listaPuertas) //Aca a�adimos los meshes correspondiente a cada puerta a la escena
            {
                escena.Meshes.Add(puerta.getMeshC1());
                escena.Meshes.Add(puerta.getMeshC3());
                escena.Meshes.Add(puerta.getMeshC2());
                escena.Meshes.Add(puerta.getMeshP());
            }
            listaEnemigos = new ArrayList(); //Creamos lista de enemigos
            enemigo = new Enemigo(); //Cargamos un enemigo  Toda la carga de enemigos se hace en una funcion cargar enemigos, etc
            enemigo.setEscena(escena);
            enemigo.getCaminoOriginal().SetValue(new Vector3(718.5f, 5.02f, 1493.4f), 0);
            enemigo.getCaminoOriginal().SetValue(new Vector3(1360.2f, 5.02f, 1493.4f), 1);
            enemigo.getCaminoOriginal().SetValue(new Vector3(1360.2f, 5.02f, 503.2f), 2);
            enemigo.setCantidadWP(3);
            enemigo.setEscena(escena);
            enemigo.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo.init();
            enemigo.setCamara(camara);


            esqueleto = new ElementoMapa("Esqueleto-TgcScene.xml", new Vector3 (359f, 0f, 940f));
            esqueleto.rotateY(3.14f);

            antorcha1 = new ElementoMapa("Antorcha-TgcScene.xml", new Vector3(359f, 80f, 925f));
            antorcha1.rotateY(3.14f);

            /*enemigo2 = new Enemigo2(); //Cargamos un enemigo
            enemigo2.setEscena(escena);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(227.5f, 5.02f, 861.67f), 0);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(231.5f, 5.02f, 708.3f), 1);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(118.4f, 5.02f, 700f), 2);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(123.4f, 5.02f, 290.15f), 3);
            enemigo2.setCantidadWP(4);
            enemigo2.setEscena(escena);
            enemigo2.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo2.init();*/

            enemigo2 = new Enemigo2(); //Cargamos un enemigo
            enemigo2.setEscena(escena);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(965f, 5.02f, 842f), 0);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(931f, 5.02f, 835f), 1);;
            enemigo2.setCantidadWP(2);
            enemigo2.setEscena(escena);
            enemigo2.setEstado(Enemigo.Estado.Parado);
            enemigo2.init();
            enemigo2.setCamara(camara);

            //A�adimos enemigos a la lista
            listaEnemigos.Add(enemigo);
            listaEnemigos.Add(enemigo2); //Cargamos los enemigos

            recargaVela = new VelaRecarga(new Vector3(359f, 7f, 964f), vela1);
            recargaFarol = new FarolRecarga(new Vector3(379f,2f,964f), farol);
            recarga = new LinternaRecarga(new Vector3(457f, 5f, 964f), linterna);// se carga la/s recarga con la posicion

            pasos = new Sonidos();
            barra = new Barra();

            //A�adimos los escondite a la lista
            listaEscondites = new ArrayList();
            escondite1 = new Escondite();
            escondite1.init(new Vector3(1249f, 5.02f, 1211f));
            listaEscondites.Add(escondite1);

            //A�adimos recargas a la lista
            listaRecargas = new ArrayList();
            listaRecargas.Add(recargaFarol);
            listaRecargas.Add(recargaVela);
            listaRecargas.Add(recarga);

            //A�adimos elementos del mapa
            listaElementoMapa = new ArrayList();
            listaElementoMapa.Add(antorcha1);
            listaElementoMapa.Add(esqueleto);
            ///////////////USER VARS//////////////////

            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("variablePrueba");
            GuiController.Instance.UserVars.addVar("PosCam");
            GuiController.Instance.UserVars.addVar("contador");

            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("variablePrueba", 5451);
            GuiController.Instance.UserVars.setValue("PosCam", camara.getPosition());


            ///////////////MODIFIERS//////////////////

            //Crear un modifier para un valor FLOAT
            GuiController.Instance.Modifiers.addFloat("valorFloat", -50f, 200f, 0f);
            float angle = FastMath.TWO_PI;
            GuiController.Instance.Modifiers.addVertex3f("rotation", new Vector3(-angle, -angle, -angle), new Vector3(angle, angle, angle), new Vector3(0, 0, 0));

            //Crear un modifier para un ComboBox con opciones
            string[] opciones = new string[]{"opcion1", "opcion2", "opcion3"};
            GuiController.Instance.Modifiers.addInterval("valorIntervalo", opciones, 0);

            //Crear un modifier para modificar un v�rtice
            GuiController.Instance.Modifiers.addVertex3f("valorVertice", new Vector3(-100, -100, -100), new Vector3(50, 50, 50), new Vector3(0, 0, 0));


            /*
            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            GuiController.Instance.FpsCamera.Enable = true;
            //Configurar posicion y hacia donde se mira
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));
            */



            ///////////////LISTAS EN C#//////////////////
            //crear
            List<string> lista = new List<string>();

            //agregar elementos
            lista.Add("elemento1");
            lista.Add("elemento2");

            //obtener elementos
            string elemento1 = lista[0];

            //bucle foreach
            foreach (string elemento in lista)
            {
                //Loggear por consola del Framework
                GuiController.Instance.Logger.log(elemento);
            }

            //bucle for
            for (int i = 0; i < lista.Count; i++)
            {
                string element = lista[i];
            }
        }



        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            //Device de DirectX para renderizar
            while(!GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.J))
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();
                menu.render();
                GuiController.Instance.Drawer2D.endDrawSprite();
            }

            Device d3dDevice = GuiController.Instance.D3dDevice;
            objeto.actualizarEscenario(escena, camara); // Atencion aca, esto es como moo de prueba baja mucho ls FPS, lo ideal seria tener ls meshes cocinados y en el init del programa estos se carguen a cada uno de los objetos
            //Obtener valor de UserVar (hay que castear)
            int valor = (int)GuiController.Instance.UserVars.getValue("variablePrueba");
           
            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia

            //Capturar Input teclado 
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.F))
            {
                objeto.CambiarEstadoLuz();
            }
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.D1))
            {
                objeto.Encendida = false;
                objeto = linterna;
                objeto.Encendida = true;

            }
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.D2))
            {
                objeto.Encendida = false;
                objeto = vela1;
                objeto.Encendida = true;
            }
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.D3))
            {
                objeto.Encendida = false;
                objeto = farol;
                objeto.Encendida = true;
            }
            //Capturar Input Mouse
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Boton izq apretado
            }
            postProcesado(elapsedTime, d3dDevice);
            
        }

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {

        }


        public void renderTotal(float elapsedTime)
        {
            //enemigo.seguirA(camara.getPosition(), elapsedTime);
            moverCamaraConVela(elapsedTime); //Actualizamos el valor de la camara y vemos si generar efecto de vela
            // vela1.render();
            objeto.render();
            escena.renderAll();
            colisionesConPuerta();
            renderPuertas();
            verificarRegargas();
            renderRecargas(elapsedTime);
            objeto.bajarIntensidad(elapsedTime);// bajo la intensidad
            verificarSonidos(elapsedTime);
            // barra.render(linterna.damePorcentaje());
            GuiController.Instance.UserVars.setValue("PosCam", camara.getPosition()); //Actualizamos la user var, nos va a servir
            renderEscondites();
            colisionesConEscondites();
            renderEnemigos(camara.getPosition());
            renderElementosMapa();
        }
        public void cargarImagenes2D()
        {
            /*TgcSprite menu;
        TgcSprite ganado;
        TgcSprite objetivo;
        TgcSprite manual;
        TgcSprite gameOver;*/
            menu = new TgcSprite();
            ganado = new TgcSprite();
            objetivo = new TgcSprite();
            manual = new TgcSprite();
            gameOver = new TgcSprite();
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            menu.Texture = TgcTexture.createTexture(alumnoMediaFolder + "CucarachaJugosita\\Media\\Menu.jpg");
            ganado.Texture = TgcTexture.createTexture(alumnoMediaFolder + "CucarachaJugosita\\Media\\Ganado.jpg");
            objetivo.Texture = TgcTexture.createTexture(alumnoMediaFolder + "CucarachaJugosita\\Media\\Objetivo.jpg");
            manual.Texture = TgcTexture.createTexture(alumnoMediaFolder + "CucarachaJugosita\\Media\\Manual.jpg");
            gameOver.Texture = TgcTexture.createTexture(alumnoMediaFolder + "CucarachaJugosita\\Media\\GameOver.jpg");
            
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = menu.Texture.Size;
            menu.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            //Ubicarlo centrado en la pantalla
            menu.Position = new Vector2(0, 0);
            menu.Scaling = new Vector2((float)screenSize.Width / textureSize.Width, (float)screenSize.Height / textureSize.Height);
        }
        public void renderRecargas(float elapsedTime)
        {
            foreach(Recarga recarga in listaRecargas)
            {
                recarga.render(elapsedTime);
            }
        }
        public void verificarRegargas()
        {
            foreach(Recarga recarga in listaRecargas)
            {
                if (recarga.verificarColision(camara))
                {
                    recarga.recarga();
                }
            }
        }
        public void renderElementosMapa()
        {
            foreach(ElementoMapa elemento in listaElementoMapa)
            {
                elemento.render();
            }
        }
        public void verificarSonidos(float elapsedTime)
        {
            contador = contador + ((1) * elapsedTime);
            if (contador >= (45 * elapsedTime))
            {
                pasos.play();
                contador = 0;
            }
        }
        public void renderPuertas()
        {
            foreach(Puerta puerta in listaPuertas)
            {
                puerta.render();
            }
        }
        public void renderEscondites()
        {
            foreach(Escondite escondite in listaEscondites)
            {
                escondite.render();
            }
        }
        public void renderEnemigos(Vector3 posCam)
        {
            foreach(Enemigo enemigo in listaEnemigos)
            {
                enemigo.render(posCam);
            }
        }

        public void postProcesado(float elapsedTime, Device d3dDevice)
        {
            int contador = 0;
            foreach (Enemigo enemigo in listaEnemigos)
            {
                if(enemigo.getEstado() == Enemigo.Estado.Persiguiendo)
                {
                    contador++;
                }               
            }
            if (contador > 0)
            {
                GuiController.Instance.CustomRenderEnabled = true;
                time += elapsedTime;
                effect.SetValue("time", time);
                pOldRT = d3dDevice.GetRenderTarget(0);
                Surface pSurf = renderTarget2D.GetSurfaceLevel(0);
                d3dDevice.SetRenderTarget(0, pSurf);
                d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);


                //Dibujamos la escena comun, pero en vez de a la pantalla al Render Target
                drawSceneToRenderTarget(d3dDevice, elapsedTime);

                //Liberar memoria de surface de Render Target
                pSurf.Dispose();

                //Ahora volvemos a restaurar el Render Target original (osea dibujar a la pantalla)
                d3dDevice.SetRenderTarget(0, pOldRT);


                //Luego tomamos lo dibujado antes y lo combinamos con una textura con efecto de alarma
                drawPostProcess(d3dDevice);
            }
            else
            {
                renderTotal(elapsedTime);
            }
        }

        private void drawSceneToRenderTarget(Device d3dDevice, float elapsedTime)
        {
            //Arrancamos el renderizado. Esto lo tenemos que hacer nosotros a mano porque estamos en modo CustomRenderEnabled = true
            //d3dDevice.BeginScene();


            //Como estamos en modo CustomRenderEnabled, tenemos que dibujar todo nosotros, incluso el contador de FPS
            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);

            //Tambien hay que dibujar el indicador de los ejes cartesianos
            GuiController.Instance.AxisLines.render();

            //Dibujamos todos los meshes del escenario
            renderTotal(elapsedTime);
            //Terminamos manualmente el renderizado de esta escena. Esto manda todo a dibujar al GPU al Render Target que cargamos antes
            //d3dDevice.EndScene();
        }
        private void drawPostProcess(Device d3dDevice)
        {
            //Arrancamos la escena
            //d3dDevice.BeginScene();

            //Cargamos para renderizar el unico modelo que tenemos, un Quad que ocupa toda la pantalla, con la textura de todo lo dibujado antes
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, screenQuadVB, 0);

            //Ver si el efecto de oscurecer esta activado, configurar Technique del shader segun corresponda
                effect.Technique = "BlurTechnique";

            //Cargamos parametros en el shader de Post-Procesado
            effect.SetValue("render_target2D", renderTarget2D);
            effect.SetValue("blur_intensity", 0.015f);


            //Limiamos la pantalla y ejecutamos el render del shader
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();

            //Terminamos el renderizado de la escena
           // d3dDevice.EndScene();
        }


        public void colisionesConPuerta()
        {
            foreach(Puerta puerta in listaPuertas)
            {
                if (puerta.verificarColision(camara))
                {
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.E))
                    {
                        if (puerta.getEstado() == Puerta.Estado.Cerrado)
                        {
                            puerta.seAbrioJugador();
                            camara.bloqueada();
                        }
                        else
                        {
                            puerta.seCerroJugador();
                            camara.bloqueada();
                        }
                    }
                }
                
                
                    foreach(Enemigo enemigo in listaEnemigos)
                    {
                        if (puerta.verificarColision(enemigo))
                        {
                            puerta.seAbrio();
                            enemigo.bloqueado();
                        }
                    }
                
            }
        }

        public void cargarShaderPostProcesado()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            CustomVertex.PositionTextured[] screenQuadVertices = new CustomVertex.PositionTextured[]
            {
                new CustomVertex.PositionTextured( -1, 1, 1, 0,0),
                new CustomVertex.PositionTextured(1,  1, 1, 1,0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0,1),
                new CustomVertex.PositionTextured(1,-1, 1, 1,1)
            };
            //vertex buffer de los triangulos
            screenQuadVB = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                    4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                        CustomVertex.PositionTextured.Format, Pool.Default);
            screenQuadVB.SetData(screenQuadVertices, 0, LockFlags.None);

            //Creamos un Render Targer sobre el cual se va a dibujar la pantalla
            renderTarget2D = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);


            //Cargar shader con efectos de Post-Procesado
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            effect = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\blurPersecucion.fx");

            //Configurar Technique dentro del shader
            effect.Technique = "BlurTechnique";

        }
        public void colisionesConEscondites()
        {
            foreach(Escondite escondite in listaEscondites)
            {
                if (escondite.verificarColision(camara))
                {
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.E))
                    {
                        if (camara.getEscondido())
                        {
                            camara.desesconder();
                        }
                        else
                        {
                            escondite.esconder(camara);
                        }
                        
                    }
                }
            }
        }
        public void moverCamaraConVela(float elapsedTime)
        {
            Vector3 Aux = camara.getPosition();
            camara.updateCamera();
            if(camara.getPosition() != Aux)
            {
                objeto.mover(elapsedTime);
            }
        }
    }
}

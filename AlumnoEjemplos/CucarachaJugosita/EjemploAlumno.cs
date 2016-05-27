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
using System.Windows.Forms;

namespace AlumnoEjemplos.CucarachaJugosita
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
        EnemigoAnimacion enemigoAnimado;
        LinternaRecarga recarga;
        VelaRecarga recargaVela;
        FarolRecarga recargaFarol;
        Llave llave;
        Puerta puerta1;
        Puerta puerta2;
        Puerta puerta3;
        Puerta puerta4;
        Puerta puerta5;
        Sonidos sonidos;
        float contador = 0;
        Barra barra;
        ArrayList listaEnemigos;
        ArrayList listaPuertas;
        ArrayList listaRecargas;
        ArrayList listaElementoMapa;
        ArrayList listaLlaves;
        ArrayList todosElementosARenderizar; //Hay que separar en 2 xq los enemigos son skeletical
        ArrayList enemigosARenderizar;
        List<LuzNormal> listaLuces;
        ElementoMapa esqueleto;
        ElementoMapa antorcha1;
        ArrayList listaEscondites;
        Escondite escondite1;
        VertexBuffer screenQuadVB;
        Texture renderTarget2D;
        Texture g_pGlowMap;
        Texture g_pRenderTarget4;
        Texture g_pRenderTarget4Aux;
        Surface pOldRT;
        Surface g_pDepthStencil;     // Depth-stencil buffer
        Effect effect;
        Effect efectoVictoria;
        Effect efectoMerlusa;
        Effect efectoNightVision;
        TgcSprite menu;
        TgcSprite ganado;
        TgcSprite objetivo;
        TgcSprite manual;
        TgcSprite gameOver;
        EstadoMenu estadoMenu;
        ManejoIluminacion manejoI;
        NumerosLlaves numeroLLaves;
        Trofeo trofeo;
        float time = 0;
        public float timeMerlusa = 0f; // Vuevle a 0 cada vez que se le termina la merlusa
        Boolean nightVision = false;
        float contadorNight =  30f;
        TgcMesh meshInservible;
        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
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
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Survival horror - Amnesia";
        }

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {

            cargarShaderPostProcesado();
            camara = new Camara();
            camara.setCamera(new Vector3(60f, 60f, 183f), new Vector3(289f, 30f, 90f));
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
            escena = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\mapitaFinal1-TgcScene.xml");
            foreach (TgcMesh mesh in escena.Meshes) //escalamos el mapa
            {
                mesh.Scale = new Vector3(5f, 5f, 5f);
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
            Vector3 mira = new Vector3(0, 0, 0);
            Vector3 vector = new Vector3(0, 0, 20);
            listaPuertas = new ArrayList();
            puerta1 = new Puerta();
            puerta1.init(new Vector3(960f, 57f, 396f));
            puerta1.rotateY(-1.57f);
            puerta2 = new Puerta();
            puerta2.init(new Vector3(1699f, 45f, 855f));
            puerta2.rotateY(-3.14f);
            puerta2.escalar(new Vector3(0.8f, 0.8f, 0.8f));
            puerta3 = new Puerta();
            puerta3.init(new Vector3(2457f, 57f, 1130f));
            puerta3.rotateY(-1.57f);
            puerta4 = new Puerta();
            puerta4.init(new Vector3(3193f, 57f, 1300f));
            puerta4.rotateY(-3.14f);
            puerta5 = new Puerta();
            puerta5.init(new Vector3(1425f, 57f, 1363f));
            puerta5.rotateY(-3.14f);

            //Añadimos puertas a la lista
            listaPuertas.Add(puerta1);
            listaPuertas.Add(puerta2);
            listaPuertas.Add(puerta3);
            listaPuertas.Add(puerta4);
            listaPuertas.Add(puerta5);

            foreach (Puerta puerta in listaPuertas) //Aca añadimos los meshes correspondiente a cada puerta a la escena
            {
                escena.Meshes.Add(puerta.getMeshC1());
                escena.Meshes.Add(puerta.getMeshC3());
                escena.Meshes.Add(puerta.getMeshC2());
                escena.Meshes.Add(puerta.getMeshP());
            }
            listaEnemigos = new ArrayList(); //Creamos lista de enemigos
            enemigo = new Enemigo(); //Cargamos un enemigo  Toda la carga de enemigos se hace en una funcion cargar enemigos, etc
            enemigo.setEscena(escena);
            enemigo.getCaminoOriginal().SetValue(new Vector3(1454f, 5.02f, 1490f), 0);
            enemigo.getCaminoOriginal().SetValue(new Vector3(1451f, 5.02f, 1273f), 1);
            //enemigo.getCaminoOriginal().SetValue(new Vector3(1360.2f, 5.02f, 503.2f), 2);
            enemigo.setCantidadWP(2);
            enemigo.setEscena(escena);
            enemigo.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo.init();
            enemigo.setCamara(camara);


            esqueleto = new ElementoMapa("Esqueleto-TgcScene.xml", new Vector3(359f, 0f, 940f));
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
            enemigo2.getCaminoOriginal().SetValue(new Vector3(931f, 5.02f, 835f), 1); ;
            enemigo2.setCantidadWP(2);
            enemigo2.setEscena(escena);
            enemigo2.setEstado(Enemigo.Estado.Parado);
            enemigo2.init();
            enemigo2.setCamara(camara);

            enemigoAnimado = new EnemigoAnimacion(new Vector3(338f, 60f, 160f), new Vector3(10, 100, 400));
            enemigoAnimado.getCaminoOriginal().SetValue(new Vector3(672.27f, 5.02f, -0.13f), 0);
            enemigoAnimado.getCaminoOriginal().SetValue(new Vector3(679.675f, 5.02f, 448.91f), 1);
            enemigoAnimado.getCaminoOriginal().SetValue(new Vector3(431f, 5.02f, 406f), 2);
            enemigoAnimado.getCaminoOriginal().SetValue(new Vector3(412f, 5.02f, 948f), 3);
            enemigoAnimado.getCaminoOriginal().SetValue(new Vector3(305f, 5.02f, 953f), 4);
            enemigoAnimado.setCantidadWP(5);
            //enemigoAnimado.setEscena(escena);
            enemigoAnimado.setEstado(Enemigo.Estado.Parado);
            
            enemigoAnimado.init();
            //Añadimos enemigos a la lista
            listaEnemigos.Add(enemigoAnimado);
            listaEnemigos.Add(enemigo);
            listaEnemigos.Add(enemigo2); //Cargamos los enemigos
            enemigo2.getCaminoOriginal().SetValue(new Vector3(965f, 5.02f, 842f), 0);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(931f, 5.02f, 835f), 1);
            recargaVela = new VelaRecarga(new Vector3(359f, 7f, 964f), vela1);
            recargaFarol = new FarolRecarga(new Vector3(379f, 2f, 964f), farol);
            recarga = new LinternaRecarga(new Vector3(457f, 5f, 964f), linterna);// se carga la/s recarga con la posicion

            sonidos = new Sonidos();
            barra = new Barra();

            //Añadimos los escondite a la lista
            listaEscondites = new ArrayList();
            escondite1 = new Escondite();
            escondite1.init(new Vector3(1249f, 5.02f, 1211f));
            listaEscondites.Add(escondite1);

            //Añadimos recargas a la lista
            listaRecargas = new ArrayList();
            listaRecargas.Add(recargaFarol);
            listaRecargas.Add(recargaVela);
            listaRecargas.Add(recarga);

            //Añadimos llaves al mapa
            llave = new Llave(new Vector3(570f, 50f, 1030f));

            //Añadimos llaves a la lista
            listaLlaves = new ArrayList();
            listaLlaves.Add(llave);
            numeroLLaves = new NumerosLlaves();
            numeroLLaves.setNumeroLLaves(listaLlaves.Count);

            //Añado el unico trofeo
            trofeo = new Trofeo(new Vector3(4373f, 10f, 1609f));

            //Añadimos elementos del mapa
            listaElementoMapa = new ArrayList();
            listaElementoMapa.Add(antorcha1);
            listaElementoMapa.Add(esqueleto);

            manejoI = new ManejoIluminacion();
            manejoI.setEscena(escena);

            listaLuces = new List<LuzNormal>();

            LuzNormal luz2 = new LuzNormal();
            luz2.Intensity = 20;
            luz2.lightColor = Color.White;
            luz2.Posicion = new Vector3(1948, 78, 697);
            luz2.Attenuation = 0.3f;
            listaLuces.Add(luz2);
            LuzTitilante luz = new LuzTitilante();
            luz.Intensity = 20;
            luz.lightColor = Color.Red;
            luz.Posicion = new Vector3(750, 120, 1188);
            luz.Attenuation = 0.3f;
            listaLuces.Add(luz);
            LuzNormal luz3 = new LuzNormal();
            luz3.Intensity = 20;
            luz3.lightColor = Color.White;
            luz3.Posicion = new Vector3(2542, 110, 97);
            luz3.Attenuation = 0.3f;
            listaLuces.Add(luz3);
            manejoI.setListaLuces(listaLuces);
            camara.setEnemigos(listaEnemigos);

            GuiController.Instance.FullScreenEnable = false;


            //Hacer que el Listener del sonido 3D siga al personaje
            TgcScene escena2;
            escena2 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\mapitaFinal1-TgcScene.xml"); // Esto es una meirda que no sirve para nada, solo xq sonido3d trabajo con mehshes y no con vectores
            meshInservible = escena2.Meshes[0];
            meshInservible.Position = camara.getPosition();
            GuiController.Instance.DirectSound.ListenerTracking = meshInservible;


            //Aca vamos a cargar todos los elementos a renderizar en una lista generica -- Para la iluminacion
            todosElementosARenderizar = new ArrayList();
            enemigosARenderizar = new ArrayList();
            foreach(TgcMesh mesh in escena.Meshes)  //En la escena ya estan cargadas los de las puertas
            {
                todosElementosARenderizar.Add(mesh);
            }
            foreach(Enemigo enemigo in listaEnemigos)
            {
                enemigosARenderizar.Add(enemigo.getMesh());
            }
            foreach(Escondite escondite in listaEscondites)
            {
                todosElementosARenderizar.Add(escondite.getMesh());
            }
            foreach(Llave llave in listaLlaves)
            {
                todosElementosARenderizar.Add(llave.getMesh());
            }
            foreach(ElementoMapa elemento in listaElementoMapa)
            {
                todosElementosARenderizar.Add(elemento.mesh);
            }
            foreach(Recarga recarga in listaRecargas)
            {
                todosElementosARenderizar.Add(recarga.getMesh());
            }
            manejoI.setTodosLosElementos(todosElementosARenderizar);
            manejoI.setEnemigosARenderizar(enemigosARenderizar);
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
            string[] opciones = new string[] { "opcion1", "opcion2", "opcion3" };
            GuiController.Instance.Modifiers.addInterval("valorIntervalo", opciones, 0);

            //Crear un modifier para modificar un vértice
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
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            switch (estadoMenu)
            {
                case EstadoMenu.Menu:
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    menu.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.J))
                    {
                        estadoMenu = EstadoMenu.Juego;
                    }
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.M))
                    {
                        estadoMenu = EstadoMenu.Manual;
                    }
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.O))
                    {
                        estadoMenu = EstadoMenu.Objetivo;
                    }
                    break;
                case EstadoMenu.Manual:
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    manual.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.V))
                    {
                        estadoMenu = EstadoMenu.Menu;
                    }
                    break;
                case EstadoMenu.Objetivo:
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    objetivo.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.V))
                    {
                        estadoMenu = EstadoMenu.Menu;
                    }
                    break;
                case EstadoMenu.Juego:
                    Device d3dDevice = GuiController.Instance.D3dDevice;

                    //objeto.actualizarEscenario(escena, camara); // Atencion aca, esto es como moo de prueba baja mucho ls FPS, lo ideal seria tener ls meshes cocinados y en el init del programa estos se carguen a cada uno de los objetos
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
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.N))
                    {
                        nightVision = true;
                    }
                    //Capturar Input Mouse
                    if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
                    {
                        //Boton izq apretado
                    }
                    postProcesado(elapsedTime, d3dDevice);
                    break;
            }



        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
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
            manejoI.iluminar(objeto, camara);
            colisionesConPuerta();
            renderPuertas();
            verificarRegargas();
            renderRecargas(elapsedTime);
            objeto.bajarIntensidad(elapsedTime);// bajo la intensidad
            // barra.render(linterna.damePorcentaje());
            GuiController.Instance.UserVars.setValue("PosCam", camara.getPosition()); //Actualizamos la user var, nos va a servir
            renderEscondites();
            colisionesConEscondites();
            renderEnemigos(camara.getPosition()); //saco el render para poder investigar bien el mapa
            renderElementosMapa();
            verificarLlaves();
            renderLlaves(elapsedTime);
            numeroLLaves.render();
            renderTrofeo(elapsedTime);
            meshInservible.Position = camara.getPosition();
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
            textureSize = ganado.Texture.Size;
            ganado.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            //Ubicarlo centrado en la pantalla
            ganado.Position = new Vector2(0, 0);
            ganado.Scaling = new Vector2((float)screenSize.Width / textureSize.Width, (float)screenSize.Height / textureSize.Height);
            textureSize = objetivo.Texture.Size;
            objetivo.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            //Ubicarlo centrado en la pantalla
            objetivo.Position = new Vector2(0, 0);
            objetivo.Scaling = new Vector2((float)screenSize.Width / textureSize.Width, (float)screenSize.Height / textureSize.Height);
            textureSize = manual.Texture.Size;
            manual.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            //Ubicarlo centrado en la pantalla
            manual.Position = new Vector2(0, 0);
            manual.Scaling = new Vector2((float)screenSize.Width / textureSize.Width, (float)screenSize.Height / textureSize.Height);
            textureSize = gameOver.Texture.Size;
            gameOver.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));
            //Ubicarlo centrado en la pantalla
            gameOver.Position = new Vector2(0, 0);
            gameOver.Scaling = new Vector2((float)screenSize.Width / textureSize.Width, (float)screenSize.Height / textureSize.Height);
            estadoMenu = EstadoMenu.Menu;

        }

        //Renders individuales de las listas de cosas
        #region
        public void renderTrofeo(float elapsedTime)
        {
            if (numeroLLaves.juntoTodas())
            {
                trofeo.verificarColision(camara);
            }
            trofeo.render(elapsedTime);
        }
        public void verificarLlaves()
        {
            foreach (Llave llave in listaLlaves)
            {
                if (llave.verificarColision(camara))
                {
                    numeroLLaves.recolectoLlave();
                }

            }
        }
        public void renderLlaves(float elapsedTime)
        {
            foreach (Llave llave in listaLlaves)
            {
                llave.render(elapsedTime);
            }
        }
        public void renderRecargas(float elapsedTime)
        {
            foreach (Recarga recarga in listaRecargas)
            {
                recarga.render(elapsedTime);
            }
        }
        public void verificarRegargas()
        {
            foreach (Recarga recarga in listaRecargas)
            {
                if (recarga.verificarColision(camara))
                {
                    recarga.recarga();
                }
            }
        }
        public void renderElementosMapa()
        {
            foreach (ElementoMapa elemento in listaElementoMapa)
            {
                elemento.render();
            }
        }
        public void renderPuertas()
        {
            foreach (Puerta puerta in listaPuertas)
            {
                puerta.render();
            }
        }
        public void renderEscondites()
        {
            foreach (Escondite escondite in listaEscondites)
            {
                escondite.render();
            }
        }
        public void renderEnemigos(Vector3 posCam)
        {
            foreach (Enemigo enemigo in listaEnemigos)
            {
                enemigo.render(posCam);
            }
        }
        #endregion
        //Fin render individuales de listas de cosas

        public void postProcesado(float elapsedTime, Device d3dDevice)
        {
            if (nightVision && contadorNight >0)
            {
                contadorNight -= elapsedTime;
                renderConEfectos(elapsedTime); // Efecto del nightVision
            }
            else
            {
                int contador = 0;
                Boolean merlusa = false;
                foreach (Enemigo enemigo in listaEnemigos)
                {
                    if (enemigo.getEstado() == Enemigo.Estado.Persiguiendo)
                    {
                        contador++;
                    }
                }
                if (contador > 0)
                {
                    sonidos.stopMerlusa();
                    efectoPostProcesadoPersecucion(elapsedTime, d3dDevice);
                }
                else
                {
                    merlusa = camara.activarEfectoMerlusa();
                    if (merlusa || timeMerlusa != 0)
                    {
                        sonidos.playMerlusa();
                        camara.efectoMerlusa(timeMerlusa);
                        efectoPostProcesadoMerlusa(elapsedTime, d3dDevice, merlusa);
                    }
                    else
                    {
                        sonidos.stopMerlusa();
                        timeMerlusa = 0;
                        efectoPostProcesadoVictoria(elapsedTime, d3dDevice); // Este es el render Generico que se hace siempre, podriamos separarlo en 2, para evitar hacer postProcesado innecesario
                    }

                    //renderTotal(elapsedTime); --->>> Comentamos xq se hace el render en el post procesado si abro esto se renderiza 2 veces y duplica tiempo
                }
            }
            
        }

        //Efectos de post procesado en persecucion
        #region
        public void efectoPostProcesadoPersecucion(float elapsedTime, Device d3dDevice)
        {
            GuiController.Instance.CustomRenderEnabled = true;
            time += elapsedTime;
            effect.SetValue("time", time);
            pOldRT = d3dDevice.GetRenderTarget(0);
            Surface pSurf = renderTarget2D.GetSurfaceLevel(0);
            d3dDevice.SetRenderTarget(0, pSurf);
            Surface pOldDS = d3dDevice.DepthStencilSurface;
            // Probar de comentar esta linea, para ver como se produce el fallo en el ztest
            // por no soportar usualmente el multisampling en el render to texture.
            d3dDevice.DepthStencilSurface = g_pDepthStencil;
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);


            //Dibujamos la escena comun, pero en vez de a la pantalla al Render Target
            drawSceneToRenderTargetPersecucion(d3dDevice, elapsedTime);

            //Liberar memoria de surface de Render Target
            pSurf.Dispose();

            //Ahora volvemos a restaurar el Render Target original (osea dibujar a la pantalla)
            d3dDevice.SetRenderTarget(0, pOldRT);
            d3dDevice.DepthStencilSurface = pOldDS;

            //Luego tomamos lo dibujado antes y lo combinamos con una textura con efecto de alarma
            drawPostProcessPersecucion(d3dDevice);
        }
        private void drawSceneToRenderTargetPersecucion(Device d3dDevice, float elapsedTime)
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
            // d3dDevice.EndScene();
        }
        private void drawPostProcessPersecucion(Device d3dDevice)
        {
            //Arrancamos la escena
            //d3dDevice.BeginScene();

            //Cargamos para renderizar el unico modelo que tenemos, un Quad que ocupa toda la pantalla, con la textura de todo lo dibujado antes
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, screenQuadVB, 0);


            //Cargamos parametros en el shader de Post-Procesado
            effect.SetValue("render_target2D", renderTarget2D);
            effect.SetValue("blur_intensity", 0.015f);
            /*efectoVictoria.SetValue("posCam", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
            efectoVictoria.SetValue("posicion", TgcParserUtils.vector3ToFloat4Array(posVictoria));
            efectoVictoria.SetValue("render_target2D", renderTarget2D);*/
            //Limiamos la pantalla y ejecutamos el render del shader
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            /*efectoVictoria.Begin(FX.None);
            efectoVictoria.BeginPass(0);*/
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();
            /*efectoVictoria.EndPass();
            efectoVictoria.End();*/

            //Terminamos el renderizado de la escena
            //d3dDevice.EndScene();
        }
        #endregion
        //Fin efectos post procesado persecucion

        // Efectos de post procesado difuminado victoria
        #region
        public void efectoPostProcesadoVictoria(float elapsedTime, Device d3dDevice)
        {
            GuiController.Instance.CustomRenderEnabled = true;
            time += elapsedTime;
            effect.SetValue("time", time);
            pOldRT = d3dDevice.GetRenderTarget(0);
            Surface pSurf = renderTarget2D.GetSurfaceLevel(0);
            d3dDevice.SetRenderTarget(0, pSurf);
            Surface pOldDS = d3dDevice.DepthStencilSurface;
            // Probar de comentar esta linea, para ver como se produce el fallo en el ztest
            // por no soportar usualmente el multisampling en el render to texture.
            d3dDevice.DepthStencilSurface = g_pDepthStencil;
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);


            //Dibujamos la escena comun, pero en vez de a la pantalla al Render Target
            drawSceneToRenderTargetVictoria(d3dDevice, elapsedTime);

            //Liberar memoria de surface de Render Target
            pSurf.Dispose();

            //Ahora volvemos a restaurar el Render Target original (osea dibujar a la pantalla)
            d3dDevice.SetRenderTarget(0, pOldRT);
            d3dDevice.DepthStencilSurface = pOldDS;

            //Luego tomamos lo dibujado antes y lo combinamos con una textura con efecto de alarma
            drawPostProcessVictoria(d3dDevice);
        }
        private void drawSceneToRenderTargetVictoria(Device d3dDevice, float elapsedTime)
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
            // d3dDevice.EndScene();
        }
        private void drawPostProcessVictoria(Device d3dDevice)
        {
            //Arrancamos la escena
            //d3dDevice.BeginScene();

            //Cargamos para renderizar el unico modelo que tenemos, un Quad que ocupa toda la pantalla, con la textura de todo lo dibujado antes
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, screenQuadVB, 0);


            //Cargamos parametros en el shader de Post-Procesado
            efectoVictoria.SetValue("posCam", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
            efectoVictoria.SetValue("posicion", TgcParserUtils.vector3ToFloat4Array(trofeo.posicion));
            efectoVictoria.SetValue("render_target2D", renderTarget2D);
            //Limiamos la pantalla y ejecutamos el render del shader
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            efectoVictoria.Begin(FX.None);
            efectoVictoria.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            efectoVictoria.EndPass();
            efectoVictoria.End();

            //Terminamos el renderizado de la escena
            //d3dDevice.EndScene();
        }
        #endregion
        //Fin efectos post procesado victoria

        //Efectos post procesado merlusa
        #region
        public void efectoPostProcesadoMerlusa(float elapsedTime, Device d3dDevice, Boolean merlusa)
        {
            GuiController.Instance.CustomRenderEnabled = true;
            if (merlusa)
            {
                timeMerlusa += elapsedTime;
            }
            else
            {
                timeMerlusa -= 3*elapsedTime;
                if(timeMerlusa <= 0.05f)
                {
                    timeMerlusa = 0;
                }
            }
            
            effect.SetValue("time", time);
            pOldRT = d3dDevice.GetRenderTarget(0);
            Surface pSurf = renderTarget2D.GetSurfaceLevel(0);
            d3dDevice.SetRenderTarget(0, pSurf);
            Surface pOldDS = d3dDevice.DepthStencilSurface;
            // Probar de comentar esta linea, para ver como se produce el fallo en el ztest
            // por no soportar usualmente el multisampling en el render to texture.
            d3dDevice.DepthStencilSurface = g_pDepthStencil;
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);


            //Dibujamos la escena comun, pero en vez de a la pantalla al Render Target
            drawSceneToRenderTargetMerlusa(d3dDevice, elapsedTime);

            //Liberar memoria de surface de Render Target
            pSurf.Dispose();

            //Ahora volvemos a restaurar el Render Target original (osea dibujar a la pantalla)
            d3dDevice.SetRenderTarget(0, pOldRT);
            d3dDevice.DepthStencilSurface = pOldDS;

            //Luego tomamos lo dibujado antes y lo combinamos con una textura con efecto de alarma
            drawPostProcessMerlusa(d3dDevice);
        }
        private void drawSceneToRenderTargetMerlusa(Device d3dDevice, float elapsedTime)
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
            // d3dDevice.EndScene();
        }
        private void drawPostProcessMerlusa(Device d3dDevice)
        {
            //Arrancamos la escena
            //d3dDevice.BeginScene();

            //Cargamos para renderizar el unico modelo que tenemos, un Quad que ocupa toda la pantalla, con la textura de todo lo dibujado antes
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, screenQuadVB, 0);


            //Cargamos parametros en el shader de Post-Procesado
            efectoMerlusa.SetValue("time", timeMerlusa);
            efectoMerlusa.SetValue("blur_intensity", 0.01f);
            efectoMerlusa.SetValue("render_target2D", renderTarget2D);
            //Limiamos la pantalla y ejecutamos el render del shader
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            efectoMerlusa.Begin(FX.None);
            efectoMerlusa.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            efectoMerlusa.EndPass();
            efectoMerlusa.End();

            //Terminamos el renderizado de la escena
            //d3dDevice.EndScene();
        }
        #endregion
        //Fin efectos post procesado merlusa
        //Inicio efectos nightVision
        #region
        public void renderConEfectos(float elapsedTime)
        {
            GuiController.Instance.CustomRenderEnabled = true;
            Device device = GuiController.Instance.D3dDevice;
            Control panel3d = GuiController.Instance.Panel3d;
            float aspectRatio = (float)panel3d.Width / (float)panel3d.Height;

            // dibujo la escena una textura 
            efectoNightVision.Technique = "DefaultTechnique";
            // guardo el Render target anterior y seteo la textura como render target
            Surface pOldRT = device.GetRenderTarget(0);
            Surface pSurf = renderTarget2D.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            // hago lo mismo con el depthbuffer, necesito el que no tiene multisampling
            Surface pOldDS = device.DepthStencilSurface;
            device.DepthStencilSurface = g_pDepthStencil;
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            //device.BeginScene();
            //Dibujamos todos los meshes del escenario
            renderScene(elapsedTime, "DefaultTechnique");

            //Render personames enemigos
            foreach (Enemigo enemigo in listaEnemigos)
                enemigo.getMesh().render();

            //device.EndScene();
            pSurf.Dispose();


            // dibujo el glow map
            efectoNightVision.Technique = "DefaultTechnique";
            pSurf = g_pGlowMap.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            //device.BeginScene();

            //Dibujamos SOLO los meshes que tienen glow brillantes
            //Render personaje brillante
            //Render personames enemigos
            foreach (Enemigo enemigo in listaEnemigos)
            {
                Ray rayo = new Ray(camara.getPosition(), enemigo.getMesh().Position);
                int contador = 0;
                foreach (TgcMesh mesh in escena.Meshes)
                {
                    if (rayo.intersectAABB(mesh.BoundingBox))
                    {
                        contador++;
                    }

                }
                if(contador == 0)
                {
                    enemigo.getMesh().render();
                }
                
            }
                

            // El resto opacos
            //renderScene(elapsedTime, "DibujarObjetosOscuros");

            //device.EndScene();
            pSurf.Dispose();

            // Hago un blur sobre el glow map
            // 1er pasada: downfilter x 4
            // -----------------------------------------------------
            pSurf = g_pRenderTarget4.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            //device.BeginScene();
            efectoNightVision.Technique = "DownFilter4";
            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, screenQuadVB, 0);
            efectoNightVision.SetValue("g_RenderTarget", g_pGlowMap);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            efectoNightVision.Begin(FX.None);
            efectoNightVision.BeginPass(0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            efectoNightVision.EndPass();
            efectoNightVision.End();
            pSurf.Dispose();
            //device.EndScene();
            device.DepthStencilSurface = pOldDS;

            // Pasadas de blur
            for (int P = 0; P < 3; ++P)
            {
                // Gaussian blur Horizontal
                // -----------------------------------------------------
                pSurf = g_pRenderTarget4Aux.GetSurfaceLevel(0);
                device.SetRenderTarget(0, pSurf);
                // dibujo el quad pp dicho :
                //device.BeginScene();
                efectoNightVision.Technique = "GaussianBlurSeparable";
                device.VertexFormat = CustomVertex.PositionTextured.Format;
                device.SetStreamSource(0, screenQuadVB, 0);
                efectoNightVision.SetValue("g_RenderTarget", g_pRenderTarget4);

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                efectoNightVision.Begin(FX.None);
                efectoNightVision.BeginPass(0);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                efectoNightVision.EndPass();
                efectoNightVision.End();
                pSurf.Dispose();
                //device.EndScene();

                pSurf = g_pRenderTarget4.GetSurfaceLevel(0);
                device.SetRenderTarget(0, pSurf);
                pSurf.Dispose();

                //  Gaussian blur Vertical
                // -----------------------------------------------------
                //device.BeginScene();
                efectoNightVision.Technique = "GaussianBlurSeparable";
                device.VertexFormat = CustomVertex.PositionTextured.Format;
                device.SetStreamSource(0, screenQuadVB, 0);
                efectoNightVision.SetValue("g_RenderTarget", g_pRenderTarget4Aux);

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                efectoNightVision.Begin(FX.None);
                efectoNightVision.BeginPass(1);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                efectoNightVision.EndPass();
                efectoNightVision.End();
                //device.EndScene();

            }


            //  To Gray Scale
            // -----------------------------------------------------
            // Ultima pasada vertical va sobre la pantalla pp dicha
            device.SetRenderTarget(0, pOldRT);
            //pSurf = g_pRenderTarget4Aux.GetSurfaceLevel(0);
            //device.SetRenderTarget(0, pSurf);

            //device.BeginScene();
            efectoNightVision.Technique = "GrayScale";
            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, screenQuadVB, 0);
            efectoNightVision.SetValue("g_RenderTarget", renderTarget2D);
            efectoNightVision.SetValue("g_GlowMap", g_pRenderTarget4Aux);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            efectoNightVision.Begin(FX.None);
            efectoNightVision.BeginPass(0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            efectoNightVision.EndPass();
            efectoNightVision.End();

            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
            //device.EndScene();
        }

        public void renderScene(float elapsedTime, String Technique)
        {
            //Dibujamos todos los meshes del escenario

            foreach (TgcMesh m in todosElementosARenderizar)
            {
                m.Effect = efectoNightVision;
                m.Technique = Technique;
                //m.render();
            }

            renderTotal(elapsedTime);
            TgcText2d textoActual;
            textoActual = new TgcText2d();
            textoActual.Text = ((int) contadorNight).ToString();
            //textoActual.Position = new Point(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth * 3/16, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight * 18/20);  <-- Relacion si se quiere a la izquierda de la barra
            textoActual.Position = new Point(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth * 1 / 60, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight * 3 / 10);
            textoActual.Size = new Size(GuiController.Instance.D3dDevice.PresentationParameters.BackBufferWidth, GuiController.Instance.D3dDevice.PresentationParameters.BackBufferHeight);
            textoActual.Color = Color.DarkGreen;
            textoActual.changeFont(new System.Drawing.Font("Chiller", 200, FontStyle.Regular));
            textoActual.render();

        }
        #endregion
        //Fin efectos nightVision
        public enum EstadoMenu
        {
            Menu = 0,
            Objetivo = 1,
            Manual = 2,
            Juego = 3,
            Ganado = 4,
            GameOver = 5,
        }

        public void colisionesConPuerta()
        {
            foreach (Puerta puerta in listaPuertas)
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


                foreach (Enemigo enemigo in listaEnemigos)
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
            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth,
                                                                         d3dDevice.PresentationParameters.BackBufferHeight,
                                                                         DepthFormat.D24S8,
                                                                         MultiSampleType.None,
                                                                         0,
                                                                         true);
            //vertex buffer de los triangulos
            screenQuadVB = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                    4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                        CustomVertex.PositionTextured.Format, Pool.Default);
            screenQuadVB.SetData(screenQuadVertices, 0, LockFlags.None);

            //Creamos un Render Targer sobre el cual se va a dibujar la pantalla
            renderTarget2D = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);

            
            g_pGlowMap = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);

            g_pRenderTarget4 = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth / 4
                    , d3dDevice.PresentationParameters.BackBufferHeight / 4, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);

            g_pRenderTarget4Aux = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth / 4
                    , d3dDevice.PresentationParameters.BackBufferHeight / 4, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);

            //Cargar shader con efectos de Post-Procesado
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            effect = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\blurPersecucion.fx");
            efectoVictoria = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\shaderVictoria.fx");
            efectoMerlusa = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\shaderMerlusa.fx");
            efectoNightVision = TgcShaders.loadEffect(alumnoMediaFolder + "CucarachaJugosita\\GaussianBlur.fx");
            efectoNightVision.SetValue("g_RenderTarget", renderTarget2D);
            efectoNightVision.SetValue("screen_dx", d3dDevice.PresentationParameters.BackBufferWidth);
            efectoNightVision.SetValue("screen_dy", d3dDevice.PresentationParameters.BackBufferHeight);
            //Configurar Technique dentro del shader
            effect.Technique = "BlurTechnique";
            efectoVictoria.Technique = "OscurecerTechnique";
            efectoMerlusa.Technique = "BlurTechnique";
        }

        


        public void colisionesConEscondites()
        {
            foreach (Escondite escondite in listaEscondites)
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
            if (camara.getPosition() != Aux)
            {
                objeto.mover(elapsedTime);
            }
        }
    }
}

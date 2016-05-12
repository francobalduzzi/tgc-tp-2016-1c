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
        ElementoMapa esqueleto;
        ElementoMapa antorcha1;

        

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
            return "Grupo Piola";
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
            camara = new Camara();
            camara.setCamera(new Vector3(359f, 60f, 1000f), new Vector3(289f, 30f, 90f));
            camara.MovementSpeed = 200f;
            camara.RotationSpeed = 5f;
            camara.JumpSpeed = 80f;
            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas

            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcSceneLoader loader = new TgcSceneLoader();
            escena = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\mapaFinal-TgcScene.xml");
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
            listaPuertas.Add(puerta1);
            listaPuertas.Add(puerta2);

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
            enemigo.getCaminoOriginal().SetValue(new Vector3(718.5f, 5.02f, 1493.4f), 0);
            enemigo.getCaminoOriginal().SetValue(new Vector3(1360.2f, 5.02f, 1493.4f), 1);
            enemigo.getCaminoOriginal().SetValue(new Vector3(1360.2f, 5.02f, 503.2f), 2);
            enemigo.setCantidadWP(3);
            enemigo.setEscena(escena);
            enemigo.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo.init();


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

            listaEnemigos.Add(enemigo);
            listaEnemigos.Add(enemigo2); //Cargamos los enemigos

            recargaVela = new VelaRecarga(new Vector3(359f, 7f, 964f));
            recargaFarol = new FarolRecarga(new Vector3(379f,2f,964f));
            recarga = new LinternaRecarga(new Vector3(457f, 5f, 964f));// se carga la/s recarga con la posicion

            pasos = new Sonidos();
            barra = new Barra();



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
            //Device de DirectX para renderizar
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
            //enemigo.seguirA(camara.getPosition(), elapsedTime);
            moverCamaraConVela(elapsedTime); //Actualizamos el valor de la camara y vemos si generar efecto de vela
            // vela1.render();
            objeto.render();
            escena.renderAll();
            colisionesConPuerta();
            puerta1.render();
            puerta2.render();

            verificarRegargas();
            recarga.render(elapsedTime);
            recargaVela.render(elapsedTime);
            recargaFarol.render(elapsedTime);
            objeto.bajarIntensidad(elapsedTime);// bajo la intensidad
            verificarSonidos(elapsedTime);
           // barra.render(linterna.damePorcentaje());
            GuiController.Instance.UserVars.setValue("PosCam", camara.getPosition()); //Actualizamos la user var, nos va a servir

            renderEnemigos(camara.getPosition());
            esqueleto.render();
            antorcha1.render();
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {

        }

        public void verificarRegargas()
        {
            if (recarga.verificarColision(camara))//si agarra la recarga aumento la intensidad 
            {
                linterna.recargar();
            }
            if (recargaVela.verificarColision(camara))//si agarra la recarga aumento la intensidad 
            {
                vela1.recargar();
            }
            if (recargaFarol.verificarColision(camara))//si agarra la recarga aumento la intensidad 
            {
                farol.recargar();
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
        public void renderEnemigos(Vector3 posCam)
        {
            foreach(Enemigo enemigo in listaEnemigos)
            {
                enemigo.render(posCam);
            }
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

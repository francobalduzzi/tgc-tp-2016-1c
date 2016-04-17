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
        LinternaRecarga recarga;
        VelaRecarga recargaVela;
        FarolRecarga recargaFarol;
        Puerta puerta;
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
            camara.setCamera(new Vector3(175f, 60f, 317f), new Vector3(289f, 30f, 90f));
            camara.MovementSpeed = 200f;
            camara.RotationSpeed = 5f;
            camara.JumpSpeed = 80f;
            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas

            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcSceneLoader loader = new TgcSceneLoader();
            escena = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\mapaTerrorExportado-TgcScene.xml");
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
            puerta = new Puerta();
            puerta.init();


            enemigo = new Enemigo(); //Cargamos un enemigo
            enemigo.setEscena(escena);
            enemigo.getCaminoOriginal().SetValue(new Vector3(44, 5.06f, 267), 0);
            enemigo.getCaminoOriginal().SetValue(new Vector3(200, 5.06f, 269), 1);
            enemigo.getCaminoOriginal().SetValue(new Vector3(207, 5.06f, 67), 2);
            enemigo.setCantidadWP(3);
            enemigo.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo.init();

            recargaVela = new VelaRecarga(new Vector3(500f,17f,263f));
            recargaFarol = new FarolRecarga(new Vector3(480f,17f,263f));
            recarga = new LinternaRecarga(new Vector3(527f, 17f, 263f));// se carga la/s recarga con la posicion


            ///////////////USER VARS//////////////////

            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("variablePrueba");
            GuiController.Instance.UserVars.addVar("PosCam");

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
            enemigo.render(camara.getPosition());
            colisionesConPuerta();
            puerta.render();

            if (recarga.verificarColision(camara))//si agarra la recarga aumento la intensidad 
            {
                linterna.recargar();
            }
            recarga.render(elapsedTime);
            recargaVela.render(elapsedTime);
            recargaFarol.render(elapsedTime);
            linterna.bajarIntensidad(elapsedTime);// bajo la intensidad

            GuiController.Instance.UserVars.setValue("PosCam", camara.getPosition()); //Actualizamos la user var, nos va a servir

        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {

        }
        public void colisionesConPuerta()
        {
            if (puerta.verificarColision(camara))
            {
                if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.E))
                {
                    if(puerta.getEstado() == Puerta.Estado.Cerrado)
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
            if (puerta.verificarColision(enemigo))
            {
                puerta.seAbrio();
                enemigo.bloqueado();
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

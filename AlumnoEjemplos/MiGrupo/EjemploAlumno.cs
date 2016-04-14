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



namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        TgcScene escena;
        vela vela1;
        Camara camara;
        Linterna linterna;
        TgcScene puerta;
        TgcMesh meshP;
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
            camara.setCamera(new Vector3(10f, 45f, 90f), new Vector3(289f, 30f, 90f));
            camara.MovementSpeed = 200f;
            camara.RotationSpeed = 5f;
            camara.JumpSpeed = 80f;
            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas

            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcSceneLoader loader = new TgcSceneLoader();
            escena = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\mapitaPiola-TgcScene.xml");
            puerta = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\Component_2-TgcScene.xml");
            meshP = puerta.Meshes[0];
            meshP.Position = new Vector3(40f, 10f, 90f);
            meshP.rotateY(Geometry.DegreeToRadian(90f));
            
            meshP.move();
            vela1 = new vela();
            vela1.init(camara);
            camara.SetEscena(escena);//cargamos la escena en la camara para que detecte colisiones

            //Cargamos la linterna
            linterna = new Linterna(camara.getLookAt(), camara.getPosition());
            //vela.Meshes[0].Rotation = new Vector3(-5f, -14f, 0f);
            //Camara en primera persona:
            Vector3 mira = new Vector3(0,0,0);
            Vector3 vector = new Vector3(0,0,20);

            ///////////////USER VARS//////////////////

            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("variablePrueba");

            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("variablePrueba", 5451);



            ///////////////MODIFIERS//////////////////

            //Crear un modifier para un valor FLOAT
            GuiController.Instance.Modifiers.addFloat("valorFloat", -50f, 200f, 0f);

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
            efectosLinterna();
            //Obtener valor de UserVar (hay que castear)
            int valor = (int)GuiController.Instance.UserVars.getValue("variablePrueba");
           
            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia

            //Capturar Input teclado 
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.F))
            {
                linterna.CambiarEstadoLuz();
            }
            //Capturar Input Mouse
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Boton izq apretado
            }
            moverCamaraConVela(elapsedTime); //Actualizamos el valor de la camara y vemos si generar efecto de vela
           // vela1.render();
            linterna.render();
            meshP.render();
            escena.renderAll();

        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {

        }

        public void moverCamaraConVela(float elapsedTime)
        {
            Vector3 Aux = camara.getPosition();
            camara.updateCamera();
            if(camara.getPosition() != Aux)
            {
                linterna.moverLinterna(elapsedTime);
                vela1.moverVela(elapsedTime, camara);
            }
        }
        public void efectosLinterna()
        {
            linterna.Posicion = camara.getPosition();
            linterna.Direccion = camara.getLookAt();
            Vector3 lightDir;
            lightDir = linterna.Direccion - linterna.Posicion;
            lightDir.Normalize();
            Effect currentShader;
            if (linterna.Encendida)
            {
                //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con SpotLight
                currentShader = GuiController.Instance.Shaders.TgcMeshSpotLightShader;
            }
            else
            {
                //Sin luz: Restaurar shader default
                currentShader = GuiController.Instance.Shaders.TgcMeshShader;
            }

            foreach (TgcMesh mesh in escena.Meshes)
            {
                mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
            }
            //Renderizar meshes
            foreach (TgcMesh mesh in escena.Meshes)
            {
                if (linterna.Encendida)
                {
                    //Cargar variables shader de la luz
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(linterna.Posicion));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(linterna.Posicion));
                    mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat4Array(lightDir));
                    mesh.Effect.SetValue("lightIntensity", linterna.Intensity);
                    mesh.Effect.SetValue("lightAttenuation", linterna.Attenuation);
                    mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(linterna.SpotAngle));
                    mesh.Effect.SetValue("spotLightExponent", linterna.SpotExponent);

                    //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", linterna.SpecularEx);
                }

                //No renderizar aca porque hace una caja negra loca
            }
        }
    }
}

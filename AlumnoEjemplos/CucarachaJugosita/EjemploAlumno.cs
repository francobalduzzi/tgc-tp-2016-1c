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
        Enemigo enemigo1; // Uno solo para las pruebas dsps abra que hacer una lista. Hay que pasarla las coordenadas para que pueda arrancar y el estado. Camino ida necesita minimo 1 parametro aunque se quede quieto
        Enemigo enemigo2;
        Enemigo enemigo3;
        Enemigo enemigo4;
        Enemigo enemigo5;
        EnemigoAnimacion enemigoAnimado;
        EnemigoAnimacion enemigoAnimado2;
        EnemigoAnimacion enemigoAnimado3;
        LinternaRecarga recarga;
        LinternaRecarga recarga1;
        LinternaRecarga recarga2;
        LinternaRecarga recarga3;
        VelaRecarga recargaVela1;
        VelaRecarga recargaVela2;
        VelaRecarga recargaVela3;
        FarolRecarga recargaFarol1;
        FarolRecarga recargaFarol2;
        FarolRecarga recargaFarol3;
        Llave llave;
        Llave llave2;
        Llave llave3;
        Llave llave4;
        Llave llave5;
        Llave llave6;
        Puerta puerta1;
        Puerta puerta2;
        Puerta puerta3;
        Puerta puerta4;
        Puerta puerta5;
        Puerta puerta6;
        Sonidos sonidos;
        Barra barra;
        ArrayList listaEnemigos;
        ArrayList listaPuertas;
        ArrayList listaRecargas;
        ArrayList listaElementoMapa;
        ArrayList listaLlaves;
        ArrayList todosElementosARenderizar; //Hay que separar en 2 xq los enemigos son skeletical
        ArrayList enemigosARenderizar;
        ArrayList elementosDesaparecedores;
        LuzNormal antorcha1;
        LuzNormal antorcha2;
        LuzNormal antorcha3;
        LuzNormal antorcha4;
        LuzNormal antorcha5;
        LuzNormal antorcha6;
        LuzNormal antorcha7;
        LuzNormal antorcha8;
        LuzNormal antorcha9;
        List<LuzNormal> listaLuces;
        ElementoMapa cosa1;
        ElementoMapa cosa2;
        ElementoMapa cosa3;
        ElementoMapa cosa4;
        ElementoMapa cosa5;
        ElementoMapa cosa6;
        ElementoMapa cosa7;
        ElementoMapa cosa8;
        ElementoMapa cosa9;
        ElementoMapa cosa10;
        ElementoMapa cosa11;
        ElementoMapa cosa12;
        ArrayList listaEscondites;
        Escondite escondite1;
        Escondite escondite2;
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
        NightRecarga activadorNightvision;
        float time;
        public float timeMerlusa; // Vuevle a 0 cada vez que se le termina la merlusa
        public Boolean merlusaPostPersecucion;
        public float contadorSecundarioMerlusa;
        public Boolean reproducidoMerlusa;
        Boolean nightVision;
        float contadorNight;
        TgcMesh meshInservible;
        PuertaFinal puertaF;
        Boolean finPartida;
        Boolean perseguido;
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
            time = 0;
            timeMerlusa = 0f; // Vuevle a 0 cada vez que se le termina la merlusa
            merlusaPostPersecucion = false;
            contadorSecundarioMerlusa = 9f;
            reproducidoMerlusa = false;
            nightVision = false;
            contadorNight = 30f;


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
            puerta3 = new Puerta();//puertaSusto
            puerta3.init(new Vector3(2645f, 57f, 1971f));
            puerta3.rotateY(3.14f);
            puerta4 = new Puerta();
            puerta4.init(new Vector3(3193f, 57f, 1300f));
            puerta4.rotateY(-3.14f);
            puerta5 = new Puerta();
            puerta5.init(new Vector3(1425f, 57f, 1270f));
            puerta5.rotateY(-3.14f);
            puerta6 = new Puerta();//puertaSusto
            puerta6.init(new Vector3(4442f, 57f, 1232f));
            puerta6.rotateY(3.14f);

            //Añadimos puertas a la lista
            listaPuertas.Add(puerta1);
            listaPuertas.Add(puerta2);
            listaPuertas.Add(puerta3);
            listaPuertas.Add(puerta4);
            listaPuertas.Add(puerta5);
            listaPuertas.Add(puerta6);

            foreach (Puerta puerta in listaPuertas) //Aca añadimos los meshes correspondiente a cada puerta a la escena
            {
                escena.Meshes.Add(puerta.getMeshC1());
                escena.Meshes.Add(puerta.getMeshC3());
                escena.Meshes.Add(puerta.getMeshC2());
                escena.Meshes.Add(puerta.getMeshP());
            }
            listaEnemigos = new ArrayList(); //Creamos lista de enemigos
            enemigo1 = new Enemigo(); //Cargamos un enemigo  Toda la carga de enemigos se hace en una funcion cargar enemigos, etc
            enemigo1.setEscena(escena);
            enemigo1.getCaminoOriginal().SetValue(new Vector3(376f, 5.02f, 967f), 0);
            enemigo1.getCaminoOriginal().SetValue(new Vector3(376f, 5.02f, 1500f), 1);
            enemigo1.getCaminoOriginal().SetValue(new Vector3(1458f, 5.02f, 1490f), 2);
            enemigo1.getCaminoOriginal().SetValue(new Vector3(1470f, 5.02f, 1047f), 3);
            //enemigo.getCaminoOriginal().SetValue(new Vector3(1360.2f, 5.02f, 503.2f), 2);
            enemigo1.setCantidadWP(4);
            enemigo1.setEscena(escena);
            enemigo1.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo1.init();
            enemigo1.setCamara(camara);


            cosa1 = new ElementoMapa("Esqueleto-TgcScene.xml", new Vector3(225f, 0f, 312f));
            cosa1.rotateY(1.57f);
            cosa2 = new ElementoMapa("Esqueleto2-TgcScene.xml", new Vector3(934f, 5.02f, 164f));
            cosa2.rotateY(1.57f);
            cosa3 = new ElementoMapa("Esqueleto3-TgcScene.xml", new Vector3(325f, 0f, 1004f));
            cosa3.rotateY(-1.57f);
            cosa4 = new ElementoMapa("Esqueleto-TgcScene.xml", new Vector3(1740f, 0f, 1520f));
            cosa5 = new ElementoMapa("Esqueleto2-TgcScene.xml", new Vector3(1467f, 0f, 958f));
            cosa5.rotateY(3.14f);
            cosa6 = new ElementoMapa("Esqueleto3-TgcScene.xml", new Vector3(1815f, 0f, 645f));
            cosa6.rotateY(1.57f);
            cosa7 = new ElementoMapa("Esqueleto-TgcScene.xml", new Vector3(3235f, 0f, 825f));
            cosa7.rotateY(3.14f);
            cosa8 = new ElementoMapa("Esqueleto2-TgcScene.xml", new Vector3(4485f, 0f, 1737f));
            cosa9 = new ElementoMapa("Esqueleto3-TgcScene.xml", new Vector3(2657f, 0f, 575f));
            cosa9.rotateY(3.14f);
            cosa10 = new ElementoMapa("Esqueleto-TgcScene.xml", new Vector3(1180f, 0f, 330f));
            cosa10.rotateY(1.57f);
            cosa11 = new ElementoMapa("Esqueleto2-TgcScene.xml", new Vector3(866f, 0f, 1136f));
            cosa12 = new ElementoMapa("Esqueleto3-TgcScene.xml", new Vector3(1755f, 0f, 2120f));


            enemigo2 = new Enemigo2(); //Cargamos un enemigo
            enemigo2.setEscena(escena);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(2075f, 5.02f, 396f), 0);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(1396f, 5.02f, 392f), 1);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(1413f, 5.02f, 178f), 2);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(2055f, 5.02f, 194f), 3);
            enemigo2.getCaminoOriginal().SetValue(new Vector3(2075f, 5.02f, 396f), 4);
            enemigo2.setCantidadWP(5);
            enemigo2.setEscena(escena);
            enemigo2.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo2.init();
            enemigo2.setCamara(camara);

            enemigo3 = new Enemigo(); //Cargamos un enemigo
            enemigo3.setEscena(escena);
            enemigo3.getCaminoOriginal().SetValue(new Vector3(3027f, 5.02f, 289f), 0);
            enemigo3.getCaminoOriginal().SetValue(new Vector3(3027f, 5.02f, 1092f), 1);
            enemigo3.getCaminoOriginal().SetValue(new Vector3(3433f, 5.02f, 1108f), 2);
            enemigo3.getCaminoOriginal().SetValue(new Vector3(3433f, 5.02f, 378f), 3);
            enemigo3.getCaminoOriginal().SetValue(new Vector3(3027f, 5.02f, 289f), 4);
            enemigo3.setCantidadWP(5);
            enemigo3.setEscena(escena);
            enemigo3.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo3.init();
            enemigo3.setCamara(camara);

            enemigo4 = new Enemigo(); //Cargamos un enemigo
            enemigo4.setEscena(escena);
            enemigo4.getCaminoOriginal().SetValue(new Vector3(1254f, 5.02f, 1049f), 0);
            enemigo4.getCaminoOriginal().SetValue(new Vector3(597f, 5.02f, 1035f), 1);
            enemigo4.setCantidadWP(2);
            enemigo4.setEscena(escena);
            enemigo4.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo4.init();
            enemigo4.setCamara(camara);


            enemigo5 = new Enemigo2(); //Cargamos un enemigo
            enemigo5.setEscena(escena);
            enemigo5.getCaminoOriginal().SetValue(new Vector3(1144f, 5.02f, 301f), 0);
            enemigo5.getCaminoOriginal().SetValue(new Vector3(1148f, 5.02f, 807f), 1);
            enemigo5.getCaminoOriginal().SetValue(new Vector3(1682f, 5.02f, 812f), 2);
            enemigo5.setCantidadWP(3);
            enemigo5.setEscena(escena);
            enemigo5.setEstado(Enemigo.Estado.RecorriendoIda);
            enemigo5.init();
            enemigo5.setCamara(camara);


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

            enemigoAnimado2 = new EnemigoAnimacion(new Vector3(2673f, 60f, 1913f), new Vector3(100, 100, 80));
            enemigoAnimado2.getCaminoOriginal().SetValue(new Vector3(2677f, 5.02f, 2006f), 0);
            enemigoAnimado2.getCaminoOriginal().SetValue(new Vector3(2669f, 5.02f, 1817f), 1);
            enemigoAnimado2.getCaminoOriginal().SetValue(new Vector3(3294f, 5.02f, 1811f), 2);
            enemigoAnimado2.setCantidadWP(3);
            enemigoAnimado2.setEstado(Enemigo.Estado.Parado);
            enemigoAnimado2.init();

            enemigoAnimado3 = new EnemigoAnimacion(new Vector3(4473f, 60f, 1177f), new Vector3(100, 100, 80));
            enemigoAnimado3.getCaminoOriginal().SetValue(new Vector3(4470f, 5.02f, 1288f), 0);
            enemigoAnimado3.getCaminoOriginal().SetValue(new Vector3(4470f, 5.02f, 565f), 1);
            enemigoAnimado3.getCaminoOriginal().SetValue(new Vector3(3805f, 5.02f, 565f), 2);
            enemigoAnimado3.setCantidadWP(3);
            enemigoAnimado3.setEstado(Enemigo.Estado.Parado);
            enemigoAnimado3.init();
            //Añadimos enemigos a la lista
            listaEnemigos.Add(enemigoAnimado3);
            listaEnemigos.Add(enemigoAnimado2);
            listaEnemigos.Add(enemigoAnimado);
            listaEnemigos.Add(enemigo1);
            listaEnemigos.Add(enemigo2); //Cargamos los enemigos
            listaEnemigos.Add(enemigo3);
            listaEnemigos.Add(enemigo4);
            listaEnemigos.Add(enemigo5);
            recargaVela1 = new VelaRecarga(new Vector3(1388f, 32f, 294f), vela1);
            recargaVela2 = new VelaRecarga(new Vector3(4247f, 32f, 1637f), vela1);
            recargaVela3 = new VelaRecarga(new Vector3(2870f, 32f, 2224f), vela1);
            recargaFarol1 = new FarolRecarga(new Vector3(1405f, 27f, 807f), farol);
            recargaFarol2 = new FarolRecarga(new Vector3(3895f, 27f, 565f), farol);
            recargaFarol3 = new FarolRecarga(new Vector3(3034f, 27f, 1802f), farol);
            recarga = new LinternaRecarga(new Vector3(961f, 30f, 2020f), linterna);
            recarga1 = new LinternaRecarga(new Vector3(3434f, 30f, 661f), linterna);
            recarga2 = new LinternaRecarga(new Vector3(4716f, 30f, 1656f), linterna);
            recarga3 = new LinternaRecarga(new Vector3(2428f, 30f, 2245f), linterna);

            sonidos = new Sonidos();
            barra = new Barra();

            //Añadimos los escondite a la lista
            listaEscondites = new ArrayList();
            escondite1 = new Escondite(); //Para poner casillero le pasamos de path: "LockerMetal-TgcScene.xml" y de tipo: Escondito.Tipo.Casillero.
                                          //Para poner mesa le pasamos de path: "mesaPiola-TgcScene.xml" y de tipo: Escondite.Tipo.Mesa
            escondite1.init(new Vector3(1191f, 5.02f, 370f), new Vector3(963f, 5.02f, 370f), "LockerAlphaBlending-TgcScene.xml", Escondite.Tipo.Casillero);
            listaEscondites.Add(escondite1);
            escondite2 = new Escondite();
            escondite2.init(new Vector3(521f, 5.02f, 1045f), new Vector3(523f, 5.02f, 860f), "mesaPiola-TgcScene.xml", Escondite.Tipo.Mesa);
            listaEscondites.Add(escondite2);

            //Añadimos recargas a la lista
            listaRecargas = new ArrayList();
            listaRecargas.Add(recargaFarol1);
            listaRecargas.Add(recargaFarol2);
            listaRecargas.Add(recargaFarol3);
            listaRecargas.Add(recargaVela1);
            listaRecargas.Add(recargaVela2);
            listaRecargas.Add(recargaVela3);
            listaRecargas.Add(recarga);
            listaRecargas.Add(recarga1);
            listaRecargas.Add(recarga2);
            listaRecargas.Add(recarga3);

            //Añadimos llaves al mapa
            llave = new Llave(new Vector3(570f, 40f, 1030f));
            llave2 = new Llave(new Vector3(1676f, 40f, 614f));
            llave3 = new Llave(new Vector3(2437f, 40f, 98f));
            llave4 = new Llave(new Vector3(3228f, 40f, 1061f));
            llave5 = new Llave(new Vector3(1759f, 40f, 1986f));
            llave6 = new Llave(new Vector3(4485f, 40f, 1524f));
            //Añadimos llaves a la lista
            listaLlaves = new ArrayList();
            listaLlaves.Add(llave);
            listaLlaves.Add(llave2);
            listaLlaves.Add(llave3);
            listaLlaves.Add(llave4);
            listaLlaves.Add(llave5);
            listaLlaves.Add(llave6);
            numeroLLaves = new NumerosLlaves();
            numeroLLaves.setNumeroLLaves(listaLlaves.Count);

            //Añado el unico trofeo
            trofeo = new Trofeo(new Vector3(2524f, 10f, 626f));

            //Añadimos elementos del mapa
            listaElementoMapa = new ArrayList();
            listaElementoMapa.Add(cosa1);
            listaElementoMapa.Add(cosa2);
            listaElementoMapa.Add(cosa3);
            listaElementoMapa.Add(cosa4);
            listaElementoMapa.Add(cosa5);
            listaElementoMapa.Add(cosa6);
            listaElementoMapa.Add(cosa7);
            listaElementoMapa.Add(cosa8);
            listaElementoMapa.Add(cosa9);
            listaElementoMapa.Add(cosa10);
            listaElementoMapa.Add(cosa11);
            listaElementoMapa.Add(cosa12);

            manejoI = new ManejoIluminacion();
            manejoI.setEscena(escena);

            listaLuces = new List<LuzNormal>();

            antorcha1 = new LuzNormal(new Vector3(138f, 80f, 10f));
            antorcha1.rotateY(3.14f);
            antorcha1.lightColor = Color.White;
            antorcha1.Intensity = 20f;
            antorcha1.Attenuation = 0.5f;
            antorcha2 = new LuzNormal(new Vector3(806f, 80f, 485f));
            antorcha2.lightColor = Color.White;
            antorcha2.Intensity = 20f;
            antorcha2.Attenuation = 0.5f;
            antorcha3 = new LuzNormal(new Vector3(1191f, 80f, 351f));
            antorcha3.lightColor = Color.White;
            antorcha3.Intensity = 20f;
            antorcha3.Attenuation = 0.5f;
            antorcha3.rotateY(1.57f);
            antorcha4 = new LuzNormal(new Vector3(1689f, 80f, 343f));
            antorcha4.lightColor = Color.White;
            antorcha4.Intensity = 20f;
            antorcha4.Attenuation = 0.5f;
            antorcha4.rotateY(3.14f);
            antorcha5 = new LuzNormal(new Vector3(1467f, 80f, 958f));
            antorcha5.lightColor = Color.White;
            antorcha5.Intensity = 20f;
            antorcha5.Attenuation = 0.5f;
            antorcha5.rotateY(3.14f);
            antorcha6 = new LuzNormal(new Vector3(310f, 80f, 974f));
            antorcha6.lightColor = Color.White;
            antorcha6.Intensity = 20f;
            antorcha6.Attenuation = 0.5f;
            antorcha6.rotateY(-1.57f);
            antorcha7 = new LuzNormal(new Vector3(2703f, 80f, 2342f));
            antorcha7.lightColor = Color.White;
            antorcha7.Intensity = 20f;
            antorcha7.Attenuation = 0.5f;
            antorcha8 = new LuzNormal(new Vector3(4485f, 80f, 1737f));
            antorcha8.lightColor = Color.White;
            antorcha8.Intensity = 20f;
            antorcha8.Attenuation = 0.5f;
            antorcha9 = new LuzNormal(new Vector3(2534f, 80f, 557f));
            antorcha9.lightColor = Color.White;
            antorcha9.Intensity = 20f;
            antorcha9.Attenuation = 0.5f;
            antorcha9.rotateY(3.14f);
            listaLuces.Add(antorcha1);
            listaLuces.Add(antorcha2);
            listaLuces.Add(antorcha3);
            listaLuces.Add(antorcha4);
            listaLuces.Add(antorcha5);
            listaLuces.Add(antorcha6);
            listaLuces.Add(antorcha7);
            listaLuces.Add(antorcha8);
            listaLuces.Add(antorcha9);
            manejoI.setListaLuces(listaLuces);
            camara.setEnemigos(listaEnemigos);


            //Instanciamos puerta final
            puertaF = new PuertaFinal();
            puertaF.init(new Vector3(2560f, 57f, 866f), numeroLLaves);
            escena.Meshes.Add(puertaF.getMeshP());
            escena.Meshes.Add(puertaF.getMeshC1());
            escena.Meshes.Add(puertaF.getMeshC2());
            escena.Meshes.Add(puertaF.getMeshC3());

            GuiController.Instance.FullScreenEnable =false;


            //Hacer que el Listener del sonido 3D siga al personaje
            TgcScene escena2;
            escena2 = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\mapitaFinal1-TgcScene.xml"); // Esto es una meirda que no sirve para nada, solo xq sonido3d trabajo con mehshes y no con vectores
            meshInservible = escena2.Meshes[0];
            meshInservible.Position = camara.getPosition();
            GuiController.Instance.DirectSound.ListenerTracking = meshInservible;


            activadorNightvision = new NightRecarga(new Vector3(794f, 30f, 138f));

            //Aca vamos a cargar todos los elementos a renderizar en una lista generica -- Para la iluminacion
            todosElementosARenderizar = new ArrayList();
            enemigosARenderizar = new ArrayList();
            elementosDesaparecedores = new ArrayList();
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
                escena.Meshes.Add(escondite.getMesh());
            }
            foreach(Llave llave in listaLlaves)
            {
                elementosDesaparecedores.Add(llave);
            }
            foreach(ElementoMapa elemento in listaElementoMapa)
            {
                todosElementosARenderizar.Add(elemento.mesh);
            }
            foreach(Recarga recarga in listaRecargas)
            {
                elementosDesaparecedores.Add(recarga);
            }
            elementosDesaparecedores.Add(activadorNightvision);
            manejoI.setTodosLosElementos(todosElementosARenderizar);
            manejoI.setEnemigosARenderizar(enemigosARenderizar);
            manejoI.setElementosDesaparecedores(elementosDesaparecedores);








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
                case EstadoMenu.GameOver:
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    gameOver.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                    this.reiniciar();
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.V))
                    {
                        estadoMenu = EstadoMenu.Menu;
                    }
                    break;
                case EstadoMenu.Ganado:
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    ganado.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                    this.reiniciar();
                    if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.V))
                    {
                        estadoMenu = EstadoMenu.Menu;
                    }
                    break;
                case EstadoMenu.Juego:
                    Device d3dDevice = GuiController.Instance.D3dDevice;
                    //sonidos.playFondo();  ---->> Musica de fondo en todo el juego probar si dejar esta
                    //objeto.actualizarEscenario(escena, camara); // Atencion aca, esto es como moo de prueba baja mucho ls FPS, lo ideal seria tener ls meshes cocinados y en el init del programa estos se carguen a cada uno de los objetos
                    //Obtener valor de UserVar (hay que castear)
                    if (finPartida)
                    {
                        merlusaPostPersecucion = false;
                        //finPartida = false;
                    }
                    
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
                    //renderTotal(elapsedTime);
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
            
            if (nightVision && contadorNight > 0) //Hacemos esto para que el nightvision no arrastre valores de linterna etc.
            {
                foreach(TgcMesh mesh in todosElementosARenderizar)
                {
                    mesh.render();
                }
                foreach(ElementoDesaparecedor elemento in elementosDesaparecedores)
                {
                    if (elemento.desaparecer())
                    {
                        elemento.getMesh().render();
                    }

                   
                }
                chequearPersecucion();
            }
            else
            {
                manejoI.iluminar(objeto, camara);
                objeto.bajarIntensidad(elapsedTime);// bajo la intensidad -->>> La bajamos solo si no esta el NightVision
            }
            
            colisionesConPuerta();
            renderPuertas();
            verificarRegargas();
            renderRecargas(elapsedTime);
            
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
            colisionPuertaF();
            colisionEnemigoCamara();
            colisionVictoria();
            puertaF.render();
            sonidos.sonidoMonstruo(elapsedTime);
            sonidos.sonidoRespiracionCadaTanto(elapsedTime);
            activarNightVision();
            activadorNightvision.render(elapsedTime);
            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
            objeto.render();
        }

        public void activarNightVision()
        {
            if (activadorNightvision.verificarColision(camara))
            {
                nightVision = true;
            }
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
                    sonidos.playPersecucion();
                    perseguido = true;
                    efectoPostProcesadoPersecucion(elapsedTime, d3dDevice);
                    merlusaPostPersecucion = true;
                    timeMerlusa = 0;
                    
                }
                else
                {
                    perseguido = false;
                    sonidos.stopPersecucion();
                    merlusa = camara.activarEfectoMerlusa();
                    if (merlusaPostPersecucion)
                    {
                        merlusa = merlusaPostPersecucion;
                        

                    }
                    if (merlusa || timeMerlusa != 0 || (objeto.Intensity > -0.05f && objeto.Intensity < 0.05f))
                    {
                        
                        if (!reproducidoMerlusa || merlusaPostPersecucion)
                            {
                            if (!finPartida)
                            {
                                merlusaPostPersecucion = false;
                                reproducidoMerlusa = true;
                                sonidos.playMerlusa();
                            }
                            else
                            {
                                finPartida = false;
                            }
                                
                            }
                        camara.efectoMerlusa(timeMerlusa + contadorSecundarioMerlusa);
                        efectoPostProcesadoMerlusa(elapsedTime, d3dDevice, merlusa);
                        
                    }
                    else
                    {
                        //sonidos.stopMerlusa();
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

            efectoVictoria.Technique = "OscurecerTechnique";
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

            pSurf.Dispose();

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
            /*foreach(LuzNormal luz in listaLuces)
            {
                luz.getMesh().render();
            }*/
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
            efectoVictoria.SetValue("blur_intensity", 0.010f);
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
                if(timeMerlusa <= 2f) //Aca marcamos el coeficiente maximo de la merluseria
                {
                    timeMerlusa += elapsedTime;
                }
                
            }
            else
            {
                timeMerlusa -= 3*elapsedTime;
                contadorSecundarioMerlusa -= elapsedTime;
                if(timeMerlusa <= 0.05f)
                {
                    if(contadorSecundarioMerlusa >= 0f)
                    {
                        timeMerlusa = FastMath.Sin(contadorSecundarioMerlusa);
                    }
                    else
                    {
                        timeMerlusa = 0;
                        contadorSecundarioMerlusa = 9f;
                        reproducidoMerlusa = false;
                    }
                    
                }
            }
            
            effect.SetValue("time", time/2);
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
            //Render de llaves y recargas brillosas
            foreach (ElementoDesaparecedor elemento in elementosDesaparecedores)
            {
                Ray rayo = new Ray(camara.getPosition(), elemento.getMesh().Position);
                int contador = 0;
                foreach (TgcMesh mesh in escena.Meshes)
                {
                    if (rayo.intersectAABB(mesh.BoundingBox))
                    {
                        contador++;
                    }

                }
                if (contador == 0)
                {
                    if (elemento.desaparecer())
                    {
                        elemento.getMesh().render();
                    }
                    
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

        public void chequearPersecucion()
        {
            int contador = 0;
            foreach (Enemigo enemigo in listaEnemigos)
                {
                if (enemigo.getEstado() == Enemigo.Estado.Persiguiendo)
                     {
                        contador++;
                    }
                }
           if (contador > 0)
            {
                perseguido = true;
            }
            else
            {
                perseguido = false;
            }
        }


        public enum EstadoMenu
        {
            Menu = 0,
            Objetivo = 1,
            Manual = 2,
            Juego = 3,
            Ganado = 4,
            GameOver = 5,
        }
        public void colisionVictoria()
        {
            TgcBox bounding = new TgcBox();
            bounding = TgcBox.fromSize(camara.getPosition(), new Vector3(5f, 5f, 5f));
            if (TgcCollisionUtils.testAABBAABB(trofeo.meshTrofeo.BoundingBox, bounding.BoundingBox))
            {
                estadoMenu = EstadoMenu.Ganado;
            }
        }
        public void colisionEnemigoCamara()
        {
            TgcBox bounding = new TgcBox();
            bounding = TgcBox.fromSize(camara.getPosition(), new Vector3(5f, 5f, 5f));
            int contador1 = 0;
            foreach (Enemigo enemigo in listaEnemigos)
            {
                if (TgcCollisionUtils.testAABBAABB(enemigo.getMesh().BoundingBox, bounding.BoundingBox) && enemigo.getEstado() == Enemigo.Estado.Persiguiendo)
                {
                    contador1++;
                }
            }
            if(contador1>0)
            {
                estadoMenu = EstadoMenu.GameOver;
                finPartida = true;
            }
        }
        public void colisionPuertaF()
        {
            if (puertaF.verificarColision(camara))
            {
                if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.E))
                {
                    if (puertaF.getEstado() == PuertaFinal.Estado.Cerrado)
                    {
                        puertaF.seAbrioJugador();
                        camara.bloqueada();
                    }
                    else
                    {
                        puertaF.seCerroJugador();
                        camara.bloqueada();
                    }
                }
            }
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
                if (escondite.verificarColision(camara, perseguido))
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






        public void reiniciar()  //Esta cosa horrible va abajo de todo
        {
            camara.setCamera(new Vector3(60f, 60f, 183f), new Vector3(289f, 30f, 90f));
            camara.MovementSpeed = 200f;
            camara.RotationSpeed = 5f;
            camara.JumpSpeed = 80f;
            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas

            //Carpeta de archivos Media del alumno
            vela1.reiniciar();

            //Cargamos la linterna
            linterna.reiniciar();
            farol.reiniciar();
            objeto = farol; //Empieza con el farol
            //vela.Meshes[0].Rotation = new Vector3(-5f, -14f, 0f);
            //Camara en primera persona:
            foreach(Puerta puerta in listaPuertas)
            {
                puerta.estado = Puerta.Estado.Cerrado;
            }

            foreach(Enemigo enemigo in listaEnemigos)
            {
                enemigo.reiniciar();
            }
            enemigoAnimado.reiniciarAnimacion(); //Hay que reiniciarle las banderas
            enemigoAnimado2.reiniciarAnimacion();
            enemigoAnimado3.reiniciarAnimacion();
            foreach(ElementoDesaparecedor elemento in elementosDesaparecedores)
            {
                elemento.reiniciar();
            }

            numeroLLaves = new NumerosLlaves(); // Esto es una pija, hay que recrear el objeto sino no funca y no reinicia el contador
            numeroLLaves.setNumeroLLaves(listaLlaves.Count);
            trofeo.reiniciar();


            //Instanciamos puerta final
            puertaF.estado = PuertaFinal.Estado.Cerrado;


             time = 0;
             timeMerlusa = 0f; // Vuevle a 0 cada vez que se le termina la merlusa
             merlusaPostPersecucion = false;
             contadorSecundarioMerlusa = 9f;
             reproducidoMerlusa = false;
             nightVision = false;
             contadorNight = 30f;
             sonidos.stopFondo();
             sonidos.stopMerlusa();
             sonidos.stopPersecucion();
             sonidos.stopSonidoMonstruo();
             sonidos.stopSonidoRespiracionCadaTanto();
             sonidos.dispose(); //Hago dispose y vuevlo a cargar todos los sonidos por las dudas
             sonidos.init(); //Los cargamos de vuelta para que empiecen de 0

        }
    }
}

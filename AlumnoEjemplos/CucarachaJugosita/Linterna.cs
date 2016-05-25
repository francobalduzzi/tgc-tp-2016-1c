using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using System.Drawing;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.CucarachaJugosita
{
    public class Linterna : TipoIluminador
    {

        Device d3dDevice = GuiController.Instance.D3dDevice;
        public TgcScene linterna;
        private float movimientoLinterna;
        const float VALORMAXIMOINTENSIDAD = 20;
        private float porcentajeRestante;
        Barra barra;
        public Linterna(Vector3 Direccion1, Vector3 Posicion1)
        {
            this.init();
            this.Direccion = Direccion1;
            this.Posicion = Posicion1;
            this.SpotAngle = 20f;
            this.SpotExponent = 40f;
            this.Intensity = VALORMAXIMOINTENSIDAD;
            this.Encendida = false;
            this.color = Color.White;
            this.Attenuation = 0.1f;
            this.SpecularEx = 9f;
            barra = new Barra();
        }
        override public void mover(float elapsedTime)
        {
            movimientoLinterna += elapsedTime;
            var random = FastMath.Cos(6 * movimientoLinterna);
            var random2 = FastMath.Cos(12 * movimientoLinterna);
            linterna.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-350f + random2), Geometry.DegreeToRadian(180f + random), Geometry.DegreeToRadian(-270f));
        }
        override public void init()
        {
            tipo = Tipo.Linterna;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosDir;
            var loader = new TgcSceneLoader();
            linterna = loader.loadSceneFromFile(alumnoMediaFolder + "CucarachaJugosita\\Media\\Linterna-TgcScene.xml");
            linterna.Meshes[0].Scale = new Vector3(0.4f, 0.4f, 0.4f);
            linterna.Meshes[0].Position = Posicion + new Vector3(-30f, -15f, 60f);
            linterna.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-350f), Geometry.DegreeToRadian(180f), Geometry.DegreeToRadian(-270f));
        }
        public float damePorcentaje()
        {
            return ((this.Intensity) / VALORMAXIMOINTENSIDAD);
        }
        override public void render()
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            GuiController.Instance.D3dDevice.Transform.View = Matrix.Identity;
            linterna.renderAll();
            GuiController.Instance.D3dDevice.Transform.View = matrizView;
            barra.render(this.damePorcentaje());
        }
        override public void CambiarEstadoLuz()
        {
            this.Encendida = !this.Encendida;
        }




        public void recargar()
        {
            this.Intensity += VALORMAXIMOINTENSIDAD * 0.25f;
            if (this.Intensity > VALORMAXIMOINTENSIDAD)
            {
                this.Intensity = VALORMAXIMOINTENSIDAD;
            }
        }

        public override void bajarIntensidad(float elapsedTime)
        {
            if (this.Intensity > -0.05f && this.Intensity < 0.05f)
            {
                this.Encendida = false;
            }
            if (this.Encendida)
            {
                this.Intensity = (Intensity - (0.2f * elapsedTime));//si es necesario modificar velocidad de reduccion  
            }
        }





        
    }
}

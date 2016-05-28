using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils._2D;
using System.Drawing;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class Vela : TipoIluminador
    {
        private TgcScene vela1;
        private float movimientoVela = 0f;
        Barra barra;
        const float VALORMAXIMOINTENSIDAD = 20;
        override public void init()
        {
            tipo = Tipo.Vela;
            this.Encendida = true;
            var loader = new TgcSceneLoader();
            vela1 = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Objetos\\Vela\\Vela-TgcScene.xml");
            vela1.Meshes[0].Scale = new Vector3(2f, 2f, 2f);
            vela1.Meshes[0].Position = GuiController.Instance.CurrentCamera.getPosition() + new Vector3(-30f, -20f, 60f);
            vela1.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-5f), Geometry.DegreeToRadian(-14f), Geometry.DegreeToRadian(0f));
            barra = new Barra();
            Intensity = VALORMAXIMOINTENSIDAD;
            Attenuation = 0.7f;
            this.color = Color.White;
            Encendida = false;
        }
        public void recargar()
        {
            this.Intensity += VALORMAXIMOINTENSIDAD * 0.25f;
            if (this.Intensity > VALORMAXIMOINTENSIDAD)
            {
                this.Intensity = VALORMAXIMOINTENSIDAD;
            }
        }

        override public void mover(float elapsedTime)
        {
            movimientoVela += elapsedTime;
            var random = FastMath.Cos(6 * movimientoVela);
            var random2 = FastMath.Cos(12 * movimientoVela);
            vela1.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-5f + random2), Geometry.DegreeToRadian(-14f + random), Geometry.DegreeToRadian(0f));

        }
        override public void render()
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            GuiController.Instance.D3dDevice.Transform.View = Matrix.Identity;
            vela1.renderAll();
            GuiController.Instance.D3dDevice.Transform.View = matrizView;
            barra.render(this.damePorcentaje());
        }
        override public void CambiarEstadoLuz()
        {
            this.Encendida = !this.Encendida;
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
        public float damePorcentaje()
        {
            return ((this.Intensity) / VALORMAXIMOINTENSIDAD);
        }
      
       public void reiniciar()
        {
            Intensity = VALORMAXIMOINTENSIDAD;
            Encendida = false;
        }

    }
}

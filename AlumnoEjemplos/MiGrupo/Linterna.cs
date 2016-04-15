using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;

namespace AlumnoEjemplos.MiGrupo
{
    public class Linterna
    {
        public Vector3 Direccion { get; set; } //Posicion y direccion de la camara no de la linterna
        public Vector3 Posicion { get; set; }
        public float SpotAngle { get; set; }
        public float SpotExponent { get; set; }
        public float Intensity { get; set; }
        public float Attenuation { get; set; }
        public bool Encendida { get; set; }
        public float SpecularEx { get; set; }
        public int Duracion;
        public TgcScene linterna;
        private float movimientoLinterna;
        public Linterna(Vector3 Direccion1, Vector3 Posicion1)
        {
            this.init();
            this.Direccion = Direccion1;
            this.Posicion = Posicion1;
            this.SpotAngle = 20f;
            this.SpotExponent = 40f;
            this.Intensity = 20;
            this.Encendida = true;
            this.Attenuation = 0.1f;
            this.SpecularEx = 9f;
        }
        public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            var loader = new TgcSceneLoader();
            linterna = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\Linterna-TgcScene.xml");
            linterna.Meshes[0].Scale = new Vector3(0.4f, 0.4f, 0.4f);
            linterna.Meshes[0].Position = Posicion + new Vector3(-30f, -15f, 60f);
            linterna.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-350f), Geometry.DegreeToRadian(180f), Geometry.DegreeToRadian(-270f));
        }
        public void moverLinterna(float elapsedTime)
        {
            movimientoLinterna += elapsedTime;
            var random = FastMath.Cos(6 * movimientoLinterna);
            var random2 = FastMath.Cos(12 * movimientoLinterna);
            linterna.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-350f + random2), Geometry.DegreeToRadian(180f + random), Geometry.DegreeToRadian(-270f));
        }
        public void render()
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            GuiController.Instance.D3dDevice.Transform.View = Matrix.Identity;
            linterna.renderAll();
            GuiController.Instance.D3dDevice.Transform.View = matrizView;
        }
        public void CambiarEstadoLuz()
        {
            this.Encendida = !this.Encendida;
        }
    }
}

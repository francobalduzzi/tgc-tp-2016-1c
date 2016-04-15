using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.MiGrupo
{
    class vela
    {
        private TgcScene vela1;
        private float movimientoVela = 0f;
        public void init(Camara camara)
        {
            var loader = new TgcSceneLoader();
            vela1 = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Objetos\\Vela\\Vela-TgcScene.xml");
            vela1.Meshes[0].Scale = new Vector3(2f, 2f, 2f);
            vela1.Meshes[0].Position = GuiController.Instance.CurrentCamera.getPosition() + new Vector3(-30f, -20f, 60f);
            vela1.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-5f), Geometry.DegreeToRadian(-14f), Geometry.DegreeToRadian(0f));
        }
        public void moverVela(float elapsedTime, Camara camara)
        {
            movimientoVela += elapsedTime;
            var random = FastMath.Cos(6 * movimientoVela);
            var random2 = FastMath.Cos(12 * movimientoVela);
            vela1.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-5f + random2), Geometry.DegreeToRadian(-14f + random), Geometry.DegreeToRadian(0f));

        }
        public void render()
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            GuiController.Instance.D3dDevice.Transform.View = Matrix.Identity;
            vela1.renderAll();
            GuiController.Instance.D3dDevice.Transform.View = matrizView;
        }
    }
}

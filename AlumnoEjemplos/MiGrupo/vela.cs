using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils._2D;
using System.Drawing;

namespace AlumnoEjemplos.MiGrupo
{
    class Vela : TipoIluminador
    {
        private TgcScene vela1;
        private float movimientoVela = 0f;
        override public void init()
        {
            this.Encendida = true;
            var loader = new TgcSceneLoader();
            vela1 = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Objetos\\Vela\\Vela-TgcScene.xml");
            vela1.Meshes[0].Scale = new Vector3(2f, 2f, 2f);
            vela1.Meshes[0].Position = GuiController.Instance.CurrentCamera.getPosition() + new Vector3(-30f, -20f, 60f);
            vela1.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-5f), Geometry.DegreeToRadian(-14f), Geometry.DegreeToRadian(0f));
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
        }
        override public void CambiarEstadoLuz()
        {
            this.Encendida = !this.Encendida;
        }
        override public void actualizarEscenario(TgcScene escena, Camara camara)
        {

            Effect currentShader;
            if (this.Encendida)
            {
                //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con SpotLight
                currentShader = GuiController.Instance.Shaders.TgcMeshPointLightShader;
            }
            else
            {
                //Sin luz: Restaurar shader default
                currentShader = GuiController.Instance.Shaders.TgcMeshSpotLightShader;
            }

            //Aplicar a cada mesh el shader actual
            foreach (TgcMesh mesh in escena.Meshes)
            {
                mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
            }


            //Renderizar meshes
            foreach (TgcMesh mesh in escena.Meshes)
            {
                if (this.Encendida)
                {
                    //Cargar variables shader de la luz
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camara.getPosition()));
                    mesh.Effect.SetValue("lightIntensity", (float)30);
                    mesh.Effect.SetValue("lightAttenuation", (float)0.3);

                    //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", (float)9f);
                }
            }

        }

    }
}

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using System.Drawing;

namespace AlumnoEjemplos.MiGrupo
{
    public class Linterna : TipoIluminador
    {
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
        override public void mover(float elapsedTime)
        {
            movimientoLinterna += elapsedTime;
            var random = FastMath.Cos(6 * movimientoLinterna);
            var random2 = FastMath.Cos(12 * movimientoLinterna);
            linterna.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-350f + random2), Geometry.DegreeToRadian(180f + random), Geometry.DegreeToRadian(-270f));
        }
        override public void init()
        {
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;
            var loader = new TgcSceneLoader();
            linterna = loader.loadSceneFromFile(alumnoMediaFolder + "MiGrupo\\Linterna-TgcScene.xml");
            linterna.Meshes[0].Scale = new Vector3(0.4f, 0.4f, 0.4f);
            linterna.Meshes[0].Position = Posicion + new Vector3(-30f, -15f, 60f);
            linterna.Meshes[0].Rotation = new Vector3(Geometry.DegreeToRadian(-350f), Geometry.DegreeToRadian(180f), Geometry.DegreeToRadian(-270f));
        }
        override public void render()
        {
            var matrizView = GuiController.Instance.D3dDevice.Transform.View;
            GuiController.Instance.D3dDevice.Transform.View = Matrix.Identity;
            linterna.renderAll();
            GuiController.Instance.D3dDevice.Transform.View = matrizView;
        }
        override public void CambiarEstadoLuz()
        {
            this.Encendida = !this.Encendida;
        }







        override public void actualizarEscenario(TgcScene escena, Camara camara)
        {
            this.Posicion = camara.getPosition();
            this.Direccion = camara.getLookAt();
            Vector3 lightDir;
            lightDir = this.Direccion - this.Posicion;
            lightDir.Normalize();
            Effect currentShader;
            if (this.Encendida)
            {
                //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con SpotLight
                currentShader = GuiController.Instance.Shaders.TgcMeshSpotLightShader;
            }
            else
            {
                //Sin luz: Restaurar shader default
                currentShader = GuiController.Instance.Shaders.TgcMeshPointLightShader;
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
                if (this.Encendida)
                {
                    //Cargar variables shader de la luz
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(this.Posicion));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(this.Posicion));
                    mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat4Array(lightDir));
                    mesh.Effect.SetValue("lightIntensity", this.Intensity);
                    mesh.Effect.SetValue("lightAttenuation", this.Attenuation);
                    mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(this.SpotAngle));
                    mesh.Effect.SetValue("spotLightExponent", this.SpotExponent);

                    //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", this.SpecularEx);
                }

                //No renderizar aca porque hace una caja negra loca
            }
        }
    }
}

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using System.Drawing;
using System.IO;

namespace AlumnoEjemplos.CucarachaJugosita
{
    public class Enemigo2 : Enemigo
    {
        public Enemigo2()
        {

            //Path para carpeta de texturas de la malla
            mediaPath = alumnoMediaFolder + "CucarachaJugosita\\Media\\SkeletalAnimations\\BasicHuman\\";

            //Cargar dinamicamente todos los Mesh animados que haya en el directorio
            DirectoryInfo dir = new DirectoryInfo(mediaPath);
            FileInfo[] meshFiles = dir.GetFiles("*-TgcSkeletalMesh.xml", SearchOption.TopDirectoryOnly);
            string[] meshList = new string[meshFiles.Length];
            for (int i = 0; i < meshFiles.Length; i++)
            {
                string name = meshFiles[i].Name.Replace("-TgcSkeletalMesh.xml", "");
                meshList[i] = name;
            }

            //Cargar dinamicamente todas las animaciones que haya en el directorio "Animations"
            DirectoryInfo dirAnim = new DirectoryInfo(mediaPath + "Animations\\");
            FileInfo[] animFiles = dirAnim.GetFiles("*-TgcSkeletalAnim.xml", SearchOption.TopDirectoryOnly);
            animationList = new string[animFiles.Length];
            animationsPath = new string[animFiles.Length];

            for (int i = 0; i < animFiles.Length; i++)
            {
                string name = animFiles[i].Name.Replace("-TgcSkeletalAnim.xml", "");
                animationList[i] = name;
                animationsPath[i] = animFiles[i].FullName;
            }
            //Cargar mesh inicial

            animationList = new string[animFiles.Length];
            animationsPath = new string[animFiles.Length];
            for (int i = 0; i < animFiles.Length; i++)
            {
                string name = animFiles[i].Name.Replace("-TgcSkeletalAnim.xml", "");
                animationList[i] = name;
                animationsPath[i] = animFiles[i].FullName;
            }

            animationList = new string[]{
                "StandBy",
                "Walk",
                "Run",
                "Jump",
                "LowKick",
                "Push",
                "Patear",
                "Pegar",
                "Arrojar",
                "AASD",
            };


            selectedAnim = animationList[0];

            TgcSkeletalLoader loader = new TgcSkeletalLoader();
            mesh = loader.loadMeshAndAnimationsFromFile(mediaPath + "Quake2Scout-TgcSkeletalMesh.xml", mediaPath, animationsPath);
            mesh.Scale = new Vector3(1.3f,1.3f,1.3f);
            //Crear esqueleto a modo Debug
            mesh.buildSkletonMesh();

            //Elegir animacion inicial
            mesh.playAnimation(selectedAnim, true);

            bounding = new TgcBox();
            bounding = TgcBox.fromSize(mesh.Position, new Vector3(100f, 100f, 300f));
            bounding.move(new Vector3(15, 0, -170));
        }

    }
}

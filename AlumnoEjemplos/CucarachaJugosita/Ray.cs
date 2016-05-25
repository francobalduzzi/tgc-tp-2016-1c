using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class Ray
    {
        Vector3 origen;
        Vector3 direccion;

        public Ray(Vector3 origenN, Vector3 destinoN)
        {
            origen = origenN;
            direccion = destinoN - origenN;
        }

         public Boolean intersectAABB(TgcBoundingBox bounding)
         {
             Vector3 min = bounding.PMin;
             Vector3 max = bounding.PMax;
             float t;

             float tmin = (min.X - origen.X) / direccion.X;
             float tmax = (max.X - origen.X) / direccion.X;
             if(tmin > tmax)//swap
             {
                 float aux = tmin;
                 tmin = tmax;
                 tmax = aux;
             }
             float tymin = (min.Y - origen.Y) / direccion.Y;
             float tymax = (max.Y - origen.Y) / direccion.Y;

             if(tymin > tymax)
             {
                 float aux = tymin;
                 tymin = tymax;
                 tymax = aux;
             }
             if ((tmin > tymax) || (tymin > tmax))
             {
                 return false;
             }

             if (tymin > tmin)
             {
                 tmin = tymin;
             }

             if (tymax < tmax)
             {
                 tmax = tymax;
             }

             float tzmin = (min.Z - origen.Z) / direccion.Z;
             float tzmax = (max.Z - origen.Z) / direccion.Z;
             if(tzmin > tzmax)
             {
                 float aux = tzmin;
                 tzmin = tzmax;
                 tzmax = aux;
             }

             if ((tmin > tzmax) || (tzmin > tmax))
             {
                 return false;
             }

             if (tzmin > tmin)
             {
                 tmin = tzmin;
             }

             if (tzmax < tmax)
             {
                 tmax = tzmax;
             }

            return ((tmin < 1) && (tmax > 0));


        }



        public Vector3 calculoNormalPared(TgcMesh mesh)
        {
            Vector3 punto1 = mesh.getVertexPositions()[0];
            Vector3 punto2 = mesh.getVertexPositions()[1];
            Vector3 punto3 = mesh.getVertexPositions()[2];
            Vector3 vectorDir1 = punto2 - punto1;
            Vector3 vectorDir2 = punto3 - punto1;
            Vector3 normalPared = Vector3.Cross(vectorDir1, vectorDir2);
            return normalPared;
        }
        public Boolean interseccionRayoPlano(TgcMesh mesh, out Vector3 salida)
        {
            Vector3 normalPared = calculoNormalPared(mesh);
            Vector3 puntoPared = mesh.getVertexPositions()[0];
            Vector3 calculo1 = puntoPared - origen;
            Vector3 calculo2 = direccion;
            if (Vector3.Dot(normalPared, calculo2) == 0)
            {
                salida = new Vector3(0, 0, 0);
                return false;
            }
            else
            {
                float r1 = Vector3.Dot(normalPared, calculo1) / Vector3.Dot(normalPared, calculo2);
                if (r1 >= 0 && r1 <= 1)
                {
                    salida = origen + r1 * calculo2;
                    return true;
                }
                else
                {
                    salida = new Vector3(0, 0, 0);
                    return false;
                }
            }
        }



    }
}

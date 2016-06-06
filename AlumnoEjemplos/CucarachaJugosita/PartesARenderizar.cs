using Microsoft.DirectX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CucarachaJugosita
{
    class PartesARenderizar
    {
        public TgcBox caja1;
        public TgcBox caja2;
        public TgcBox caja3;
        public TgcBox caja4;
        public TgcBox caja5;
        public TgcBox caja6;
        public TgcBox caja7;
        public TgcBox caja8;
        public TgcBox caja9;
        public TgcBox caja10;
        public TgcBox caja11;
        public TgcBox caja12;
        public TgcBox caja13;
        public TgcBox caja14;
        ArrayList listaCaja1;
        ArrayList listaCaja2;
        ArrayList listaCaja3;
        ArrayList listaCaja4;
        ArrayList listaCaja5;
        ArrayList listaCaja6;
        ArrayList listaCaja7;
        ArrayList listaCaja8;
        ArrayList listaCaja9;
        ArrayList listaCaja10;
        ArrayList listaCaja11;
        ArrayList listaCaja12;
        ArrayList listaCaja13;
        ArrayList listaCaja14;
        TgcScene escena;
        TgcObb obb;
        Camara camara;

        public PartesARenderizar(TgcScene mapa,Camara camarita)
        {
            escena = mapa;
            camara = camarita;

            listaCaja1 = new ArrayList();
            listaCaja2 = new ArrayList();
            listaCaja3 = new ArrayList();
            listaCaja4 = new ArrayList();
            listaCaja5 = new ArrayList();
            listaCaja6 = new ArrayList();
            listaCaja7 = new ArrayList();
            listaCaja8 = new ArrayList();
            listaCaja9 = new ArrayList();
            listaCaja10 = new ArrayList();
            listaCaja11 = new ArrayList();
            listaCaja12 = new ArrayList();
            listaCaja13 = new ArrayList();
            listaCaja14 = new ArrayList();


            caja1 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(750, 750, 600), Color.Blue);
            caja1.move(new Vector3(375, 370, 300));
            caja2 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(500, 750, 600), Color.Blue);
            caja2.Position = new Vector3(750 + 250, 370, 300);
            caja3 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(750, 750, 650), Color.Blue);
            caja3.Position = new Vector3(375, 370, 600 + 325);
            caja4 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(1300, 750, 650), Color.Blue);
            caja4.Position = new Vector3(750 + 650, 370, 600 + 325);
            caja5 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(700, 750, 600), Color.Blue);
            caja5.Position = new Vector3(1350 + 350, 370, 300);
            caja6 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(700, 750, 500), Color.Blue);
            caja6.Position = new Vector3(2050 + 350, 370, 250);
            caja7 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(800, 750, 500), Color.Blue);
            caja7.Position = new Vector3(2750 + 400, 370, 250);
            caja8 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(550, 750, 600), Color.Blue);
            caja8.Position = new Vector3(545, 370, 1250 + 300);
            caja9 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(1230, 750, 900), Color.Blue);
            caja9.Position = new Vector3(820 + 615, 370, 1250 + 450);
            caja10 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(850, 750, 650), Color.Blue);
            caja10.Position = new Vector3(2050 + 425, 370, 900 + 325);
            caja11 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(1300, 750, 850), Color.Blue);
            caja11.Position = new Vector3(2050 + 650, 370, 1550 + 425);
            caja12 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(800, 750, 1050), Color.Blue);
            caja12.Position = new Vector3(2900 + 400, 370, 500 + 525);
            caja13 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(1200, 750, 1400), Color.Blue);
            caja13.Position = new Vector3(3700 + 600, 370, 500 + 700);
            caja14 = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(850, 750, 400), Color.Blue);
            caja14.Position = new Vector3(2050 + 425, 370, 500 + 200);

            this.init();            
        }

        private void init()
        {
            foreach (TgcMesh mesh in escena.Meshes) 
            {
                obb = TgcObb.computeFromAABB(mesh.BoundingBox);
                if (TgcCollisionUtils.testObbAABB(obb, caja1.BoundingBox))
                {
                    listaCaja1.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja2.BoundingBox))
                {
                    listaCaja2.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja2.BoundingBox))
                {
                    listaCaja2.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja3.BoundingBox))
                {
                    listaCaja3.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja4.BoundingBox))
                {
                    listaCaja4.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja5.BoundingBox))
                {
                    listaCaja5.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja6.BoundingBox))
                {
                    listaCaja6.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja7.BoundingBox))
                {
                    listaCaja7.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja8.BoundingBox))
                {
                    listaCaja8.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja9.BoundingBox))
                {
                    listaCaja9.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja10.BoundingBox))
                {
                    listaCaja10.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja11.BoundingBox))
                {
                    listaCaja11.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja12.BoundingBox))
                {
                    listaCaja12.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja13.BoundingBox))
                {
                    listaCaja13.Add(mesh);
                }
                if (TgcCollisionUtils.testObbAABB(obb, caja14.BoundingBox))
                {
                    listaCaja14.Add(mesh);
                }
            }
        }

        public ArrayList aRenderizar()
        {
            ArrayList listaADevolver1 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja1.BoundingBox))
            {
                this.addAll(listaCaja1, listaADevolver1);
                this.addAll(listaCaja2, listaADevolver1);
                this.addAll(listaCaja3, listaADevolver1);
                this.addAll(listaCaja8, listaADevolver1);
                return listaADevolver1;
            }

            ArrayList listaADevolver2 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja2.BoundingBox))
            {
                this.addAll(listaCaja2, listaADevolver2);
                this.addAll(listaCaja1, listaADevolver2);
                this.addAll(listaCaja4, listaADevolver2);
                return listaADevolver2;
            }

            ArrayList listaADevolver3 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja3.BoundingBox))
            {
                this.addAll(listaCaja3,listaADevolver3);
                this.addAll(listaCaja1, listaADevolver3);
                this.addAll(listaCaja8,listaADevolver3);
                this.addAll(listaCaja4, listaADevolver3);
                this.addAll(listaCaja10, listaADevolver3);
                return listaADevolver3;
            }

            ArrayList listaADevolver4 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja4.BoundingBox))
            {
                this.addAll(listaCaja4,listaADevolver4);
                this.addAll(listaCaja3, listaADevolver4);
                this.addAll(listaCaja2, listaADevolver4);
                this.addAll(listaCaja9, listaADevolver4);
                this.addAll(listaCaja5, listaADevolver4);
                this.addAll(listaCaja10, listaADevolver4);
                this.addAll(listaCaja12, listaADevolver4);
                return listaADevolver4;
            }

            ArrayList listaADevolver5 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja5.BoundingBox))
            {
                this.addAll(listaCaja5, listaADevolver5);
                this.addAll(listaCaja4, listaADevolver5);
                this.addAll(listaCaja6, listaADevolver5);
                return listaADevolver5;
            }

            ArrayList listaADevolver6 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja6.BoundingBox))
            {
                this.addAll(listaCaja6, listaADevolver6);
                this.addAll(listaCaja5, listaADevolver6);
                this.addAll(listaCaja7, listaADevolver6);
                return listaADevolver6;
            }

            ArrayList listaADevolver7 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja7.BoundingBox))
            {
                this.addAll(listaCaja7, listaADevolver7);
                this.addAll(listaCaja6, listaADevolver7);
                this.addAll(listaCaja12, listaADevolver7);
                return listaADevolver7;
            }

            ArrayList listaADevolver8 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja8.BoundingBox))
            {
                this.addAll(listaCaja8, listaADevolver8);
                this.addAll(listaCaja3, listaADevolver8);
                this.addAll(listaCaja9, listaADevolver8);
                this.addAll(listaCaja1, listaADevolver8);
                return listaADevolver8;
            }

            ArrayList listaADevolver9 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja9.BoundingBox))
            {
                this.addAll(listaCaja9, listaADevolver9);
                this.addAll(listaCaja8, listaADevolver9);
                this.addAll(listaCaja4, listaADevolver9);
                this.addAll(listaCaja10, listaADevolver9);
                this.addAll(listaCaja11, listaADevolver9);
                return listaADevolver9;
            }

            ArrayList listaADevolver10 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja10.BoundingBox))
            {
                this.addAll(listaCaja10, listaADevolver10);
                this.addAll(listaCaja4, listaADevolver10);
                this.addAll(listaCaja14, listaADevolver10);
                this.addAll(listaCaja11, listaADevolver10);
                this.addAll(listaCaja12, listaADevolver10);
                this.addAll(listaCaja13, listaADevolver10);
                return listaADevolver10;
            }

            ArrayList listaADevolver11 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja11.BoundingBox))
            {
                this.addAll(listaCaja11, listaADevolver11);
                this.addAll(listaCaja10, listaADevolver11);
                this.addAll(listaCaja9, listaADevolver11);
                this.addAll(listaCaja12, listaADevolver11);
                return listaADevolver11;
            }

            ArrayList listaADevolver12 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja12.BoundingBox))
            {
                this.addAll(listaCaja12, listaADevolver12);
                this.addAll(listaCaja10, listaADevolver12);
                this.addAll(listaCaja7, listaADevolver12);
                this.addAll(listaCaja13, listaADevolver12);
                this.addAll(listaCaja11, listaADevolver12);
                return listaADevolver12;
            }

            ArrayList listaADevolver13 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja13.BoundingBox))
            {
                this.addAll(listaCaja13, listaADevolver13);
                this.addAll(listaCaja12, listaADevolver13);
                this.addAll(listaCaja10, listaADevolver13);
                return listaADevolver13;
            }

            ArrayList listaADevolver14 = new ArrayList();
            if (TgcCollisionUtils.testSphereAABB(camara.camaraColision, caja14.BoundingBox))
            {
                this.addAll(listaCaja14, listaADevolver14);
                this.addAll(listaCaja10, listaADevolver14);
                return listaADevolver14;
            }
            return listaCaja14;
        }

        public void addAll(ArrayList desde, ArrayList hasta)
        {
            foreach(TgcMesh mesh in desde)
            {
                hasta.Add(mesh);
            }
        }
    }
}

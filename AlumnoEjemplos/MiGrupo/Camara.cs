﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.Input;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Camara en primera persona personalizada para niveles de Quake 3.
    /// Evita utilizar senos y cosenos
    /// 
    /// Autor: Martin Giachetti
    /// 
    /// Adaptacion : Ezequiel Kogut
    /// </summary>
    /// 
    public class Camara : TgcCamera
    {

        /*
         * Esta Camara es un prototipo. Esta pensada para no utilizar senos y cosenos en las rotaciones.
         * Se utiliza una camara que se desplaza sobre las caras de un cubo sin techo, ni piso. 
         * La teoria es la siguiente: La direccion donde mira la camara esta formado por dos puntos, el ojo y el target.
         * Si el ojo es el centro del cubo y el target es un punto que se desplaza por las caras del cubo.
         * Entonces se puede cambiar el angulo de la direccion desplazando proporcionalmente a la cantidad de grados el punto 
         * target sobre las caras del cubo.
         */
        public Vector3 eye = new Vector3();
        public Vector3 target = new Vector3();
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        private Matrix viewMatrix = Matrix.Identity;
        private Vector3 forwardDirection = new Vector3();
        private Vector3 sideDirection = new Vector3();
        private bool lockCam = false;
        protected Point mouseCenter;
        private float movementSpeed;
        private float rotationSpeed;
        private float jumpSpeed;
        private double latitud;
        private double longitud;
        public static Vector3 movimiento;
        public static bool moving;
        private TgcScene escena;
        private Vector3 eyeAux;
        private Vector3 targetAux;
        TgcBoundingSphere camaraColision;


        public Camara()
        {
            Control focusWindows = GuiController.Instance.D3dDevice.CreationParameters.FocusWindow;
            mouseCenter = focusWindows.PointToScreen(
                new Point(
                    focusWindows.Width / 2,
                    focusWindows.Height / 2)
                    );
        }

        public void SetEscena(TgcScene unaEscena)
        {
            escena = unaEscena;
        }
        ~Camara()
        {
            LockCam = false;
        }


        public Vector3 ForwardDirection
        {
            get { return forwardDirection; }
        }

        public Vector3 SideDirection
        {
            get { return sideDirection; }
        }

        public bool LockCam
        {
            get { return lockCam; }
            set
            {
                if (!lockCam && value)
                {
                    Cursor.Position = mouseCenter;

                    Cursor.Hide();
                }
                if (lockCam && !value)
                    Cursor.Show();
                lockCam = value;
            }
        }

        public float MovementSpeed
        {
            get { return movementSpeed; }
            set { movementSpeed = value; }
        }

        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        public float JumpSpeed
        {
            get { return jumpSpeed; }
            set { jumpSpeed = value; }
        }

        public bool Enable
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void recalcularDirecciones()
        {
            Vector3 forward = target - eye;
            forward.Y = 0;
            forward.Normalize();

            forwardDirection = forward;
            sideDirection.X = forward.Z;
            sideDirection.Z = -forward.X;
        }

        #region Miembros de TgcCamera

        public Vector3 getPosition()
        {
            return eye;
        }

        public Vector3 getLookAt()
        {
            return target;
        }

        public void updateCamera()
         {
            float elapsedTime = GuiController.Instance.ElapsedTime;
            bool colision = ChequearColisiones(); //Detectamos cplisiones y guardamos

            //Forward
            if(!colision)
            {
                eyeAux = eye;
                targetAux = target;
            }
            if (GuiController.Instance.D3dInput.keyDown(Key.W) && !colision)
             {
                 Vector3 v = moveForward(MovementSpeed * elapsedTime);
                 movimiento = v;
                 moving = true;
             }

             //Backward
             if (GuiController.Instance.D3dInput.keyDown(Key.S) && !colision)
             {
                 Vector3 v = moveForward(-MovementSpeed * elapsedTime);
                 movimiento = v;
                 moving = true;
            }

            //Strafe right
            if (GuiController.Instance.D3dInput.keyDown(Key.D) && !colision)
             {
                 Vector3 v = moveSide(MovementSpeed * elapsedTime);
                 movimiento = v;
                 moving = true;
            }

            //Strafe left
            if (GuiController.Instance.D3dInput.keyDown(Key.A) && !colision)
             {
                 Vector3 v = moveSide(-MovementSpeed * elapsedTime);
                 movimiento = v;
                 moving = true;
            }

            //Jump
            if (GuiController.Instance.D3dInput.keyDown(Key.Space) && !colision)
             {
                 Vector3 v = moveUp(JumpSpeed * elapsedTime);
                 movimiento = v;
                 moving = true;
            }

            //Crouch
            if (GuiController.Instance.D3dInput.keyDown(Key.LeftControl) && !colision)
             {
                 Vector3 v = moveUp(-JumpSpeed * elapsedTime);
                 movimiento = v;
                 moving = true;
            }
            if (colision)
            {
                eye = eyeAux;
                target = targetAux;
            }
            if (GuiController.Instance.D3dInput.keyPressed(Key.L))
             {
                 LockCam = !LockCam;

             }

             //Solo rotar si se esta aprentando el boton izq del mouse
            //if (lockCam || GuiController.Instance.D3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            // {
                 rotate(-GuiController.Instance.D3dInput.XposRelative * rotationSpeed,
                        -GuiController.Instance.D3dInput.YposRelative * rotationSpeed);

            // }


             if (lockCam)
                 Cursor.Position = mouseCenter;

             viewMatrix = Matrix.LookAtLH(eye, target, up);

             updateViewMatrix(GuiController.Instance.D3dDevice);


         }
        public bool ChequearColisiones()
        {
            camaraColision = new TgcBoundingSphere(eye, 5f);
            foreach (TgcMesh mesh in escena.Meshes)
            {
                if (TgcCollisionUtils.testSphereAABB(camaraColision, mesh.BoundingBox))
                {
                    return true;
                }
            }
            return false;
        }
        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            d3dDevice.Transform.View = viewMatrix;
        }

        #endregion


        public void move(Vector3 v)
        {

            eye.Add(v);
            target.Add(v);
        }

        public void setCamera(Vector3 eye, Vector3 target)
        {
            this.eye = eye;
            this.target = target;

            Vector3 dir = target - eye;
            double ang = (180 * Math.Atan(dir.Z / dir.X)) / Math.PI;

            //calculo el angulo correspondiente a la latitud y longitud.
            if (dir.X > 0)
            {
                if (ang < 0)
                {
                    latitud = ang + 360;
                }
                else
                {
                    latitud = ang;
                }
            }
            else
            {
                latitud = ang + 180;
            }

            latitud += 45;
            longitud = (180 * Math.Atan(dir.Y / Math.Sqrt(dir.X * dir.X + dir.Z * dir.Z))) / Math.PI + 45;

            rotate(0, 0);

            recalcularDirecciones();

        }

        public Vector3 moveForward(float movimiento)
        {
            Vector3 v = ForwardDirection * movimiento;
            move(v);
            return v;
        }

        public Vector3 moveSide(float movimiento)
        {
            Vector3 v = SideDirection * movimiento;
            move(v);
            return v;
        }

        public Vector3 moveUp(float movimiento)
        {
            move(up * movimiento);
            Vector3 v = up * movimiento;
            return v;
        }

        public void rotateY(float movimiento)
        {
            //rotate(movimiento, 0.0f, 0.0f);

            rotate(movimiento, 0);
        }

        public void rotateXZ(float movimiento)
        {
            rotate(0, movimiento);
        }

        public void rotate(float lat, float lon)
        {
            latitud += lat;
            if (latitud >= 360)
                latitud -= 360;

            if (latitud < 0)
                latitud += 360;


            longitud += lon;
            if (longitud > 90)
                longitud = 90;
            if (longitud < 0)
                longitud = 0;

            recalcularTarget();
        }

        private const float LADO_CUBO = 1.0f;
        private const float MEDIO_LADO_CUBO = LADO_CUBO * 0.5f;
        private float STEP_ANGULO = LADO_CUBO / 90;
        private void recalcularTarget()
        {
            float x = 0;
            float y = 0;
            float z = 0;

            if (latitud < 180)
            {
                if (latitud < 90)
                {
                    z = (float)latitud * STEP_ANGULO;
                }
                else
                {
                    z = LADO_CUBO;
                    x = (float)(latitud - 90) * STEP_ANGULO;
                }
                z = z - MEDIO_LADO_CUBO;
                x = MEDIO_LADO_CUBO - x;
            }
            else
            {
                if (latitud < 270)
                {
                    z = (float)(latitud - 180) * STEP_ANGULO;
                }
                else
                {
                    z = LADO_CUBO;
                    x = (float)(latitud - 270) * STEP_ANGULO;
                }
                z = MEDIO_LADO_CUBO - z;
                x = x - MEDIO_LADO_CUBO;
            }

            y = (float)longitud * STEP_ANGULO - MEDIO_LADO_CUBO;



            target = eye + new Vector3(x, y, z);

            recalcularDirecciones();
        }

    }
}

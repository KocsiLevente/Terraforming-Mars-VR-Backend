using System;

namespace TerraformingMarsBackend.Models
{
    public class GameObject
    {
        public float posX;
        public float PosX { get { return posX; } set { posX = value; } }

        public float posY;
        public float PosY { get { return posY; } set { posY = value; } }

        public float posZ;
        public float PosZ { get { return posZ; } set { posZ = value; } }

        public float rotX;
        public float RotX { get { return rotX; } set { rotX = value; } }

        public float rotY;
        public float RotY { get { return rotY; } set { rotY = value; } }

        public float rotZ;
        public float RotZ { get { return rotZ; } set { rotZ = value; } }

        public float scaleX;
        public float ScaleX { get { return scaleX; } set { scaleX = value; } }

        public float scaleY;
        public float ScaleY { get { return scaleY; } set { scaleY = value; } }

        public float scaleZ;
        public float ScaleZ { get { return scaleZ; } set { scaleZ = value; } }

        public GameObject(float posX, float posY, float posZ, float rotX = 0.0f, float rotY = 0.0f, float rotZ = 0.0f, float scaleX = 1.0f, float scaleY = 1.0f, float scaleZ = 1.0f)
        {
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            RotX = rotX;
            RotY = rotY;
            RotZ = rotZ;
            ScaleX = scaleX;
            ScaleY = scaleY;
            ScaleZ = scaleZ;
        }
    }
}

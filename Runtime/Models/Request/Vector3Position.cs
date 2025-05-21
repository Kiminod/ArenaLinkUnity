using UnityEngine;

namespace ArenaLink.Models.Request
{
    public class Vector3Position
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        
        public Vector3Position() { }
        
        public Vector3Position(Vector3 vector) 
        {
            X = vector.x;
            Y = vector.y;
            Z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
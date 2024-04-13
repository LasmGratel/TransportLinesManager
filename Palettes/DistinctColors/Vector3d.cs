using System;
using UnityEngine;

namespace TransportLinesManager.Palettes.DistinctColors
{
    public readonly struct Vector3d
    {
        public readonly double X, Y, Z;

        public Vector3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3d(in Vector3d vec)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
        }

        public Vector3d(double[] array)
        {
            if (array.Length != 3) throw new ArgumentException("Array length out of bounds");
            X = array[0];
            Y = array[1];
            Z = array[2];
        }

        public void Deconstruct(out double x, out double y, out double z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public static Vector3d operator +(Vector3d a) => a;
        public static Vector3d operator -(Vector3d a) => new Vector3d(-a.X, -a.Y, -a.Z);

        public static Vector3d operator +(Vector3d a, Vector3d b)
            => new Vector3d(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3d operator -(Vector3d a, Vector3d b)
            => new Vector3d(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3d operator *(Vector3d a, double b)
            => new Vector3d(a.X * b, a.Y * b, a.Z * b);

        public static Vector3d operator *(Vector3d a, Vector3d b)
            => new Vector3d(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vector3d operator /(Vector3d a, Vector3d b)
            => new Vector3d(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static implicit operator Color32(Vector3d vec) => new Color32((byte)vec.X, (byte)vec.Y, (byte)vec.Z, 255);
        public static implicit operator Vector3d(Color32 rgb) => new Vector3d(rgb.r, rgb.g, rgb.b);


        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}

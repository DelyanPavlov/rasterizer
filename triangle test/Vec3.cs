using System;
using System.Collections.Generic;
using System.Text;

namespace triangle_test
{
    internal class Vec3
    {
        public double X, Y, Z;

        public Vec3(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Vec3(double X)
        {
            this.X = X;
            this.Y = X;
            this.Z = X;
        }

        public Vec3(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Vec3(float X)
        {
            this.X = X;
            this.Y = X;
            this.Z = X;
        }

        public static Vec3 operator +(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static Vec3 operator -(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vec3 operator -(Vec3 lhs)
        {
            return new Vec3(-lhs.X, -lhs.Y, -lhs.Z);
        }

        public static Vec3 operator *(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z);
        }

        public static Vec3 operator /(Vec3 lhs, Vec3 rhs)
        {
            return new Vec3(lhs.X / rhs.X, lhs.Y / rhs.Y, lhs.Z / rhs.Z);
        }

        public static Vec3 operator *(Vec3 lhs, double rhs)
        {
            return new Vec3(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }

        public static Vec3 operator /(Vec3 lhs, double rhs)
        {
            return new Vec3(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);
        }

        public static Vec3 operator *(Vec3 lhs, float rhs)
        {
            return new Vec3(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }

        public static Vec3 operator /(Vec3 lhs, float rhs)
        {
            return new Vec3(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);
        }

        public static double Dot(Vec3 a, Vec3 b)
        {
            return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
        }

        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            return new Vec3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public static Vec3 Normalize(Vec3 v)
        {
            return v / v.Length();
        }
    }
}
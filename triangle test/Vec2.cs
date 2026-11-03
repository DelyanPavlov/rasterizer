using System;
using System.Collections.Generic;
using System.Text;

namespace triangle_test
{
    internal class Vec2
    {
        public double X, Y;

        public Vec2(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Vec2(double X)
        {
            this.X = X;
            this.Y = X;
        }

        public Vec2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Vec2(float Y)
        {
            this.X = Y;
            this.Y = Y;
        }

        public static Vec2 operator+(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        public static Vec2 operator -(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Vec2 operator -(Vec2 lhs)
        {
            return new Vec2(0 - lhs.X, 0 - lhs.Y);
        }

        public static Vec2 operator *(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.X * rhs.X, lhs.Y * rhs.Y);
        }

        public static Vec2 operator /(Vec2 lhs, Vec2 rhs)
        {
            return new Vec2(lhs.X / rhs.X, lhs.Y / rhs.Y);
        }

        public static Vec2 operator *(Vec2 lhs, double rhs)
        {
            return new Vec2(lhs.X * rhs, lhs.Y * rhs);
        }

        public static Vec2 operator /(Vec2 lhs, double rhs)
        {
            return new Vec2(lhs.X / rhs, lhs.Y / rhs);
        }

        public static Vec2 operator *(Vec2 lhs, float rhs)
        {
            return new Vec2(lhs.X * rhs, lhs.Y * rhs);
        }

        public static Vec2 operator /(Vec2 lhs, float rhs)
        {
            return new Vec2(lhs.X / rhs, lhs.Y / rhs);
        }

        public static double Dot(Vec2 a, Vec2 b)
        {
            return (a.X * b.X) + (a.Y * b.Y); 
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public static Vec2 Normalize(Vec2 v)
        {
            return v / v.Length();
        }
    }
}

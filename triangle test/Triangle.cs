using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace triangle_test
{
    internal class Triangle
    {
        public Vertex[] vertices = new Vertex[3];
        public Vec2[] UVs = new Vec2[3];
        public Vec3 normals = new Vec3(1);
        public Vec2[] midPoints = new Vec2[3];

        public bool isBackwards;
        public Triangle(Vertex v1, Vertex v2, Vertex v3, Vec2 UV1, Vec2 UV2, Vec2 UV3, Vec3 norm1, Vec3 norm2, Vec3 norm3)
        {
            this.vertices = [v1, v2, v3];
            this.UVs = [UV1, UV2, UV3];
            this.normals = Vec3.Cross(norm2 - norm1, norm3 - norm1);
        }

        public double[] CalcPixelColor(Vec2 p)
        {
            p = new Vec2(p.X / Camera.screenDim.X, p.Y / Camera.screenDim.Y);
            Vec2 a = vertices[0].position, b = vertices[1].position, c = vertices[2].position;
            Vec2 v0 = b - a, v1 = c - a, v2 = p - a;

            double d00 = Vec2.Dot(v0, v0);
            double d01 = Vec2.Dot(v0, v1);
            double d11 = Vec2.Dot(v1, v1);
            double d20 = Vec2.Dot(v2, v0);
            double d21 = Vec2.Dot(v2, v1);
            double denom = d00 * d11 - d01 * d01;

            double v = (d11 * d20 - d01 * d21) / denom;
            double w = (d00 * d21 - d01 * d20) / denom;
            double u = 1.0 - v - w;

            return [v, w, u];
        }
    }
}

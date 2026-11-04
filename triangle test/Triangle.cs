using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace triangle_test
{
    internal class Triangle
    {
        public Vertex[] vertices = new Vertex[3];
        public Vec2[] normals = new Vec2[3];
        public Vec2[] midPoints = new Vec2[3];

        public bool isBackwards;

        public Triangle(Vertex[] vertices)
        {
            this.vertices = vertices;
        }

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.vertices = [v1, v2, v3];
        }

        public void CalcTriNorms()
        {
            double area = 0;
            for (int i = 0; i < 3; i++)
            {
                Vec2 a = vertices[i].position;
                Vec2 b = vertices[(i + 1) % 3].position;
                Vec2 opposite = vertices[(i + 2) % 3].position;

                midPoints[i] = (a + b) / 2;
                Vec2 edge = b - a;
                Vec2 n = Vec2.Normalize(new Vec2(edge.Y, -edge.X));

                area += (a.X - b.X) * (a.Y + b.Y);

                if (Vec2.Dot(n, opposite - midPoints[i]) < 0) n = -n;
                normals[i] = n;
            }
            if(area < 0)
            {
                isBackwards = true;
            }
            else
            {
                isBackwards = false;
            }
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

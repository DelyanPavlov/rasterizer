using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace triangle_test
{
    internal static class Camera
    {
        public static double X = 0, Y = 0, Z = 0;
        public static double t = 1;
        public static Vec2 screenDim = new Vec2(1280, 720);
        static double nearPlane = 0.3;

        public static Triangle CalcScreenCords(Triangle tri)
        {
            for (int i = 0; i < tri.vertices.Length; i++)
            {
                tri.vertices[i].screenX = tri.vertices[i].spaceX * t / tri.vertices[i].spaceZ;
                tri.vertices[i].screenY = tri.vertices[i].spaceY * t / tri.vertices[i].spaceZ;
                tri.vertices[i].position = new Vec2(tri.vertices[i].screenX, tri.vertices[i].screenY);

                if (tri.vertices[i].spaceZ <= nearPlane)
                {
                    tri.isBackwards = true;
                }
            }
            return tri;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace triangle_test
{
    internal class Vertex
    {
        public double screenX, screenY;
        public double SpaceX, spaceY, spaceZ;
        public double u, v;
        public Vec2 position;
        public Vec2 ScreenPosition;
        public Vertex(double screenX, double screenY, double u, double v)
        {
            this.screenX = screenX;
            this.screenY = screenY;
            this.u = u;
            this.v = v;
            position = new Vec2((float)screenX, (float)screenY);
        }

        public void calcScreenPos(int windowW, int windowH)
        {
            ScreenPosition = new Vec2((float)screenX * windowW, (float)screenY * windowH);
        }
    }
}

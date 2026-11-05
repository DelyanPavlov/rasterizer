using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace triangle_test
{
    internal class Vertex
    {
        public double screenX, screenY;
        public double spaceX, spaceY, spaceZ;
        public Vec2 position;
        public Vec2 ScreenPosition;

        public Vertex(double spaceX, double spaceY, double spaceZ)
        {
            this.spaceX = spaceX + Camera.X;
            this.spaceY = spaceY + Camera.Y;
            this.spaceZ = spaceZ + Camera.Z;
        }

        public void calcScreenPos(int windowW, int windowH)
        {
            ScreenPosition = new Vec2((float)screenX * windowW, (float)screenY * windowH);
        }
        public Vertex(Vertex o)
        {
            spaceX = o.spaceX; spaceY = o.spaceY; spaceZ = o.spaceZ;
        }
    }
}

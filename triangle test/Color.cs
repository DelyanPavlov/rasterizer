using System;
using System.Collections.Generic;
using System.Text;

namespace triangle_test
{
    internal class Color
    {
        public int R;
        public int G;
        public int B;

        public Color(int r, int g, int b)
        {
            R = r; 
            G = g;
            B = b;
        }

        public static Color operator +(Color a, Color b)
        {
            return new Color(a.R + b.R, a.G + b.G, a.B + b.B);
        }

        public static Color operator -(Color a, Color b)
        {
            return new Color(a.R - b.R, a.G - b.G, a.B - b.B);
        }

        public static Color operator *(Color a, Color b)
        {
            return new Color((a.R * b.R) % 256, (a.G * b.G) % 256, (a.B * b.B) % 256);
        }

        public static Color operator *(Color a, double b)
        {
            return new Color((int)(a.R * b) % 256, (int)(a.G * b) % 256, (int)(a.B * b) % 256);
        }
    }
}

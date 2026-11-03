using System;
using System.Collections.Generic;
using System.Text;

namespace triangle_test
{
    internal static class Helper
    {
        public static Color[,] LoadTexture(string filePath)
        {
            string[] fullData = File.ReadAllLines(filePath);
            string[][] Data = new string[fullData.Length][];
            for (int i = 0; i < fullData.Length; i++)
            {
                Data[i] = fullData[i].Split(" ");
            }
            int width = int.Parse(Data[1][0]), height = int.Parse(Data[1][1]);
            Color[,] colorData = new Color[width, height];

            for (int i = 3; i < width * height + 3; i++)
            {
                colorData[(i - 3) / width, (i - 3) % width] = new Color(int.Parse(Data[i][0]), int.Parse(Data[i][1]), int.Parse(Data[i][2]));
            }

            return colorData;
        }

        public static Color SampleTexture(Color[,] texture, Triangle tri, double[] uvw)
        {
            double u = uvw[2];
            double v = uvw[0];
            double w = uvw[1];

            int sampleX = (int)Math.Round((tri.vertices[0].u * u + tri.vertices[1].u * v + tri.vertices[2].u * w) * texture.GetLength(0));
            int sampleY = (int)Math.Round((tri.vertices[0].v * u + tri.vertices[1].v * v + tri.vertices[2].v * w) * texture.GetLength(1));

            if (sampleX == texture.GetLength(0)) sampleX--;
            if (sampleY == texture.GetLength(1)) sampleY--;
            if (texture[sampleX, sampleY] == null)
            {

            }
            return texture[sampleX, sampleY];
        }
    }
}

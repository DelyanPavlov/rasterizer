using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
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
                colorData[(i - 3) % width, (i - 3) / width] = new Color(int.Parse(Data[i][0]), int.Parse(Data[i][1]), int.Parse(Data[i][2]));
            }

            return colorData;
        }
        static float F(string s) => float.Parse(s, CultureInfo.InvariantCulture);

        static int Idx(string s, int count)
        {
            int i = int.Parse(s, CultureInfo.InvariantCulture);
            return i > 0 ? i - 1 : count + i;
        }

        public static Triangle[] LoadModel(string filePath)
        {
            var triangles = new List<Triangle>();
            var pos = new List<Vertex>();
            var uv = new List<Vec2>();
            var norm = new List<Vec3>();

            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] p = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (p.Length == 0) continue;

                switch (p[0])
                {
                    case "v":
                        pos.Add(new Vertex(F(p[1]), F(p[2]), F(p[3])));
                        break;
                    case "vt":
                        uv.Add(new Vec2(F(p[1]), F(p[2])));
                        break;
                    case "vn":
                        norm.Add(new Vec3(F(p[1]), F(p[2]), F(p[3])));
                        break;
                    case "f":
                        int n = p.Length - 1;
                        var vi = new int[n];
                        var ti = new int[n];
                        var ni = new int[n];

                        for (int k = 0; k < n; k++)
                        {
                            string[] c = p[k + 1].Split('/');
                            vi[k] = Idx(c[0], pos.Count);
                            ti[k] = c.Length > 1 && c[1] != "" ? Idx(c[1], uv.Count) : -1;
                            ni[k] = c.Length > 2 && c[2] != "" ? Idx(c[2], norm.Count) : -1;
                        }

                        Vec2 UV(int i) => i >= 0 ? uv[i] : new Vec2(0f);
                        Vec3 N(int i) => i >= 0 ? norm[i] : new Vec3(0f);
                        triangles.Add(new Triangle(
                            pos[vi[0]], pos[vi[1]], pos[vi[2]],
                            UV(ti[0]), UV(ti[1]), UV(ti[2]),
                            N(ni[0]), N(ni[1]), N(ni[2])));
                        break;
                }
            }
            return triangles.ToArray();
        }
        public static Color SampleTexture(Color[,] texture, Triangle tri, double[] uvw)
        {
            double u = uvw[2];
            double v = uvw[0];
            double w = uvw[1];
            double[] tempU = { tri.UVs[0].X * u, tri.UVs[1].X * v, tri.UVs[2].X * w };
            double[] tempV = { tri.UVs[0].Y * u, tri.UVs[1].Y * v, tri.UVs[2].X * w };

            int sampleX = (int)Math.Round((tempU[0] + tempU[1] + tempU[2]) * texture.GetLength(0));
            int sampleY = (int)Math.Round((tempV[0] + tempV[1] + tempV[2]) * texture.GetLength(1));

            sampleX = sampleX % texture.GetLength(0);
            sampleY = sampleY % texture.GetLength(1);

            return texture[sampleX, sampleY];
        }
    }
}

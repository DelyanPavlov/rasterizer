
using System.Numerics;
using System.Text;

namespace triangle_test
{
    internal class Program
    {
        static Vector2[] points = new Vector2[3];
        static Vector2[] mids = new Vector2[3];
        static Vector2[] normals = new Vector2[3];
        static Color[] tempCols = new Color[2];
        static Color[] vecColor = new Color[3];
        static int windowW = 800, windowH = 480;
        static double[] usedT = new double[2], dist = new double[2];
        static double totalDist = 0;
        static bool isInside = true;

        static void Main(string[] args)
        {

            vecColor[0] = new Color(255, 0, 0);
            vecColor[1] = new Color(0, 255, 0);
            vecColor[2] = new Color(0, 0, 255);


            points[0] = new Vector2(0.1f * windowW, 0.1f * windowH);
            points[1] = new Vector2(0.9f * windowW, 0.1f * windowH);
            points[2] = new Vector2(0.9f * windowW, 0.9f * windowH);

            CalcTriNorms();

            Render();
        }

        static void CalcTriNorms()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 a = points[i];
                Vector2 b = points[(i + 1) % 3];
                Vector2 opposite = points[(i + 2) % 3];

                mids[i] = (a + b) / 2;
                Vector2 edge = b - a;
                Vector2 n = Vector2.Normalize(new Vector2(edge.Y, -edge.X));

                if (Vector2.Dot(n, opposite - mids[i]) < 0) n = -n;
                normals[i] = n;
            }
        }

        static Color DrawTri(Vector2 pixelPos)
        {
            totalDist = 0;
            double[] t = new double[3];
            Vector2[] nearestPoint = new Vector2[2];
            nearestPoint[0] = nearestPointOnLine(points[0], points[1], pixelPos, out t[0]);
            nearestPoint[1] = nearestPointOnLine(points[1], points[2], pixelPos, out t[1]);
            dist[0] = calcDist(pixelPos, nearestPoint[0]);
            dist[1] = calcDist(pixelPos, nearestPoint[1]);
            for (int i = 0; i < 3; i++)
            {
                Vector2 a = points[i], b = points[(i + 1) % 3];
                Color interpCol = vecColor[i] * (1 - t[i]) + vecColor[(i + 1) % 3] * t[i];
                if (i < 2)
                {
                    tempCols[i] = interpCol;
                    usedT[i] = t[i];
                }
            }
            totalDist = dist[0] + dist[1];
            return tempCols[0] * (dist[1] / totalDist) + tempCols[1] * (dist[0] / totalDist);
        }

        static Vector2 nearestPointOnLine(Vector2 a, Vector2 b, Vector2 point, out double t)
        {
            Vector2 lineDir = b - a;
            t = Vector2.Dot(point - a, lineDir) / Vector2.Dot(lineDir, lineDir);
            t = Math.Max(t, 0);
            t = Math.Min(t, 1);

            return a + (float)t * lineDir;
        }

        static double calcDist(Vector2 a, Vector2 b)
        {
            return Math.Sqrt(Math.Abs(a.X - b.X) * Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) * Math.Abs(a.Y - b.Y));
        }

        static void Render()
        {
            FileStream fs = File.Create(@"../../../output/out.ppm");
            fs.Write(Encoding.UTF8.GetBytes("P3\n"));
            fs.Write(Encoding.UTF8.GetBytes($"{windowW}  {windowH}\n"));
            fs.Write(Encoding.UTF8.GetBytes("255\n"));
            Color[,] colorDat = new Color[windowW, windowH];
            Color PixelCol;
            for (int j = 0; j < windowH; j++)
            {
                Console.WriteLine($"Starting pixelRow {j}");
                for (int i = 0; i < windowW; i++)
                {
                    isInside = true;
                    for (int x = 0; x < 3; x++)
                    {
                        if (Vector2.Dot(new Vector2(i, j) - mids[x], normals[x]) < 0)
                        {
                            isInside = false;
                            break;
                        }
                    }
                    if (isInside)
                    {
                        PixelCol = DrawTri(new Vector2(j, i));
                    }
                    else
                    {
                        PixelCol = new Color(0, 0, 0);
                    }
                    colorDat[i, j] = PixelCol;
                }
                Console.WriteLine($"Progress: {j} ({Math.Round(((double)j / windowH) * 100.0)}%)");
            }

            Console.WriteLine("Wrighting...");
            for (int j = 0; j < windowH; j++)
            {
                for (int i = 0; i < windowW; i++)
                {
                    fs.Write(Encoding.UTF8.GetBytes(colorDat[i, j].R + " " + colorDat[i, j].G + " " + colorDat[i, j].B + "\n"));
                }
            }
            Console.WriteLine("Done!");
            fs.Close();
        }
    }
}
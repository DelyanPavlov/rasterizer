
using System.Numerics;
using System.Text;

namespace triangle_test
{
    internal class Program
    {
        static double[] vwu = new double[3];
        static Triangle[] tris = new Triangle[2];
        static int windowW = 800, windowH = 600;
        public static Vec2 screenDim;
        public static Color[,] texture;

        static void Main(string[] args)
        {
            screenDim = new Vec2(windowW, windowH);

            Vertex[] vertecies = new Vertex[4];

            vertecies[0] = new Vertex(0.1, 0.1, 0, 0);
            vertecies[1] = new Vertex(0.9, 0.1, 1, 0);
            vertecies[2] = new Vertex(0.9, 0.9, 1, 1);
            vertecies[3] = new Vertex(0.1, 0.9, 0, 1);

            for (int i = 0; i < vertecies.Length; i++)
            {
                vertecies[i].calcScreenPos(windowW, windowH);
            }

            tris[0] = new Triangle(vertecies[0], vertecies[1], vertecies[2]);
            tris[1] = new Triangle(vertecies[0], vertecies[2], vertecies[3]);

            for (int i = 0; i < tris.Length; i++)
            {
                tris[i].CalcTriNorms();
            }
            texture = Helper.LoadTexture(@"../../../assets/textures/texture.ppm");
            Render();
        }

        static void Render()
        {
            FileStream fs = File.Create(@"../../../output/out.ppm");
            fs.Write(Encoding.UTF8.GetBytes("P3\n"));
            fs.Write(Encoding.UTF8.GetBytes($"{windowW}  {windowH}\n"));
            fs.Write(Encoding.UTF8.GetBytes("255\n"));
            Color[,] colorDat = new Color[windowW, windowH];
            for (int i = 0; i < windowW; i++)
            {
                for (int j = 0; j < windowH; j++)
                {
                    colorDat[i, j] = new Color(146, 179, 93);
                }
            }
            Color PixelCol;
            for (int j = 0; j < windowH; j++)
            {
                Console.WriteLine($"Starting pixelRow {j}");
                for (int i = 0; i < windowW; i++)   
                {
                    for (int y = 0; y < tris.Length; y++)
                    {
                        vwu = tris[y].CalcPixelColor(new Vec2(i, j));
                        if (vwu[0] + vwu[1] + vwu[2] <= 1.0 && vwu[0] >= 0 && vwu[1] >= 0 && vwu[2] >= 0)
                        {
                            PixelCol = Helper.SampleTexture(texture, tris[y], vwu);
                            colorDat[i, j] = PixelCol;
                        }
                    }
                }
                Console.WriteLine($"Progress: {j} ({Math.Round(((double)j / windowH) * 100.0)}%)");
            }

            Console.WriteLine("Wrighting...");
            for (int j = 0; j < windowH; j++)
            {
                for (int i = 0; i < windowW; i++)
                {
                    if(colorDat[i, j] == null)
                    {
                        colorDat[i, j] = new Color(0, 255, 0);
                    }
                    fs.Write(Encoding.UTF8.GetBytes(colorDat[i, j].R + " " + colorDat[i, j].G + " " + colorDat[i, j].B + "\n"));
                }
            }
            Console.WriteLine("Done!");
            fs.Close();
        }
    }
}
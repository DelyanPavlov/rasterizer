
using System.Numerics;
using System.Text;
using OpenCvSharp;

namespace triangle_test
{
    internal class Program
    {
        static double[] vwu = new double[3];
        static Triangle[] tris = new Triangle[2];
        public static Color[,] texture;
        public static int fps = 24;
        public static int renderLength = 6;
        public static Vertex[] vertecies = new Vertex[4];
        public static VideoWriter writer = new VideoWriter("../../../output/out.mp4", FourCC.MP4V, fps, new Size((int)Camera.screenDim.X, (int)Camera.screenDim.Y));

        static void Main(string[] args)
        {
            vertecies[0] = new Vertex(1, 1, 20, 0, 0);
            vertecies[1] = new Vertex(10, 1, 20, 2, 0);
            vertecies[2] = new Vertex(10, 10, 20, 2, 2);
            vertecies[3] = new Vertex(1, 10, 20, 0, 2);

            tris[0] = new Triangle(vertecies[0], vertecies[1], vertecies[2]);
            tris[1] = new Triangle(vertecies[0], vertecies[2], vertecies[3]);

            for (int i = 0; i < tris.Length; i++)
            {
                tris[i] = Camera.CalcScreenCords(tris[i]);
                for (int j = 0; j < 3; j++)
                {
                    tris[i].vertices[j].calcScreenPos((int)Camera.screenDim.X, (int)Camera.screenDim.Y);
                }
            }

            for (int i = 0; i < tris.Length; i++)
            {
                tris[i].CalcTriNorms();
            }
            Console.WriteLine("Loading texture...");
            texture = Helper.LoadTexture(@"../../../assets/textures/texture.ppm");

            Render();
        }

        static void Render()
        {
            int width = (int)Camera.screenDim.X;
            int height = (int)Camera.screenDim.Y;
            int totalFrames = fps * renderLength;

            byte[][] frame = new byte[totalFrames][];
            Triangle[][] tempTri = new Triangle[totalFrames][];
            Vertex[][] tempVert = new Vertex[totalFrames][];
            for (int i = 0; i < totalFrames; i++)
            {
                tempVert[i] = new Vertex[vertecies.Length];
                for (int j = 0; j < vertecies.Length; j++)
                {
                    vertecies[j].spaceX += 0.05;
                    tempVert[i][j] = new Vertex(vertecies[j]);
                }
                tempTri[i] = new Triangle[tris.Length];
                frame[i] = new byte[width * height * 3];
            }
            Parallel.For(0, totalFrames, t =>
            {
                tempTri[t][0] = new Triangle(tempVert[t][0], tempVert[t][1], tempVert[t][2]);
                tempTri[t][1] = new Triangle(tempVert[t][0], tempVert[t][2], tempVert[t][3]);
                tempTri[t][0] = Camera.CalcScreenCords(tempTri[t][0]);
                tempTri[t][1] = Camera.CalcScreenCords(tempTri[t][1]);
                RenderFrame(frame[t], width, height, tempTri[t]);
                Console.WriteLine($"Wrote frame {t + 1}/{totalFrames}");
            });

            Console.WriteLine("Writing...");
            for (int i = 0; i < totalFrames; i++)
            {
                using Mat mat = Mat.FromPixelData(height, width, MatType.CV_8UC3, frame[i]);
                writer.Write(mat);
            }

            writer.Release();
            Console.WriteLine("Done!");
        }

        static void RenderFrame(byte[] frame, int width, int height, Triangle[] tempTri)
        {
            double[] tempVWU;
            for (int i = 0; i < frame.Length; i += 3)
            {
                frame[i + 0] = 93;
                frame[i + 1] = 179;
                frame[i + 2] = 146;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = (y * width + x) * 3;
                    var p = new Vec2(x, y);

                    for (int k = 0; k < tempTri.Length; k++)
                    {
                        if (tempTri[k].isBackwards) continue;

                        tempVWU = tempTri[k].CalcPixelColor(p);

                        const double eps = -1e-9;
                        if (tempVWU[0] >= eps && tempVWU[1] >= eps && tempVWU[2] >= eps)
                        {
                            Color c = Helper.SampleTexture(texture, tempTri[k], tempVWU);
                            frame[idx + 0] = (byte)Math.Clamp(c.B, 0, 255);
                            frame[idx + 1] = (byte)Math.Clamp(c.G, 0, 255);
                            frame[idx + 2] = (byte)Math.Clamp(c.R, 0, 255);
                        }
                    }
                }
            }
        }
    }
}
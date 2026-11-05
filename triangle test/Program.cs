using System.Diagnostics;
using System.Numerics;
using System.Text;
using OpenCvSharp;

namespace triangle_test
{
    internal class Program
    {
        const string modelPath = @"../../../assets/models/cube.obj";
        public static Color[,] texture;
        public static int fps = 24;
        public static int renderLength = 4;
        public static VideoWriter writer = new VideoWriter("../../../output/out.mp4", FourCC.MP4V, fps, new Size((int)Camera.screenDim.X, (int)Camera.screenDim.Y));

        static void Main(string[] args)
        {
            Console.WriteLine("Loading texture...");
            texture = Helper.LoadTexture(@"../../../assets/textures/texture.ppm");
            Stopwatch sw = Stopwatch.StartNew();
            Render();
            sw.Stop();
            Console.WriteLine($"Time elapsed: {sw.ElapsedMilliseconds}ms");
        }
        static void RotateY(Triangle[] tris, double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            var done = new HashSet<Vertex>(ReferenceEqualityComparer.Instance);

            foreach (Triangle tri in tris)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vertex v = tri.vertices[j];
                    if (!done.Add(v)) continue;

                    double x = v.spaceX - Camera.X;
                    double y = v.spaceY - Camera.Y;
                    double z = v.spaceZ - Camera.Z;

                    double rx = x * cos + z * sin;
                    double rz = -x * sin + z * cos;

                    v.spaceX = rx - Camera.X;
                    v.spaceY = y - Camera.Y;
                    v.spaceZ = rz - Camera.Z;
                }
            }
        }

        static void Render()
        {
            int width = (int)Camera.screenDim.X;
            int height = (int)Camera.screenDim.Y;
            int totalFrames = fps * renderLength;

            byte[][] frame = new byte[totalFrames][];
            for (int i = 0; i < totalFrames; i++)
            {
                frame[i] = new byte[width * height * 3];
            }


            Parallel.For(0, totalFrames, t =>
            {
                Triangle[] tris = Helper.LoadModel(modelPath);
                double angle = 2 * Math.PI;
                RotateY(tris, angle);

                for (int i = 0; i < tris.Length; i++)
                {
                    tris[i] = Camera.CalcScreenCords(tris[i]);
                    for (int j = 0; j < 3; j++)
                    {
                        tris[i].vertices[j].calcScreenPos(width, height);
                    }
                }

                RenderFrame(frame[t], width, height, tris);
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
            double[] depth = new double[width * height];
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
                    int pix = y * width + x;
                    int idx = pix * 3;
                    var p = new Vec2(x, y);

                    for (int k = 0; k < tempTri.Length; k++)
                    {
                        if (tempTri[k].isBackwards) continue;

                        tempVWU = tempTri[k].CalcPixelColor(p);

                        const double eps = -1e-9;
                        if (tempVWU[0] >= eps && tempVWU[1] >= eps && tempVWU[2] >= eps)
                        {
                            Vertex[] vs = tempTri[k].vertices;
                            double invZ = tempVWU[2] / vs[0].spaceZ
                                        + tempVWU[0] / vs[1].spaceZ
                                        + tempVWU[1] / vs[2].spaceZ;

                            if (invZ <= depth[pix]) continue;
                            depth[pix] = invZ;

                            Color c = Helper.SampleTexture(texture, tempTri[k], tempVWU);
                            frame[idx + 0] = (byte)c.B;
                            frame[idx + 1] = (byte)c.G;
                            frame[idx + 2] = (byte)c.R;
                        }
                    }
                }
            }
        }
    }
}
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
        public static int fps = 30;
        public static int renderLength = 10;
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
                double angle = 2 * Math.PI * t / 360.0;
                RotateY(tris, angle);
            
                for (int i = 0; i < tris.Length; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        tris[i].vertices[j].calcScreenPos(width, height);
                    }
                    tris[i] = Camera.CalcScreenCords(tris[i]);
                }
            
                RenderFrame(frame[t], width, height, tris);
                Console.WriteLine($"Wrote frame {t + 1}/{totalFrames}");
            });

            //for (int t = 0; t < totalFrames; t++)
            //{
            //    Triangle[] tris = Helper.LoadModel(modelPath);
            //    double angle = 2 * Math.PI * t / 360.0;
            //    RotateY(tris, angle);
                //
            //    for (int i = 0; i < tris.Length; i++)
            //    {
            //        tris[i] = Camera.CalcScreenCords(tris[i]);
            //        for (int j = 0; j < 3; j++)
            //        {
            //            tris[i].vertices[j].calcScreenPos(width, height);
            //        }
            //    }
                //
            //    RenderFrame(frame[t], width, height, tris);
            //    Console.WriteLine($"Wrote frame {t + 1}/{totalFrames}");
            //}

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


            for (int t = 0; t < tempTri.Length; t++)
            {
                if (tempTri[t].isBackwards) continue;

                Vertex[] vs = tempTri[t].vertices;

                double minX = Math.Min(Math.Min(vs[0].ScreenPosition.X, vs[1].ScreenPosition.X), vs[2].ScreenPosition.X);
                double maxX = Math.Max(Math.Max(vs[0].ScreenPosition.X, vs[1].ScreenPosition.X), vs[2].ScreenPosition.X);
                double minY = Math.Min(Math.Min(vs[0].ScreenPosition.Y, vs[1].ScreenPosition.Y), vs[2].ScreenPosition.Y);
                double maxY = Math.Max(Math.Max(vs[0].ScreenPosition.Y, vs[1].ScreenPosition.Y), vs[2].ScreenPosition.Y);

                int left = (int)Math.Max(0, Math.Min(width - 1, Math.Floor(minX)));
                int right = (int)Math.Max(0, Math.Min(width - 1, Math.Ceiling(maxX)));
                int top = (int)Math.Max(0, Math.Min(height - 1, Math.Floor(minY)));
                int bottom = (int)Math.Max(0, Math.Min(height - 1, Math.Ceiling(maxY)));

                for (int y = top; y <= bottom; y++)
                {
                    for (int x = left; x <= right; x++)
                    {
                        int pix = y * width + x;
                        int idx = pix * 3;
                        var p = new Vec2(x, y);

                        tempVWU = tempTri[t].CalcPixelColor(p);

                        const double eps = -1e-9;
                        if (tempVWU[0] >= eps && tempVWU[1] >= eps && tempVWU[2] >= eps)
                        {
                            Vertex[] vss = tempTri[t].vertices;
                            double invZ = tempVWU[2] / vss[0].spaceZ
                                        + tempVWU[0] / vss[1].spaceZ
                                        + tempVWU[1] / vss[2].spaceZ;

                            if (invZ <= depth[pix]) continue;
                            depth[pix] = invZ;

                            Color c = Helper.SampleTexture(texture, tempTri[t], tempVWU);
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

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
        public static int renderLength = 2;
        public static VideoWriter writer = new VideoWriter("out.mp4", FourCC.MP4V, fps, new Size((int)Camera.screenDim.X, (int)Camera.screenDim.Y));

        static void Main(string[] args)
        {

            Vertex[] vertecies = new Vertex[4];

            vertecies[0] = new Vertex(1, 1, 20, 0, 0);
            vertecies[1] = new Vertex(9, 1, 20, 3, 0);
            vertecies[2] = new Vertex(9, 9, 20, 2, 2);
            vertecies[3] = new Vertex(1, 9, 20, 0, 2);

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
            texture = Helper.LoadTexture(@"../../../assets/textures/texture.ppm");
            Render();
        }

        static void Render()
        {
            int width = (int)Camera.screenDim.X;
            int height = (int)Camera.screenDim.Y;
            int totalFrames = fps * renderLength;

            if (!writer.IsOpened())
            {
                Console.WriteLine("VideoWriter failed to open. Check the codec / OpenCvSharp runtime package.");
                return;
            }

            byte[] frame = new byte[width * height * 3];

            // Nothing moves yet, so render once and write the same frame repeatedly.
            // When you add animation, move this call inside the loop and use t.
            RenderFrame(frame, width, height);

            using Mat mat = Mat.FromPixelData(height, width, MatType.CV_8UC3, frame);
            for (int t = 0; t < totalFrames; t++)
            {
                writer.Write(mat);
                Console.WriteLine($"Wrote frame {t + 1}/{totalFrames}");
            }

            // Finalises the MP4. Without this the file is unplayable.
            writer.Release();
            Console.WriteLine("Done: " + Path.GetFullPath("out.mp4"));
        }

        static void RenderFrame(byte[] frame, int width, int height)
        {
            // Background, BGR order (RGB 146,179,93)
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

                    for (int k = 0; k < tris.Length; k++)
                    {
                        if (tris[k].isBackwards) continue;

                        vwu = tris[k].CalcPixelColor(p);

                        // small epsilon so pixels on the shared edge are not dropped
                        const double eps = -1e-9;
                        if (vwu[0] >= eps && vwu[1] >= eps && vwu[2] >= eps)
                        {
                            Color c = Helper.SampleTexture(texture, tris[k], vwu);
                            frame[idx + 0] = (byte)Math.Clamp(c.B, 0, 255);
                            frame[idx + 1] = (byte)Math.Clamp(c.G, 0, 255);
                            frame[idx + 2] = (byte)Math.Clamp(c.R, 0, 255);
                        }
                    }
                }

                if (y % 100 == 0)
                    Console.WriteLine($"Rendering row {y}/{height}");
            }
        }
    }
}
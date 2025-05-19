using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Filters_lab1
{
    public abstract class FiltersByMe
    {
        public Bitmap processimage(Bitmap sourceIm, BackgroundWorker worker)
        {
            Bitmap resIm = new Bitmap(sourceIm.Width, sourceIm.Height);
            for (int i = 0; i < sourceIm.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resIm.Width * 100));
                if (worker.CancellationPending) return null;
                for (int j = 0; j < sourceIm.Height; j++)
                {
                    resIm.SetPixel(i, j, calcNewPCol(sourceIm, i, j));
                }
            }
            return resIm;
        }
        protected abstract Color calcNewPCol(Bitmap sourceIm, int x, int y);

        public int Clamp(int val, int min, int max)
        {
            if (val < min)
            {
                return min;
            }
            if (val > max)
            {
                return max;
            }
            return val;
        }
    }

    class InvertFilter : FiltersByMe
    {
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            Color sourceCol = sourceIm.GetPixel(x, y);
            Color resCol = Color.FromArgb(255 - sourceCol.R, 255 - sourceCol.G, 255 - sourceCol.B);
            return resCol;
        }
    }

    class Turn : FiltersByMe
    {
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            int newx = (int)((x - 100) * Math.Cos(Math.PI / 6) - (y - 100) * Math.Sin(Math.PI / 6) + 100);
            int newy = (int)((x - 100) * Math.Sin(Math.PI / 6) + (y - 100) * Math.Cos(Math.PI / 6) + 100);
            if (newx >= sourceIm.Width || newy >= sourceIm.Height)
            {
                return Color.FromArgb(200, 200, 200);
            }

            if (newx <= 0 || newy <= 0)
            {
                return Color.FromArgb(200, 200, 200);
            }

            Color OrColor = sourceIm.GetPixel(newx, newy);
            int R = OrColor.R;
            int G = OrColor.G;
            int B = OrColor.B;
            return Color.FromArgb(
                Clamp(R, 0, 255),
                Clamp(G, 0, 255),
                Clamp(B, 0, 255));
        }
    }

    class GrayScaleFilter : FiltersByMe
    {
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            Color OrColor = sourceIm.GetPixel(x, y);
            double intensity = 0.36 * OrColor.R + 0.53 * OrColor.G + 0.11 * OrColor.B;
            int intense = (int)intensity;
            return Color.FromArgb(intense, intense, intense);
        }
    }

    class SepiaFilter : FiltersByMe
    {
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            Color OrColor = sourceIm.GetPixel(x, y);
                double intensity = 0.36 * OrColor.R + 0.53 * OrColor.G + 0.11 * OrColor.B;
                int R = (int)intensity + 2 * 12;
                int G = (int)intensity + (int)0.5 * 12;
                int B = (int)intensity - 1 * 12;
                return Color.FromArgb(
                    Clamp(R, 0, 255),
                    Clamp(G, 0, 255),
                    Clamp(B, 0, 255));
        }
    }

    class BrightnessFilter : FiltersByMe
    {
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            Color OrColor = sourceIm.GetPixel(x, y);
            int R = OrColor.R + 70;
            int G = OrColor.G + 70;
            int B = OrColor.B + 70;
            return Color.FromArgb(
                Clamp(R, 0, 255),
                Clamp(G, 0, 255),
                Clamp(B, 0, 255));
        }
    }
    class Move : FiltersByMe
    {
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            x += 50;
            y += 50;
            if (x >= sourceIm.Width || y >= sourceIm.Height)
            {
                return Color.FromArgb(200, 200, 200);
            }
            Color OrColor = sourceIm.GetPixel(x, y);
            int R = OrColor.R;
            int G = OrColor.G;
            int B = OrColor.B;
            return Color.FromArgb(
                Clamp(R, 0, 255),
                Clamp(G, 0, 255),
                Clamp(B, 0, 255));
        }
    }

    class Glass : FiltersByMe
    {
        Random rand = new Random();
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {

            int newx = x + (int)((rand.NextDouble() - 0.5) * 15);
            int newy = y + (int)((rand.NextDouble() - 0.5) * 15);

            if (newx < 0 || newx >= sourceIm.Width || newy < 0 || newy >= sourceIm.Height)
            {
                return Color.FromArgb(200, 200, 200);
            }
            Color OrColor = sourceIm.GetPixel(newx, newy);
            int R = OrColor.R;
            int G = OrColor.G;
            int B = OrColor.B;
            return Color.FromArgb(
                Clamp(R, 0, 255),
                Clamp(G, 0, 255),
                Clamp(B, 0, 255));
        }
    }

    class Waves : FiltersByMe
    {
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            int newx = (int)(x + 10 * Math.Sin((2 * Math.PI * y) / 60));
            int newy = y;

            if (newx < 0 || newx >= sourceIm.Width || newy < 0 || newy >= sourceIm.Height)
            {
                return Color.FromArgb(200, 200, 200);
            }
            Color OrColor = sourceIm.GetPixel(newx, newy);
            int R = OrColor.R;
            int G = OrColor.G;
            int B = OrColor.B;
            return Color.FromArgb(
                Clamp(R, 0, 255),
                Clamp(G, 0, 255),
                Clamp(B, 0, 255));
        }
    }

    class Waves2 : FiltersByMe
    {
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            int newx = x;
            int newy = (int)(y + 10 * Math.Sin((2 * Math.PI * x) / 60));

            if (newx < 0 || newx >= sourceIm.Width || newy < 0 || newy >= sourceIm.Height)
            {
                return Color.FromArgb(200, 200, 200);
            }
            Color OrColor = sourceIm.GetPixel(newx, newy);
            int R = OrColor.R;
            int G = OrColor.G;
            int B = OrColor.B;
            return Color.FromArgb(
                Clamp(R, 0, 255),
                Clamp(G, 0, 255),
                Clamp(B, 0, 255));
        }
    }

    class MatrixFiltes : FiltersByMe
    {
        protected float[,] kernel = null;
        protected MatrixFiltes() { }
        public MatrixFiltes(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resR = 0;
            float resG = 0;
            float resB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceIm.Width - 1);
                    int idY = Clamp(y + k, 0, sourceIm.Height - 1);
                    Color neighborCol = sourceIm.GetPixel(idX, idY);
                    resR += neighborCol.R * kernel[k + radiusX, l + radiusY];
                    resG += neighborCol.G * kernel[k + radiusX, l + radiusY];
                    resB += neighborCol.B * kernel[k + radiusX, l + radiusY];
                }
            return Color.FromArgb(
                Clamp((int)resR, 0, 255),
                Clamp((int)resG, 0, 255),
                Clamp((int)resB, 0, 255)
                );
        }
    }
    class BlurFilter : MatrixFiltes
    {
        public BlurFilter()
        {
            int szX = 3;
            int szY = 3;
            kernel = new float[szX, szY];
            for (int i = 0; i < szX; i++)
                for (int j = 0; j < szY; j++)
                {
                    kernel[i, j] = 1.0f / (float)(szX * szY);
                }
        }
    }

    class Embossing : MatrixFiltes
    {
        public Embossing()
        {
            int szX = 3;
            int szY = 3;
            kernel = new float[szX, szY];

            kernel[0, 0] = 0.0f;
            kernel[0, 1] = 1.0f;
            kernel[0, 2] = 0.0f;
            kernel[1, 0] = 1.0f;
            kernel[1, 1] = 0.0f;
            kernel[1, 2] = -1.0f;
            kernel[2, 0] = 0.0f;
            kernel[2, 1] = -1.0f;
            kernel[2, 2] = 0.0f;
        }
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resR = 0;
            float resG = 0;
            float resB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceIm.Width - 1);
                    int idY = Clamp(y + k, 0, sourceIm.Height - 1);
                    Color neighborCol = sourceIm.GetPixel(idX, idY);
                    resR += neighborCol.R * kernel[k + radiusX, l + radiusY] + 25;
                    resG += neighborCol.G * kernel[k + radiusX, l + radiusY] + 25;
                    resB += neighborCol.B * kernel[k + radiusX, l + radiusY] + 25;
                }
            return Color.FromArgb(
                Clamp((int)resR, 0, 255),
                Clamp((int)resG, 0, 255),
                Clamp((int)resB, 0, 255)
                );
        }
    }

    class SobelFilter : MatrixFiltes
    {
        public SobelFilter()
        {
            int szX = 3;
            int szY = 3;
            kernel = new float[szX, szY];

            kernel[0, 0] = -1.0f;
            kernel[0, 1] = -2.0f;
            kernel[0, 2] = -1.0f;
            kernel[1, 0] = 0.0f;
            kernel[1, 1] = 0.0f;
            kernel[1, 2] = 0.0f;
            kernel[2, 0] = 1.0f;
            kernel[2, 1] = 2.0f;
            kernel[2, 2] = 1.0f;
        }

    }

    class MotionBlur : MatrixFiltes
    {
        public MotionBlur()
        {
            int n = 15;
            int szX = n;
            int szY = n;
            kernel = new float[szX, szY];

            for (int i = 0; i < szX; i++)
            {
                for (int j = 0; j < szY; j++)
                {
                    if (i == j) kernel[i, j] = (float)(1.0f / n);
                    else kernel[i, j] = 0.0f;
                }
            }
        }
    }

    class SharpnessFilter : MatrixFiltes
    {
        public SharpnessFilter()
        {
            int szX = 3;
            int szY = 3;
            kernel = new float[szX, szY];

            kernel[0, 0] = 0.0f;
            kernel[0, 1] = -1.0f;
            kernel[0, 2] = 0.0f;
            kernel[1, 0] = -1.0f;
            kernel[1, 1] = 5.0f;
            kernel[1, 2] = -1.0f;
            kernel[2, 0] = 0.0f;
            kernel[2, 1] = -1.0f;
            kernel[2, 2] = 0.0f;
        }

    }

    class BorderFilter : MatrixFiltes
    {
        public BorderFilter()
        {
            int szX = 3;
            int szY = 3;
            kernel = new float[szX, szY];

            kernel[0, 0] = 3.0f;
            kernel[0, 1] = 10.0f;
            kernel[0, 2] = 3.0f;
            kernel[1, 0] = 0.0f;
            kernel[1, 1] = 0.0f;
            kernel[1, 2] = 0.0f;
            kernel[2, 0] = -3.0f;
            kernel[2, 1] = -10.0f;
            kernel[2, 2] = -3.0f;
        }

    }

    class GaussianFilter : MatrixFiltes
    {
        public void createGaussianKernel(int radius, float sigma)
        {
            int sz = 2 * radius + 1;
            kernel = new float[sz, sz];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            for (int i = 0; i < sz; i++)
                for (int j = 0; j < sz; j++)
                {
                    kernel[i, j] /= norm;
                }
        }
        public GaussianFilter()
        {
            createGaussianKernel(5, 4);
        }
    }

    class Dilation : MatrixFiltes
    {
        public Dilation()
        {
            kernel = new float[,]
        {
            { 1.0f, 1.0f, 1.0f},
            { 1.0f, 1.0f, 1.0f},
            { 1.0f, 1.0f, 1.0f},
            {1.0f, 1.0f, 1.0f},
            {1.0f, 1.0f, 1.0f}
        };
        }
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            Color maxColor = Color.Black;
            Color OrCol = sourceIm.GetPixel(x, y);
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int newX = x + k;
                    int newY = y + l;
                    if (newX >= 0 && newX < sourceIm.Width && newY >= 0 && newY < sourceIm.Height)
                    {
                        Color neighborCol = sourceIm.GetPixel(newX, newY);
                        if (neighborCol.GetBrightness() > maxColor.GetBrightness())
                        {
                            maxColor = neighborCol;
                        }
                    }
                }
            }
            return maxColor;
        }
    }

    class Erosion : MatrixFiltes
    {
        public Erosion()
        {
            kernel = new float[,]
        {
            { 1.0f, 1.0f, 1.0f},
            { 1.0f, 1.0f, 1.0f},
            { 1.0f, 1.0f, 1.0f },
            {1.0f, 1.0f, 1.0f},
            {1.0f, 1.0f, 1.0f}
        };
        }
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            Color minColor = Color.White;
            Color OrCol = sourceIm.GetPixel(x, y);
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int newX = x + k;
                    int newY = y + l;
                    if (newX >= 0 && newX < sourceIm.Width && newY >= 0 && newY < sourceIm.Height)
                    {
                        Color neighborCol = sourceIm.GetPixel(newX, newY);
                        if (neighborCol.GetBrightness() < minColor.GetBrightness())
                        {
                            minColor = neighborCol;
                        }
                    }
                }
            }
            return minColor;
        }
    }

    
    class Gray_World : FiltersByMe
    {
        private double[] avg;

        public void CalculateAverageBrightness(Bitmap sourceIm)
        {
            long totalR = 0;
            long totalG = 0;
            long totalB = 0;
            int pixelCount = sourceIm.Width * sourceIm.Height;

            for (int y = 0; y < sourceIm.Height; y++)
            {
                for (int x = 0; x < sourceIm.Width; x++)
                {
                    Color pixelColor = sourceIm.GetPixel(x, y);
                    totalR += pixelColor.R;
                    totalG += pixelColor.G;
                    totalB += pixelColor.B;
                }
            }

            double averageR = totalR / (double)pixelCount;
            double averageG = totalG / (double)pixelCount;
            double averageB = totalB / (double)pixelCount;
            avg = new double[] { averageR, averageG, averageB };
        }

        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            if (avg == null)
            {
                CalculateAverageBrightness(sourceIm);
            }
            Color OrColor = sourceIm.GetPixel(x, y);
            double avgc = (avg[0] + avg[1] + avg[2]) / 3;
            double R = OrColor.R * (avgc / avg[0]);
            double G = OrColor.G * (avgc / avg[1]);
            double B = OrColor.B * (avgc / avg[2]);

            return Color.FromArgb(
                Clamp((int)R, 0, 255),
                Clamp((int)G, 0, 255),
                Clamp((int)B, 0, 255));
        }
    }

    class AutoContrast : FiltersByMe
    {
        private int minR = 255, minG = 255, minB = 255;
        private int maxR = 0, maxG = 0, maxB = 0;

        
        private void CalculateMinMax(Bitmap sourceIm)
        {
            for (int y = 0; y < sourceIm.Height; y++)
            {
                for (int x = 0; x < sourceIm.Width; x++)
                {
                    Color pixelColor = sourceIm.GetPixel(x, y);
                    if (pixelColor.R < minR) minR = pixelColor.R;
                    if (pixelColor.G < minG) minG = pixelColor.G;
                    if (pixelColor.B < minB) minB = pixelColor.B;
                    if (pixelColor.R > maxR) maxR = pixelColor.R;
                    if (pixelColor.G > maxG) maxG = pixelColor.G;
                    if (pixelColor.B > maxB) maxB = pixelColor.B;
                }
            }
        }

        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            if (minR == 255 && maxR == 0) 
            {
                CalculateMinMax(sourceIm);
            }
            Color OrColor = sourceIm.GetPixel(x, y);
            int R = (OrColor.R - minR) * 255 / (maxR - minR);
            int G = (OrColor.G - minG) * 255 / (maxG - minG);
            int B = (OrColor.B - minB) * 255 / (maxB - minB);

            return Color.FromArgb(
                Clamp(R, 0, 255),
                Clamp(G, 0, 255),
                Clamp(B, 0, 255));
        }
    }
    class RingFilter : FiltersByMe
    {
        double radius;
        protected override Color calcNewPCol(Bitmap sourceIm, int x, int y)
        {
            int centerX = sourceIm.Width / 2;
            int centerY = sourceIm.Height / 2;
            radius = Math.Sqrt((x - centerX)* (x - centerX) + (y - centerY)* (y - centerY));

            if ((radius <= 50)||((radius >= 100)&&(radius <= 150)))
            {
                Color sourceCol = sourceIm.GetPixel(x, y);
                Color resCol = Color.FromArgb(255 - sourceCol.R, 255 - sourceCol.G, 255 - sourceCol.B);
                return resCol;
            }
            else return sourceIm.GetPixel(x, y);
        }
    }

}



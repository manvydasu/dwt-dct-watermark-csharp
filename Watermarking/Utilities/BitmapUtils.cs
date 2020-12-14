using System.Drawing;

namespace DWT.Utilities
{
    public static class BitmapUtils
    {
        // not very performant at the moment
        public static (double[,] r, double[,] g, double[,] b) GetRGB(Bitmap image)
        {
            var redPixels   = new double[image.Width, image.Height];
            var greenPixels = new double[image.Width, image.Height];
            var bluePixels  = new double[image.Width, image.Height];

            for (var i = 0; i < image.Height; i++)
            {
                for (var j = 0; j < image.Width; j++)
                {
                    var pixel = image.GetPixel(i, j);
                    redPixels[i, j]   = pixel.R;
                    greenPixels[i, j] = pixel.G;
                    bluePixels[i, j]  = pixel.B;
                }
            }

            return (redPixels, greenPixels, bluePixels);
        }

        public static Bitmap CreateBitmapFromRGB(double[,] r, double[,] g, double[,] b)
        {
            var width            = r.GetLength(0);
            var height           = r.GetLength(1);
            var watermarkedImage = new Bitmap(width, height);

            for (var k = 0; k < height; k++)
            {
                for (var j = 0; j < width; j++)
                {
                    var red   = NormalizePixelValue(r[k, j]);
                    var green = NormalizePixelValue(g[k, j]);
                    var blue  = NormalizePixelValue(b[k, j]);

                    watermarkedImage.SetPixel(k, j, Color.FromArgb(0, red, green, blue));
                }
            }

            return watermarkedImage;
        }

        public static Bitmap CreateGeyscaleImage(int[,] data)
        {
            var width            = data.GetLength(0);
            var height           = data.GetLength(1);
            var watermarkedImage = new Bitmap(width, height);

            for (var k = 0; k < height; k++)
            {
                for (var j = 0; j < width; j++)
                {
                    var value = data[k, j];

                    var red   = value;
                    var green = value;
                    var blue  = value;

                    watermarkedImage.SetPixel(k, j, Color.FromArgb(0, red, green, blue));
                }
            }

            return watermarkedImage;
        }

        public static int NormalizePixelValue(double value)
        {
            if (value > 255)
            {
                return 255;
            }

            if (value < 0)
            {
                return 0;
            }

            return (int) value;
        }
    }
}
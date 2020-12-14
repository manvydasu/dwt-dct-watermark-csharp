using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DWT.Utilities;
using MathNet.Numerics.Statistics;
using ColorConverter = DWT.Utilities.ColorConverter;

namespace DWT.WatermarkingAlgorithms
{
    public static class DwtWatermark
    {
        private static readonly int DCT_BLOCK_SIZE   = 4;
        private static readonly int WATERMARK_FACTOR = 4;
        private static readonly int PN_SEQUENCE_SEED = 4;

        public static Bitmap EmbedWatermark(Bitmap targetImage, Bitmap watermarkImage)
        {
            var (redTarget, greenTarget, blueTarget) = BitmapUtils.GetRGB(targetImage);
            var (redWatermark, _, _)                 = BitmapUtils.GetRGB(watermarkImage);

            var grayscaleWatermark = TransformColorSpaceIntoGreyscale(redWatermark);

            applyDwtDctWatermarkOnColorSpace(redTarget, grayscaleWatermark);

            return BitmapUtils.CreateBitmapFromRGB(redTarget, greenTarget, blueTarget);
        }

        public static Bitmap EmbedWatermarkUsingYiq(Bitmap targetImage, Bitmap watermarkImage)
        {
            var (redTarget, greenTarget, blueTarget) = BitmapUtils.GetRGB(targetImage);
            var (redWatermark, greenWatermark, blueWatermark) = BitmapUtils.GetRGB(watermarkImage);
            var (yTarget, iTarget, qTarget) = ColorConverter.ConvertRgbToYiq(redTarget, greenTarget, blueTarget);
            var grayscaleWatermark = TransformColorSpaceIntoGreyscale(redWatermark);

            applyDwtDctWatermarkOnColorSpace(iTarget, grayscaleWatermark);

            var watermarkedImage = ConvertYiqColorToImage(yTarget, iTarget, qTarget);

            return watermarkedImage;
        }

        public static Image ExtractWatermark(Bitmap watermarkedImage, Bitmap watermarkImage)
        {
            var (redWatermarked, _, _) = BitmapUtils.GetRGB(watermarkedImage);
            var (redWatermark, _, _)   = BitmapUtils.GetRGB(watermarkImage);

            var grayscaleWatermark = TransformColorSpaceIntoGreyscale(redWatermark);

            return ExtractDwtDctWatermark(redWatermarked, grayscaleWatermark);
        }

        public static Bitmap ExtractWatermarkUsingYiq(Bitmap targetImage, Bitmap watermarkImage)
        {
            var (redTarget, greenTarget, blueTarget) = BitmapUtils.GetRGB(targetImage);
            var (redWatermark, greenWatermark, blueWatermark) = BitmapUtils.GetRGB(watermarkImage);
            var (yTarget, iTarget, qTarget) = ColorConverter.ConvertRgbToYiq(redTarget, greenTarget, blueTarget);
            var grayscaleWatermark = TransformColorSpaceIntoGreyscale(redWatermark);

            return ExtractDwtDctWatermark(iTarget, grayscaleWatermark);
        }

        private static void applyDwtDctWatermarkOnColorSpace(double[,] colorSpaceTarget, double[,] greyScaleWatermark)
        {
            var a         = colorSpaceTarget[0, 0];
            var flattened = greyScaleWatermark.Cast<double>().ToArray();

            DWTTransform.ForwardTransform(colorSpaceTarget, 2);
            var subBandTarget        = WatermarkUtils.Ll1(colorSpaceTarget);
            var midband              = GetMidBand(DCT_BLOCK_SIZE);
            var embeddingBlockLength = subBandTarget.GetLength(0);

            var midBandOneElementsCount = midband.Cast<double>().Count(el => (int) el == 1);
            var (pnSequenceZero, pnSequenceOne) = GenerateUncorelatedPnSequences(midBandOneElementsCount);

            var x = 0;
            var y = 0;

            for (int i = 0; i < flattened.Length; i++)
            {
                var block = subBandTarget.Submatrix(y, y + DCT_BLOCK_SIZE - 1,
                                                    x, x + DCT_BLOCK_SIZE - 1);

                DCTTransform.DCT(block);

                var ll = 0;

                for (int j = 0; j < midband.GetLength(0); j++)
                {
                    for (int k = 0; k < midband.GetLength(1); k++)
                    {
                        if (midband[j, k] == 1)
                        {
                            var elementToAdd = flattened[i] == 0 ? pnSequenceOne[ll] : pnSequenceZero[ll];
                            block[j, k] = block[j, k] + elementToAdd * WATERMARK_FACTOR;
                            ll++;
                        }
                    }
                }

                DCTTransform.IDCT(block);

                subBandTarget.ApplyDataBlock(block, y, x);

                if (x + DCT_BLOCK_SIZE >= greyScaleWatermark.GetLength(1))
                {
                    x =  1;
                    y += DCT_BLOCK_SIZE;

                    if (y >= embeddingBlockLength)
                    {
                        break;
                    }
                }
                else
                {
                    x += DCT_BLOCK_SIZE;
                }
            }

            ;

            WatermarkUtils.ApplyLl1SubBand(colorSpaceTarget, subBandTarget);
            DWTTransform.InverseWaveletTransform(colorSpaceTarget, 2);
        }

        private static Bitmap ExtractDwtDctWatermark(double[,] iWatermarked, double[,] greyScaleWatermark)
        {
            var flattened = greyScaleWatermark.Cast<double>().ToArray();

            DWTTransform.ForwardTransform(iWatermarked, 2);

            var subBandTarget           = WatermarkUtils.Ll1(iWatermarked);
            var midband                 = GetMidBand(DCT_BLOCK_SIZE);
            var embeddingBlockLength    = subBandTarget.GetLength(0);
            var midBandOneElementsCount = midband.Cast<double>().Count(el => (int) el == 1);
            var (pnSequenceZero, pnSequenceOne) = GenerateUncorelatedPnSequences(midBandOneElementsCount);
            var sequence        = new double[pnSequenceOne.Count];
            var correlationZero = new double[flattened.Length];
            var correlationOne  = new double[flattened.Length];

            var x = 0;
            var y = 0;

            for (var i = 0; i < flattened.Length; i++)
            {
                var block = subBandTarget.Submatrix(y, y + DCT_BLOCK_SIZE - 1,
                                                    x, x + DCT_BLOCK_SIZE - 1);
                DCTTransform.DCT(block);

                var ll = 0;

                for (var j = 0; j < midband.GetLength(0); j++)
                {
                    for (var k = 0; k < midband.GetLength(1); k++)
                    {
                        if (midband[j, k] == 1)
                        {
                            sequence[ll] = block[j, k];
                            ll++;
                        }
                    }
                }

                correlationZero[i] = Correlation.Pearson(pnSequenceZero, sequence);
                correlationOne[i]  = Correlation.Pearson(pnSequenceOne,  sequence);

                if (x + DCT_BLOCK_SIZE >= greyScaleWatermark.GetLength(1))
                {
                    x =  1;
                    y += DCT_BLOCK_SIZE;

                    if (y >= embeddingBlockLength)
                    {
                        break;
                    }
                }
                else
                {
                    x += DCT_BLOCK_SIZE;
                }
            }

            ;

            var reconstructedImage = new int [flattened.Length];

            for (var i = 0; i < flattened.Length; i++)
            {
                if (correlationZero[i] > correlationOne[i])
                {
                    reconstructedImage[i] = 255;
                }
                else
                {
                    reconstructedImage[i] = 0;
                }
            }

            var imgLength = greyScaleWatermark.GetLength(0);
            var img       = new int[greyScaleWatermark.GetLength(0), greyScaleWatermark.GetLength(1)];

            for (var i = 0; i < reconstructedImage.Length; i++)
            {
                img[i / imgLength, i % imgLength] = reconstructedImage[i];
            }

            return BitmapUtils.CreateGeyscaleImage(img);
        }

        private static double[,] TransformColorSpaceIntoGreyscale(double[,] data)
        {
            var width     = data.GetLength(0);
            var height    = data.GetLength(1);
            var grayscale = new double[width, height];


            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    grayscale[i, j] = data[i, j] > 125 ? 255 : 0;
                }
            }

            return grayscale;
        }

        private static double[,] GetMidBand(int size)
        {
            if (size == 4)
            {
                return new double[,]
                       {
                           {0, 1, 1, 0},
                           {1, 1, 0, 0},
                           {1, 0, 0, 0},
                           {0, 0, 0, 0},
                       };
            }

            if (size == 8)
            {
                return new double[,]
                       {
                           {0, 0, 0, 1, 1, 1, 1, 0},
                           {0, 0, 1, 1, 1, 1, 0, 0},
                           {0, 1, 1, 1, 1, 0, 0, 0},
                           {1, 1, 1, 1, 0, 0, 0, 0},
                           {1, 1, 1, 0, 0, 0, 0, 0},
                           {1, 1, 0, 0, 0, 0, 0, 0},
                           {1, 0, 0, 0, 0, 0, 0, 0},
                           {0, 0, 0, 0, 0, 0, 0, 0},
                       };
            }

            throw new NotImplementedException("Only 8x8 and 4x4 midbands are supported");
        }

        private static (List<double>, List<double>) GenerateUncorelatedPnSequences(int length)
        {
            var randomOne      = new Random(PN_SEQUENCE_SEED);
            var randomZero     = new Random(PN_SEQUENCE_SEED + 1);
            var pnSequenceZero = GeneratePnSequence(length, randomOne);
            var pnSequenceOne  = GeneratePnSequence(length, randomZero);

            while (Correlation.Pearson(pnSequenceZero, pnSequenceOne) > 0.3 ||
                   Correlation.Pearson(pnSequenceZero, pnSequenceOne) < -0.3)
            {
                pnSequenceZero = GeneratePnSequence(length, randomOne);
                pnSequenceOne  = GeneratePnSequence(length, randomZero);
            }

            return (pnSequenceZero, pnSequenceOne);
        }

        private static List<double> GeneratePnSequence(int length, Random rand)
        {
            var items = new List<double>();

            for (var i = 0; i < length; i++)
            {
                var item = rand.NextDouble() * 5;
                items.Add(item);
            }

            return items;
        }

        private static Bitmap ConvertYiqColorToImage(double[,] y, double[,] i, double[,] q)
        {
            var width            = y.GetLength(0);
            var height           = y.GetLength(1);
            var watermarkedImage = new Bitmap(width, height);
            var (watermarkedR, watermarkedG, watermarkedB) = ColorConverter.ConvertYiqToRgb(y, i, q);

            for (var k = 0; k < height; k++)
            {
                for (var j = 0; j < width; j++)
                {
                    var red   = BitmapUtils.NormalizePixelValue(watermarkedR[k, j]);
                    var green = BitmapUtils.NormalizePixelValue(watermarkedG[k, j]);
                    var blue  = BitmapUtils.NormalizePixelValue(watermarkedB[k, j]);

                    watermarkedImage.SetPixel(k, j, Color.FromArgb(0, red, green, blue));
                }
            }

            return watermarkedImage;
        }
    }
}
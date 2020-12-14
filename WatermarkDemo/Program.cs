using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DWT.WatermarkingAlgorithms;

namespace WatermarkDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var imageFilePath     = "C:/watermark_test/nature.jpg";
            var watermarkFilePath = "C:/watermark_test/watermark16.jpg";

            TestDwtWatermarkUsingYiq(imageFilePath, watermarkFilePath);

            Console.WriteLine("Done...");
        }

        private static void TestDwtWatermark(string targetImagePath, string watermarkImagePath)
        {
            var       watermarkedPath = "C:/watermark_test/watermarked.jpg";
            using var targetImage     = ReadImageFromFile(targetImagePath);
            using var watermarkImage  = ReadImageFromFile(watermarkImagePath);

            var watermarkedImage = DwtWatermark.EmbedWatermark(targetImage, watermarkImage);
            WriteImageToFile(watermarkedImage, watermarkedPath);

            using var watermarkedImageFromFile = ReadImageFromFile(watermarkedPath);

            var extractedWatermarkWithoutSaving =
                DwtWatermark.ExtractWatermark(watermarkedImage, ReadImageFromFile(watermarkImagePath));
            var extractedWatermarkFromStoredFile =
                DwtWatermark.ExtractWatermark(watermarkedImageFromFile, ReadImageFromFile(watermarkImagePath));

            WriteImageToFile(extractedWatermarkWithoutSaving,  "C:/watermark_test/extracted.jpg");
            WriteImageToFile(extractedWatermarkFromStoredFile, "C:/watermark_test/extracted_from_file.jpg");
        }
        
        private static void TestDwtWatermarkUsingYiq(string targetImagePath, string watermarkImagePath)
        {
            var       watermarkedPath = "C:/watermark_test/watermarked.jpg";
            using var targetImage     = ReadImageFromFile(targetImagePath);
            using var watermarkImage  = ReadImageFromFile(watermarkImagePath);

            var watermarkedImage = DwtWatermark.EmbedWatermarkUsingYiq(targetImage, watermarkImage);
            WriteImageToFile(watermarkedImage, watermarkedPath);

            using var watermarkedImageFromFile = ReadImageFromFile(watermarkedPath);

            var extractedWatermarkWithoutSaving =
                DwtWatermark.ExtractWatermarkUsingYiq(watermarkedImage, ReadImageFromFile(watermarkImagePath));
            var extractedWatermarkFromStoredFile =
                DwtWatermark.ExtractWatermarkUsingYiq(watermarkedImageFromFile, ReadImageFromFile(watermarkImagePath));

            WriteImageToFile(extractedWatermarkWithoutSaving,  "C:/watermark_test/extracted.jpg");
            WriteImageToFile(extractedWatermarkFromStoredFile, "C:/watermark_test/extracted_from_file.jpg");
        }

        private static Bitmap ReadImageFromFile(String filePath)
        {
            using var fileStreamTarget = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return new Bitmap(fileStreamTarget);
        }

        private static void WriteImageToFile(Image image, string path)
        {
            // Default jpeg compression is set to 75%
            image.Save(path, ImageFormat.Jpeg);
        }
    }
}
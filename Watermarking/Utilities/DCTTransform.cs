using System;

namespace DWT.Utilities
{
    public static class DCTTransform
    {
        private const double Sqrt2 = 1.4142135623730950488016887;

        public static void IDCT(double[] data)
        {
            var result = new double[data.Length];
            var c      = Math.PI / (2.0 * data.Length);
            var scale  = Math.Sqrt(2.0 / data.Length);

            for (var k = 0; k < data.Length; k++)
            {
                var sum = data[0] / Sqrt2;
                for (var n = 1; n < data.Length; n++)
                {
                    sum += data[n] * Math.Cos((2 * k + 1) * n * c);
                }

                result[k] = scale * sum;
            }

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = result[i];
            }
        }

        public static void DCT(double[] data)
        {
            var result = new double[data.Length];
            var c      = Math.PI / (2.0 * data.Length);
            var scale  = Math.Sqrt(2.0 / data.Length);

            for (var k = 0; k < data.Length; k++)
            {
                double sum = 0;
                for (var n = 0; n < data.Length; n++)
                {
                    sum += data[n] * Math.Cos((2.0 * n + 1.0) * k * c);
                }

                result[k] = scale * sum;
            }

            data[0] = result[0] / Sqrt2;
            for (var i = 1; i < data.Length; i++)
            {
                data[i] = result[i];
            }
        }

        public static void DCT(double[,] data)
        {
            var rows = data.GetLength(0);
            var cols = data.GetLength(1);

            for (var i = 0; i < rows; i++)
            {
                var row = data.GetRow(i);

                DCT(row);

                data.SetRow(i, row);
            }

            for (int j = 0; j < cols; j++)
            {
                var col = data.GetColumn(j);

                DCT(col);

                data.SetColumn(j, col);
            }
        }

        public static void IDCT(double[,] data)
        {
            var rows = data.GetLength(0);
            var cols = data.GetLength(1);

            for (var j = 0; j < cols; j++)
            {
                var col = data.GetColumn(j);

                IDCT(col);

                data.SetColumn(j, col);
            }

            for (var i = 0; i < rows; i++)
            {
                var row = data.GetRow(i);

                IDCT(row);

                data.SetRow(i, row);
            }
        }
    }
}
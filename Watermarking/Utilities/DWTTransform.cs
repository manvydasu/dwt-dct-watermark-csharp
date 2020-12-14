namespace DWT.Utilities
{
    public static class DWTTransform
    {
        private static double weight0 = 0.5;
        private static double weight1 = -0.5;
        private static double s0      = 0.5;
        private static double s1      = 0.5;

        public static void ForwardTransform(double[] data)
        {
            var temp = new double[data.Length];

            var h = data.Length >> 1;
            for (var i = 0; i < h; i++)
            {
                var k = (i << 1);
                temp[i]     = data[k] * s0      + data[k + 1] * s1;
                temp[i + h] = data[k] * weight0 + data[k + 1] * weight1;
            }

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = temp[i];
            }
        }

        public static double[,] ForwardTransform(double[,] data, int iterations)
        {
            var rows = data.GetLength(0);
            var cols = data.GetLength(1);

            for (var k = 0; k < iterations; k++)
            {
                var lev = 1 << k;

                var levCols = cols / lev;
                var levRows = rows / lev;

                for (var i = 0; i < levRows; i++)
                {
                    var row = data.GetRow(i);

                    ForwardTransform(row);
                    data.SetRow(i, row);
                }


                for (var j = 0; j < levCols; j++)
                {
                    var col = data.GetColumn(j);

                    ForwardTransform(col);

                    data.SetColumn(j, col);
                }
            }

            return data;
        }

        public static void InverseWaveletTransform(double[] data)
        {
            var temp = new double[data.Length];

            var h = data.Length >> 1;
            for (var i = 0; i < h; i++)
            {
                var k = (i << 1);
                temp[k]     = (data[i] * s0 + data[i + h] * weight0) / weight0;
                temp[k + 1] = (data[i] * s1 + data[i + h] * weight1) / s0;
            }

            for (var i = 0; i < data.Length; i++)
                data[i] = temp[i];
        }

        public static void InverseWaveletTransform(double[,] data, int iterations)
        {
            var rows = data.GetLength(0);
            var cols = data.GetLength(1);

            for (var k = iterations - 1; k >= 0; k--)
            {
                var lev = 1 << k;

                var levCols = cols / lev;
                var levRows = rows / lev;

                for (var j = 0; j < levCols; j++)
                {
                    var col = data.GetColumn(j);
                    InverseWaveletTransform(col);
                    data.SetColumn(j, col);

                }

                for (var i = 0; i < levRows; i++)
                {
                    var row = data.GetRow(i);
                    InverseWaveletTransform(row);
                    data.SetRow(i, row);
                }
            }
        }
    }
}
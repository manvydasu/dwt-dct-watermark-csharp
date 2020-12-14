namespace DWT.Utilities
{
    public class WatermarkUtils
    {
        public static T[,] Ll2<T>(T[,] dwtData)
        {
            var width  = dwtData.GetLength(0);
            var height = dwtData.GetLength(1);

            return dwtData.Submatrix(0, width / 4 - 1, 0, height / 4 - 1);
        }

        public static T[,] Ll1<T>(T[,] dwtData)
        {
            var width  = dwtData.GetLength(0);
            var height = dwtData.GetLength(1);

            return dwtData.Submatrix(0, width / 2 - 1, 0, height / 2 - 1);
        }

        public static T[,] Hl1<T>(T[,] dwtData)
        {
            var width  = dwtData.GetLength(0);
            var height = dwtData.GetLength(1);

            return dwtData.Submatrix(width / 2, width - 1, 0, height / 2 - 1);
        }

        public static T[,] Lh1<T>(T[,] dwtData)
        {
            var width  = dwtData.GetLength(0);
            var height = dwtData.GetLength(1);

            return dwtData.Submatrix(0, width / 2 - 1, height / 2 - 1, height - 1);
        }

        public static T[,] Hh1<T>(T[,] dwtData)
        {
            var width  = dwtData.GetLength(0);
            var height = dwtData.GetLength(1);

            return dwtData.Submatrix(width / 2 - 1, width - 1, height / 2 - 1, height - 1);
        }

        public static void ResetHl1Subband<T>(T[,] dwtData)
        {
            var width  = dwtData.GetLength(0);
            var height = dwtData.GetLength(1);

            for (int i = width / 2 - 1; i < width - 1; i++)
            {
                for (int j = 0; j < height / 2 - 1; j++)
                {
                    dwtData[i, j] = default(T);
                }
            }
        }

        public static void ResetLh1Subband<T>(T[,] dwtData)
        {
            var width  = dwtData.GetLength(0);
            var height = dwtData.GetLength(1);

            for (int i = 0; i < width / 2 - 1; i++)
            {
                for (int j = height / 2 - 1; j < height - 1; j++)
                {
                    dwtData[i, j] = default(T);
                }
            }
        }

        public static void ResetHh1Subband<T>(T[,] dwtData)
        {
            var width  = dwtData.GetLength(0);
            var height = dwtData.GetLength(1);

            for (int i = width / 2 - 1; i < width - 1; i++)
            {
                for (int j = height / 2 - 1; j < height - 1; j++)
                {
                    dwtData[i, j] = default(T);
                }
            }
        }

        public static void ApplyLl1SubBand(double[,] target, double[,] subBandData)
        {
            var width  = target.GetLength(0);
            var height = target.GetLength(1);

            for (int x = 0; x < width / 2; x++)
            {
                for (int y = 0; y < height / 2; y++)
                {
                    target[x, y] = subBandData[x, y];
                }
            }
        }
    }
}
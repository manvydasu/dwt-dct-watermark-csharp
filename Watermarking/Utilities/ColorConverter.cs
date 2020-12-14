namespace DWT.Utilities
{
    public static class ColorConverter
    {
        public static (double[,] y, double[,] i, double[,] q) ConvertRgbToYiq(double[,] r, double[,] g, double[,] b)
        {
            var rowsCount    = r.GetLength(0);
            var columnsCount = r.GetLength(1);

            var y = new double[r.GetLength(0), r.GetLength(1)];
            var i = new double[g.GetLength(0), g.GetLength(1)];
            var q = new double[b.GetLength(0), b.GetLength(1)];


            for (int j = 0; j < rowsCount; j++)
            {
                for (int k = 0; k < columnsCount; k++)
                {
                    y[j, k] = 0.299 * r[j, k]                   + 0.587 * g[j, k] + 0.114 * b[j, k];
                    i[j, k] = 0.596 * r[j, k]                   - 0.274 * g[j, k] - 0.322 * b[j, k];
                    q[j, k] = 0.211 * r[j, k] - 0.522 * g[j, k] + 0.311 * b[j, k];
                }
            }

            return (y, i, q);
        }

        public static (double[,] y, double[,] i, double[,] q) ConvertYiqToRgb(double[,] y, double[,] i, double[,] q)
        {
            var rowsCount    = y.GetLength(0);
            var columnsCount = y.GetLength(1);

            var r = new double[y.GetLength(0), y.GetLength(1)];
            var g = new double[i.GetLength(0), i.GetLength(1)];
            var b = new double[q.GetLength(0), q.GetLength(1)];


            for (int j = 0; j < rowsCount; j++)
            {
                for (int k = 0; k < columnsCount; k++)
                {
                    r[j, k] = y[j, k]                   + 0.956 * i[j, k] + 0.619 * q[j, k];
                    g[j, k] = y[j, k]                   - 0.272 * i[j, k] - 0.647 * q[j, k];
                    b[j, k] = y[j, k] - 1.106 * i[j, k] + 1.702 * q[j, k];
                }
            }

            return (r, g, b);
        }
    }
}
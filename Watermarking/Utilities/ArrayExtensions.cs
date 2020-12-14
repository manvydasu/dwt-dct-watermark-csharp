using System;

namespace DWT.Utilities
{
    public static class ArrayExtensions
    {
        public static T[,] Submatrix<T>(this T[,] source,
                                        int       startRow, int endRow, int startColumn, int endColumn)
        {
            return Submatrix(source, null, startRow, endRow, startColumn, endColumn);
        }

        private static T[,] Submatrix<T>(this T[,] source,   T[,] destination,
                                         int       startRow, int  endRow, int startColumn, int endColumn)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var rows = source.GetLength(0);
            var cols = source.GetLength(1);

            if ((startRow    > endRow) || (startColumn > endColumn) || (startRow  < 0)     ||
                (startRow    >= rows)  || (endRow      < 0)         || (endRow    >= rows) ||
                (startColumn < 0)      || (startColumn >= cols)     || (endColumn < 0)     ||
                (endColumn   >= cols))
            {
                throw new ArgumentException("Arguments out of range.");
            }

            if (destination == null)
            {
                destination = new T[endRow - startRow + 1, endColumn - startColumn + 1];
            }

            for (var i = startRow; i <= endRow; i++)
            {
                for (var j = startColumn; j <= endColumn; j++)
                {
                    destination[i - startRow, j - startColumn] = source[i, j];
                }
            }

            return destination;
        }
        
        public static T[] GetColumn<T>(this T[,] m, int index)
        {
            var rows   = m.GetLength(0);
            var column = new T[rows];

            for (var i = 0; i < column.Length; i++)
            {
                column[i] = m[i, index];
            }

            return column;
        }

        public static T[] GetRow<T>(this T[,] data, int index)
        {
            var row = new T[data.GetLength(1)];

            for (int i = 0; i < row.Length; i++)
            {
                row[i] = data[index, i];
            }

            return row;
        }

        public static T[,] SetRow<T>(this T[,] data, int index, T[] row)
        {
            for (var i = 0; i < row.Length; i++)
            {
                data[index, i] = row[i];
            }

            return data;
        }

        public static T[,] SetColumn<T>(this T[,] data, int index, T[] column)
        {
            for (var i = 0; i < column.Length; i++)
            {
                data[i, index] = column[i];
            }


            return data;
        }
        
        public static void ApplyDataBlock<T>(this T[,] target, T[,] data, int startRow, int startColumn)
        {
            for (var i = 0; i < data.GetLength(0); i++)
            {
                for (var j = 0; j < data.GetLength(1); j++)
                {
                    target[startRow + i, j + startColumn] = data[i, j];
                }
            }
        }
    }
}
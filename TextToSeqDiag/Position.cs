using System;

namespace TextToSeqDiag
{
    public sealed class Position
    {
        public PositionKind Kind { get; private set; }
        public int Column { get; private set; }
        public int Column2 { get; private set; }
        public int Row { get; private set; }

        public static Position Message(int source, int destination, int row)
        {
            if (source < 0) throw new ArgumentOutOfRangeException();
            if (row < 0) throw new ArgumentOutOfRangeException();
            if (source >= destination) throw new ArgumentOutOfRangeException();
            return new Position
            {
                Column = source,
                Column2 = destination,
                Row = row,
                Kind = PositionKind.Message,
            };
        }

        public static Position OneColumn(int column, int row)
        {
            if (column < 0) throw new ArgumentOutOfRangeException();
            if (row < 0) throw new ArgumentOutOfRangeException();
            return new Position
            {
                Column = column,
                Row = row,
                Kind = PositionKind.OneColumn
            };
        }

        public static Position Body(int column)
        {
            if (column < 0) throw new ArgumentOutOfRangeException();
            return new Position
            {
                Column = column,
                Kind = PositionKind.Body
            };
        }
    }
}
namespace TextToSeqDiag
{
    public sealed class Position
    {
        public PositionKind Kind { get; set; }
        public int Column { get; set; }
        public int Column2 { get; set; }
        public int Row { get; set; }
    }
}
namespace BenchmarkTest.DTO
{
    public class CurrentRollDTO
    {
        public long UserId { get; set; }
        public int CurrentFrame { get; set; }
        public int CurrentCounter { get; set; }
        public int PinsKnocked { get; set; }
        public bool IsStrike { get; set; }
        public bool IsSpare { get; set; }
    }
}

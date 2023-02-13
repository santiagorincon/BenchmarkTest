namespace BenchmarkTest.DTO
{
    public class ScoreDTO
    {
        public long GameId { get; set; }
        public int Score { get; set; }
        public string WinningUser { get; set; }
        public bool IsFinished { get; set; }
    }
}

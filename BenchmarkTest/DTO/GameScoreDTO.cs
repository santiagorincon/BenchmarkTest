using BenchmarkTest.Models;

namespace BenchmarkTest.DTO
{
    public class GameScoreDTO
    {
        public GameScoreDTO()
        {

        }

        // This method is to convert a data base object into a DTO
        public GameScoreDTO(GameScore score)
        {
            Id = score.Id;
            GameId = score.GameId;
            UserId = score.UserId;
            Frame = score.Frame;
            Score = score.Score;
            IsClosed = score.IsClosed;
            PendingRolls = score.PendingRolls;
            IsStrike = score.IsStrike;
            IsSpare = score.IsSpare;
        }
        public long Id { get; set; }
        public long GameId { get; set; }
        public long UserId { get; set; }
        public int Frame { get; set; }
        public int Score { get; set; }
        public bool IsClosed { get; set; }
        public int PendingRolls { get; set; }
        public bool IsStrike { get; set; }
        public bool IsSpare { get; set; }
    }
}

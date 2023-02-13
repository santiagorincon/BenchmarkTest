using BenchmarkTest.Models;

namespace BenchmarkTest.DTO
{
    public class RollDTO
    {
        public RollDTO()
        {

        }

        // This method is to convert a data base object into a DTO
        public RollDTO(Roll roll)
        {
            Id = roll.Id;
            GameId = roll.GameId;
            Frame = roll.Frame;
            Counter = roll.Counter;
            PinsKnocked = roll.PinsKnocked;
            IsSpare = roll.IsSpare;
            IsStrike = roll.IsStrike;
            UserId = roll.UserId;
        }

        public long Id { get; set; }
        public long GameId { get; set; }
        public int Frame { get; set; }
        public int Counter { get; set; }
        public int PinsKnocked { get; set; }
        public bool IsStrike { get; set; }
        public bool IsSpare { get; set; }
        public long UserId { get; set; }
    }
}

using BenchmarkTest.Models;

namespace BenchmarkTest.DTO
{
    public class GameDTO
    {
        public GameDTO()
        {

        }

        // This method is to get the current roll, it was included on DTO because it only depends of the data loaded itself
        public CurrentRollDTO GetCurrentRoll (long userId)
        {
            CurrentRollDTO current = new CurrentRollDTO
            {
                UserId = userId,
                CurrentFrame = 0,
                CurrentCounter = 0,
                PinsKnocked = 0,
                IsSpare = false,
                IsStrike = false
            };

            RollDTO? lastRoll = Rolls.Where(r => r.UserId == userId).OrderByDescending(r => r.Frame).ThenByDescending(r => r.Counter).FirstOrDefault();
            
            if(lastRoll is null)
            {
                var gameUser = Users.Where(u => u.UserId == userId).FirstOrDefault();

                // If there is no last roll and the user is not in the game, the return value is null because it is catched outside to throw an exception
                if(gameUser is null)
                {
                    return null;
                }
            }
            else
            {
                current.CurrentFrame = lastRoll.Frame;
                current.CurrentCounter = lastRoll.Counter;
                current.PinsKnocked = lastRoll.PinsKnocked;
                current.IsSpare = lastRoll.IsSpare;
                current.IsStrike = lastRoll.IsStrike;
            }

            // If there is no last roll it returns a new one with current frame, current counter and pins knocked with value 0
            return current;
        }

        // This method is to convert a data base object into a DTO
        public GameDTO(Game game)
        {
            Id = game.Id;
            StartDate = game.StartDate;
            EndDate = game.EndDate;
            PartialScore = game.PartialScore;
            FinalScore = game.FinalScore;
            WinnerUserId = game.WinnerUserId;
        }


        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? PartialScore { get; set; }
        public int? FinalScore { get; set; }
        public long? WinnerUserId { get; set; }
        public List<RollDTO> Rolls { get; set; }
        public List<GameUserDTO> Users { get; set; }
        public List<GameScoreDTO> Scores { get; set; }
    }
}
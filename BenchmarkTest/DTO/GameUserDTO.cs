using BenchmarkTest.Models;

namespace BenchmarkTest.DTO
{
    public class GameUserDTO
    {
        public GameUserDTO()
        {

        }

        // This method is to convert a data base object into a DTO
        public GameUserDTO(GameUser user)
        {
            GameId = user.GameId;
            UserId = user.UserId;
        }
        public long GameId { get; set; }
        public long UserId { get; set; }
    }
}

using BenchmarkTest.DTO;

namespace BenchmarkTest.Interfaces
{
    public interface GameInterface
    {
        GameDTO getGameById(long gameId);
        List<GameDTO> getGameList();
        GameDTO newGame(GameRequestDTO gameRequest);
        GameDTO updateGame(GameDTO updatedGame);
        ScoreDTO getScoreById(long gameId);
    }
}

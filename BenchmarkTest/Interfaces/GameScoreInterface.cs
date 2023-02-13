using BenchmarkTest.DTO;

namespace BenchmarkTest.Interfaces
{
    public interface GameScoreInterface
    {
        GameScoreDTO newScore(GameScoreDTO scoreRequest);
        GameScoreDTO updateScore(GameScoreDTO updatedScore);
        List<GameScoreDTO> getScores(long gameId);
        GameScoreDTO getScoreById(long scoreId);
    }
}

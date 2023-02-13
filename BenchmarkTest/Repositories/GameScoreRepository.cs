using BenchmarkTest.DTO;
using BenchmarkTest.Interfaces;
using BenchmarkTest.Models;

namespace BenchmarkTest.Repositories
{
    public class GameScoreRepository : GameScoreInterface
    {
        // Database context and additional repositories are injected using dependency injection
        private readonly BenchmarkTestContext _dbContext;
        private readonly UserInterface _userRepository;
        private readonly GameInterface _gameRepository;

        public GameScoreRepository(BenchmarkTestContext dbContext, UserInterface userRepository, GameInterface gameRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
        }

        public GameScoreDTO getScoreById(long scoreId)
        {
            var score = _dbContext.GameScores.Where(u => u.Id == scoreId)
                .Select(u => new GameScoreDTO(u))
                .FirstOrDefault();

            if (score == null)
            {
                throw new Exception(string.Format("Score not found: {0}", scoreId));
            }

            return score;
        }

        public List<GameScoreDTO> getScores(long gameId)
        {
            GameDTO game = _gameRepository.getGameById(gameId);
            var scores = _dbContext.GameScores
                .Where(u => u.GameId == gameId)
                .Select(u => new GameScoreDTO(u))
                .ToList();

            return scores;
        }

        public GameScoreDTO newScore(GameScoreDTO scoreRequest)
        {
            GameScore score = new GameScore();

            GameDTO game = _gameRepository.getGameById(scoreRequest.GameId);
            UserDTO user = _userRepository.getUserById(scoreRequest.UserId);

            score.GameId = scoreRequest.GameId;
            score.UserId = scoreRequest.UserId;
            score.Frame = scoreRequest.Frame;
            score.Score = scoreRequest.Score;
            score.IsClosed = scoreRequest.IsClosed;
            score.PendingRolls = scoreRequest.PendingRolls;
            score.IsStrike = scoreRequest.IsStrike;
            score.IsSpare = scoreRequest.IsSpare;

            _dbContext.GameScores.Add(score);
            _dbContext.SaveChanges();
            GameScoreDTO scoreResult = getScoreById(score.Id);
            return scoreResult;
        }

        public GameScoreDTO updateScore(GameScoreDTO updatedScore)
        {
            GameScore score = _dbContext.GameScores.Find(updatedScore.Id);
            if (score is null)
            {
                throw new Exception(string.Format("Score not found: {0}", updatedScore.Id));
            }

            score.GameId = updatedScore.GameId;
            score.UserId = updatedScore.UserId;
            score.Frame = updatedScore.Frame;
            score.Score = updatedScore.Score;
            score.IsClosed = updatedScore.IsClosed;
            score.PendingRolls = updatedScore.PendingRolls;
            score.IsStrike = updatedScore.IsStrike;
            score.IsSpare = updatedScore.IsSpare;

            _dbContext.SaveChanges();
            GameScoreDTO scoreResult = getScoreById(score.Id);
            return scoreResult;
        }
    }
}

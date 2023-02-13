using BenchmarkTest.DTO;
using BenchmarkTest.Interfaces;
using BenchmarkTest.Models;
using System.Data.Entity;

namespace BenchmarkTest.Repositories
{
    public class GameRepository : GameInterface
    {
        // Database context and additional repositories are injected using dependency injection
        private readonly BenchmarkTestContext _dbContext;
        private readonly UserInterface _userRepository;

        public GameRepository(BenchmarkTestContext dbContext, UserInterface userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public GameDTO getGameById(long gameId)
        {
            var game = _dbContext.Games
                .Where(g => g.Id == gameId)
                .Select(g => new GameDTO(g))
                .FirstOrDefault();

            if(game == null)
            {
                throw new Exception(string.Format("Game not found: {0}", gameId));
            }

            game.Rolls = _dbContext.Rolls.Where(r => r.GameId == gameId).Select(r => new RollDTO(r)).ToList();
            game.Users = _dbContext.GameUsers.Where(u => u.GameId == gameId).Select(u => new GameUserDTO(u)).ToList();
            game.Scores = _dbContext.GameScores.Where(s => s.GameId == gameId).Select(u => new GameScoreDTO(u)).ToList();
            return game;
        }

        public List<GameDTO> getGameList()
        {
            var games = _dbContext.Games
                .Select(g => new GameDTO(g))
                .ToList();

            return games;
        }

        public ScoreDTO getScoreById(long gameId)
        {
            ScoreDTO score = new ScoreDTO();
            GameDTO game = getGameById(gameId);
            if (game is null)
            {
                throw new Exception(string.Format("Game not found: {0}", gameId));
            }

            score.GameId = gameId;
            score.Score = game.PartialScore is null ? 0 : (int)game.PartialScore;
            score.WinningUser = game.WinnerUserId is null ? string.Empty : _userRepository.getUserById((long)game.WinnerUserId).Name;
            score.IsFinished = game.EndDate != null;
            return score;
        }

        // This method is to create a new game, I added 3 posibilities in request object to do this
        public GameDTO newGame(GameRequestDTO gameRequest)
        {
            Game game = new Game();
            game.StartDate = DateTime.Now;
            _dbContext.Games.Add(game);

            // 1. Including userids of users that were created before, it search that ids into the database and attach the users into the new game
            if (gameRequest.UserIds.Any())
            {
                foreach(long userid in gameRequest.UserIds)
                {
                    var user = _userRepository.getUserById(userid);
                    if(user is null)
                    {
                        throw new Exception(string.Format("User not found: {0}", userid));
                    }
                    GameUser gameUser = new GameUser();
                    gameUser.UserId = userid;
                    game.GameUsers.Add(gameUser);
                }
            }
            // 2. Including nicknames of users that were created before, it search that nicknames into the database and attach the users into the new game
            else if (gameRequest.UserNicknames.Any())
            {
                foreach (string nickname in gameRequest.UserNicknames)
                {
                    var user = _userRepository.getUserByNickname(nickname);
                    if (user is null)
                    {
                        throw new Exception(string.Format("Nickname not found: {0}", nickname));
                    }
                    GameUser gameUser = new GameUser();
                    gameUser.UserId = (long)user.Id;
                    game.GameUsers.Add(gameUser);
                }
            }
            // Including user names, those names are used to create new users into the database, with auto-generated nicknames
            else if (gameRequest.NewUserNames.Any())
            {
                foreach (string name in gameRequest.NewUserNames)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new Exception("Name can't be null or empty");
                    }

                    UserDTO newUser = new UserDTO
                    {
                        Name = name,
                        Nickname = _userRepository.getValidNickname(name)
                    };

                    newUser = _userRepository.newUser(newUser);

                    GameUser gameUser = new GameUser();
                    gameUser.UserId = (long)newUser.Id;
                    game.GameUsers.Add(gameUser);
                }
            }

            _dbContext.SaveChanges();
            GameDTO gameResult = getGameById(game.Id);
            return gameResult;
        }

        public GameDTO updateGame(GameDTO updatedGame)
        {
            Game game = _dbContext.Games.Find(updatedGame.Id);
            if (game is null)
            {
                throw new Exception(string.Format("Game not found: {0}", updatedGame.Id));
            }

            game.PartialScore = updatedGame.PartialScore;
            game.FinalScore = updatedGame.FinalScore;
            game.EndDate = updatedGame.EndDate;
            game.WinnerUserId = updatedGame.WinnerUserId;

            _dbContext.SaveChanges();
            GameDTO gameResult = getGameById(game.Id);
            return gameResult;
        }
    }
}

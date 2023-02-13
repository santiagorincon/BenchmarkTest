using BenchmarkTest.DTO;
using BenchmarkTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Mocks
{
    // This is a mock to simulate game repository, it will be used in the unit tests
    public class GameMock : GameInterface
    {
        private readonly List<GameDTO> _games = new List<GameDTO>();
        private readonly List<UserDTO> _users = new List<UserDTO>();   

        public GameMock()
        {
            _games = new List<GameDTO>()
            {
                new GameDTO() { Id = 1, StartDate = new DateTime(2023,2,11), EndDate = new DateTime(2023,2,11), PartialScore = 200, FinalScore = 200, WinnerUserId = 1, Users = new List<GameUserDTO>(){ new GameUserDTO() {GameId = 1, UserId = 1 } } },
                new GameDTO() { Id = 2, StartDate = new DateTime(2023,2,11), EndDate = new DateTime(2023,2,11), PartialScore = 50, FinalScore = 50, WinnerUserId = 2, Users = new List<GameUserDTO>(){ new GameUserDTO() {GameId = 2, UserId = 2 } } },
                new GameDTO() { Id = 3, StartDate = new DateTime(2023,2,11), EndDate = new DateTime(2023,2,11), PartialScore = 126, FinalScore = 126, WinnerUserId = 1, Users = new List < GameUserDTO >() { new GameUserDTO() { GameId = 3, UserId = 1 } }},
                new GameDTO() { Id = 4, StartDate = new DateTime(2023,2,11), EndDate = new DateTime(2023,2,11), PartialScore = 228, FinalScore = 228, WinnerUserId = 2, Users = new List < GameUserDTO >() { new GameUserDTO() { GameId = 4, UserId = 2 } }}
            };

            _users = new List<UserDTO>()
            {
                new UserDTO() { Id = 1, Name = "Santiago", Nickname = "Santi"},
                new UserDTO() { Id = 2, Name = "Rick", Nickname = "Rick86"}
            };
        }
        public GameDTO getGameById(long gameId)
        {
            var game = _games.FirstOrDefault(a => a.Id == gameId);
            if (game == null)
            {
                throw new Exception(string.Format("Game not found: {0}", gameId));
            }
            return game;
        }

        public List<GameDTO> getGameList()
        {
            return _games.ToList();
        }

        public ScoreDTO getScoreById(long gameId)
        {
            var game = _games.FirstOrDefault(a => a.Id == gameId);
            if (game == null)
            {
                throw new Exception(string.Format("Game not found: {0}", gameId));
            }

            ScoreDTO score = new ScoreDTO();
            score.GameId = gameId;
            score.Score = game.PartialScore is null ? 0 : (int)game.PartialScore;
            score.WinningUser = game.WinnerUserId is null ? string.Empty : _users.FirstOrDefault(u => u.Id == game.WinnerUserId).Name;
            score.IsFinished = game.EndDate != null;
            return score;
        }

        public GameDTO newGame(GameRequestDTO gameRequest)
        {
            long maxId = _games.Select(g => g.Id).Max();
            GameDTO newGame = new GameDTO();
            newGame.Id = maxId + 1;
            newGame.StartDate = DateTime.Now;

            if (gameRequest.UserIds.Any())
            {
                foreach (long userid in gameRequest.UserIds)
                {
                    var user = _users.FirstOrDefault(u => u.Id == userid);
                    if (user is null)
                    {
                        throw new Exception(string.Format("User not found: {0}", userid));
                    }
                    GameUserDTO gameUser = new GameUserDTO();
                    gameUser.UserId = userid;
                    gameUser.GameId = newGame.Id;
                    newGame.Users.Add(gameUser);
                }
            }

            _games.Add(newGame);
            return newGame;
        }

        public GameDTO updateGame(GameDTO updatedGame)
        {
            GameDTO game = _games.FirstOrDefault(g => g.Id == updatedGame.Id);
            if (game is null)
            {
                throw new Exception(string.Format("Game not found: {0}", updatedGame.Id));
            }

            game.PartialScore = updatedGame.PartialScore;
            game.FinalScore = updatedGame.FinalScore;
            game.EndDate = updatedGame.EndDate;
            game.WinnerUserId = updatedGame.WinnerUserId;

            return game;
        }
    }
}

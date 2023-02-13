using BenchmarkTest.DTO;
using BenchmarkTest.Interfaces;
using BenchmarkTest.Models;

namespace BenchmarkTest.Repositories
{
    public class RollRepository : RollInterface
    {
        // Database context and additional repositories are injected using dependency injection
        private readonly BenchmarkTestContext _dbContext;
        private readonly UserInterface _userRepository;
        private readonly GameInterface _gameRepository;
        private readonly GameScoreInterface _gameScoreRepository;

        public RollRepository(BenchmarkTestContext dbContext, UserInterface userRepository, GameInterface gameRepository, GameScoreInterface gameScoreRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _gameScoreRepository = gameScoreRepository;
        }

        // This is a very complex method, because it create the new roll (verifying that the new roll request is valid), calculate the new frame and counter, and updating the score in all the tables related
        public RollDTO newRoll(RollRequestDTO rollRequest)
        {
            GameDTO game = _gameRepository.getGameById(rollRequest.GameId);
            if(game is null)
            {
                throw new Exception(string.Format("Game not found: {0}", rollRequest.GameId));
            }

            UserDTO user = _userRepository.getUserById(rollRequest.UserId);
            if(user is null)
            {
                throw new Exception(string.Format("User not found: {0}", rollRequest.UserId));
            }

            // This is to get the last roll for the user that is trying to register a new one
            CurrentRollDTO currentRoll = game.GetCurrentRoll(rollRequest.UserId);
            if (currentRoll is null)
            {
                throw new Exception(string.Format("User with Id #{0} is not in the game", rollRequest.UserId));
            }
            else
            {
                Roll newRoll = new Roll();
                newRoll.GameId = rollRequest.GameId;
                newRoll.UserId = rollRequest.UserId;
                newRoll.IsStrike = false;
                newRoll.IsSpare = false;

                // This switch is to cast the 'PinsKnocked' into a valid int number to store in database
                // It also validate if the new roll is a strike or a spare
                rollRequest.PinsKnocked = rollRequest.PinsKnocked.Trim();
                switch (rollRequest.PinsKnocked)
                {
                    case "X":
                        newRoll.PinsKnocked = 10;
                        newRoll.IsStrike = true;
                        break;
                    case "-":
                        newRoll.PinsKnocked = 0;
                        break;
                    case "/":
                        newRoll.PinsKnocked = 10 - currentRoll.PinsKnocked;
                        newRoll.IsSpare = true;
                        break;
                    default:
                        try
                        {
                            int pins = int.Parse(rollRequest.PinsKnocked);
                            newRoll.PinsKnocked = pins;
                        }
                        catch 
                        {
                            throw new Exception(string.Format("'{0}' is not a valid value for PinsKnocked", rollRequest.PinsKnocked));
                        }
                        break;
                }

                if(newRoll.PinsKnocked == 10)
                {
                    newRoll.IsStrike = true;
                }

                // This section is to validate the max number of pins that could be knocked in this roll
                int maxPins = 10;
                if(currentRoll.CurrentCounter == 1 && !currentRoll.IsStrike)
                {
                    maxPins = 10 - currentRoll.PinsKnocked;
                }
                if (newRoll.PinsKnocked > maxPins)
                {
                    throw new Exception(string.Format("'{0}' is not a valid value for PinsKnocked, you only can knok {1} pins", rollRequest.PinsKnocked, maxPins));
                }

                if (currentRoll.CurrentFrame == 0)
                {
                    // When the currentRoll value is 0, it means it's the first roll of the game for this user
                    newRoll.Frame = 1;
                    newRoll.Counter = 1;
                }
                else
                {
                    if(currentRoll.CurrentCounter == 1 && !currentRoll.IsStrike)
                    {
                        // This section is when the user is throwing his second roll in a frame
                        newRoll.Frame = currentRoll.CurrentFrame;
                        newRoll.Counter = 2;

                        // It evaluate if the new roll is a spare
                        newRoll.IsSpare = newRoll.PinsKnocked == maxPins;
                    }
                    else
                    {
                        if (currentRoll.CurrentFrame < 10)
                        {
                            // This section is when the user is starting a new frame
                            newRoll.Frame = currentRoll.CurrentFrame + 1;
                            newRoll.Counter = 1;
                        }
                        else
                        {
                            // This section is when the user is in his last frame
                            RollDTO firstRollLastFrame = game.Rolls.Where(r => r.UserId == rollRequest.UserId && r.Frame == 10 && r.Counter == 1).FirstOrDefault();

                            newRoll.Frame = currentRoll.CurrentFrame;
                            newRoll.Counter = currentRoll.CurrentCounter + 1;
                            newRoll.IsSpare = newRoll.PinsKnocked == maxPins && !currentRoll.IsStrike;

                            // It allows a third roll in the last frame only if there is a strike or a spare on it
                            if ( (newRoll.Counter == 3 && !firstRollLastFrame.IsStrike && !currentRoll.IsSpare) || newRoll.Counter > 3)
                            {
                                throw new Exception("The game is over");
                            }
                        }
                    }
                }

                _dbContext.Rolls.Add(newRoll);
                _dbContext.SaveChanges();

                // It gets all the scores related with the game
                List<GameScoreDTO> scores = _gameScoreRepository.getScores(rollRequest.GameId);

                // It gets the score related with the current frame for the requested user
                GameScoreDTO currentScore = scores.Where(s => s.Frame == newRoll.Frame && s.UserId == newRoll.UserId).FirstOrDefault();

                if(currentScore is null)
                {
                    // A new Frame is starting, so it creates a new score for that one
                    currentScore = new GameScoreDTO();
                    currentScore.GameId = rollRequest.GameId;
                    currentScore.UserId = rollRequest.UserId;
                    currentScore.Frame = newRoll.Frame;
                    currentScore.Score = newRoll.PinsKnocked;

                    // This is to know if there is pending rolls to add into this score (Strike must add next 2 rolls and Spare next 1 roll)
                    currentScore.IsClosed = false;

                    // This ones indicate if the frame was a Strike or a Spare
                    currentScore.IsSpare = false;
                    currentScore.IsStrike = false;

                    // Each score has a 'PendingRolls' value, it allows to add the score for the next rolls into this one
                    if (newRoll.IsStrike)
                    {
                        // In case of Strike, it must add the score for the next 2 rolls, it's not needed in last frame
                        currentScore.PendingRolls = (newRoll.Frame < 10) ? 2 : 0;
                        currentScore.IsStrike = true;
                    }

                    if (newRoll.IsSpare)
                    {
                        // In case of Spare, it must add the score for the next roll
                        currentScore.PendingRolls = 1;
                        currentScore.IsSpare = true;
                    }

                    currentScore = _gameScoreRepository.newScore(currentScore);
                    scores.Add(currentScore);
                }
                else
                {
                    // When the frame was already started it only add the Pins knocked to the score, and register the spare if is needed
                    currentScore.Score += newRoll.PinsKnocked;

                    if (newRoll.IsSpare)
                    {
                        currentScore.PendingRolls = 1;
                        currentScore.IsSpare = true;

                        if(newRoll.Frame == 10)
                        {
                            currentScore.PendingRolls = 0;
                            currentScore.IsClosed = true;
                        }
                    }
                    else
                    {
                        currentScore.IsClosed = true;
                    }

                    currentScore = _gameScoreRepository.updateScore(currentScore);
                }

                // Once the score is ready, it gets the scores that need to be updated with the current score, it means, the previous Strikes or Spares that still need to add the current score
                List<GameScoreDTO> updatedScores = scores.Where(s => s.Frame < currentScore.Frame && !s.IsClosed && s.UserId == newRoll.UserId).ToList();
                foreach(GameScoreDTO score in updatedScores)
                {
                    score.Score += newRoll.PinsKnocked;
                    score.PendingRolls -= 1;

                    if(score.PendingRolls == 0)
                    {
                        score.IsClosed = true;
                    }

                    _gameScoreRepository.updateScore(score);
                }

                scores = _gameScoreRepository.getScores(rollRequest.GameId);

                // This section get the max score from all the users in the game, to update that one into the game data
                int maxScore = 0;
                long winnerId = 0;
                foreach(GameUserDTO gameUser in game.Users)
                {
                    int userScore = scores.Where(s => s.UserId == gameUser.UserId).Select(s => s.Score).Sum();

                    if(userScore > maxScore)
                    {
                        maxScore = userScore;
                        winnerId = gameUser.UserId;
                    }
                }

                game.PartialScore = maxScore;
                game.WinnerUserId = winnerId;

                // If it is the last roll of the game it close the game updating the final score and end date into the game
                if (scores.Where(s => s.IsClosed).Count() == (game.Users.Count() * 10) )
                {
                    game.FinalScore = game.PartialScore;
                    game.EndDate = DateTime.Now;
                }

                _gameRepository.updateGame(game);

                RollDTO rollResult = new RollDTO(newRoll);
                return rollResult;
            }

        }
    }
}

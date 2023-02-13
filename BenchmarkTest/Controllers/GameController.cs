using BenchmarkTest.Cache;
using BenchmarkTest.DTO;
using BenchmarkTest.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Transactions;

namespace BenchmarkTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        // These repositories are injected
        private readonly GameInterface _gameRepository;
        private readonly RollInterface _rollRepository;
        private readonly IMemoryCache _memoryCache;

        public GameController(GameInterface gameRepository, RollInterface rollRepository, IMemoryCache memoryCache)
        {
            _gameRepository = gameRepository;
            _rollRepository = rollRepository;
            _memoryCache = memoryCache;
        }

        // This is the service to create a new game, the URL is the base url for the controller (/Game)
        [HttpPost(Name = "NewGame")]
        public IActionResult NewGame([FromBody] GameRequestDTO gameRequest)
        {
            using (var scope = new TransactionScope())
            {
                GameDTO game = _gameRepository.newGame(gameRequest);
                scope.Complete();
                return CreatedAtAction(nameof(NewGame), game);
            }
        }

        // This is the service to register a new roll, the URL is '/Game/Roll'
        [HttpPost("roll", Name = "NewRoll")]
        public IActionResult NewRoll([FromBody] RollRequestDTO rollRequest)
        {
            using (var scope = new TransactionScope())
            {
                RollDTO roll = _rollRepository.newRoll(rollRequest);
                scope.Complete();
                return CreatedAtAction(nameof(NewRoll), roll);
            }
        }

        // This is the service to get the score from specific game, the URL is '/Game/Score/{id}', where {id} is the game id
        [HttpGet("score/{id}", Name = "GetGameScore")]
        public IActionResult GetGameScore(long id)
        {
            // Here is where Cache works, The validations are: - The Score is already cached. - The cached value assert with the game requested
            // If both conditions are true, it only returns the cached value
            if (!_memoryCache.TryGetValue(CacheKeys.Score, out ScoreDTO cacheScore) || cacheScore.GameId != id)
            {

                // If there is no cached value or the cached score is from a different game, it gets the value from repository and store that in cache
                cacheScore = _gameRepository.getScoreById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));
                _memoryCache.Set(CacheKeys.Score, cacheScore, cacheEntryOptions);
            }
            return new OkObjectResult(cacheScore);
        }
    }
}
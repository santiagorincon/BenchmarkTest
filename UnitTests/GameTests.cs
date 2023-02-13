using BenchmarkTest.Controllers;
using BenchmarkTest.DTO;
using BenchmarkTest.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Mocks;

namespace UnitTests
{
    public class GameTests
    {
        private readonly GameController _controller;
        private readonly GameInterface _service;
        public GameTests()
        {
            // This is to include Cache into tests
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            // It creates the controller using the mocked repository
            _service = new GameMock();
            _controller = new GameController(_service, null, memoryCache);
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestingGetGameScore()
        {
            // Act
            var result = _controller.GetGameScore(1);

            // Assert
            // Type validation
            Assert.IsInstanceOf<OkObjectResult>(result);

            // Null validation for response
            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            // Null validation for retorned data
            var model = okObjectResult.Value as ScoreDTO;
            Assert.NotNull(model);

            // It validates the retorned data with expected data
            var actual = model.Score;
            Assert.That(actual, Is.EqualTo(200));
        }
    }
}
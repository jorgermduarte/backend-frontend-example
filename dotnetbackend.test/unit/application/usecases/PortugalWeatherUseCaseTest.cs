using dotnetbackened.adapters.apis;
using dotnetbackened.application.usecases;
using dotnetbackened.enterprise.entities;
using dotnetbackened.enterprise.interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnetbackend.test.unit.application.usecases
{
    [TestClass]
    public class PortugalWeatherUseCaseTest
    {
        private PortugalWeatherUseCase useCase;
        private Mock<IWeatherRepository> weatherRepositoryMock;
        private Mock<IDistributedCache> redisCacheMock;
        private OpenWeatherMapService openWeatherMapService;

        [TestInitialize]
        public void Setup()
        {
            weatherRepositoryMock = new Mock<IWeatherRepository>();
            redisCacheMock = new Mock<IDistributedCache>();
            openWeatherMapService = new OpenWeatherMapService(new HttpClient());

            // -> Mock the dependencies for the use case
            useCase = new PortugalWeatherUseCase(openWeatherMapService, weatherRepositoryMock.Object, redisCacheMock.Object);
        }

        [TestMethod]
        public async Task CreateNewWeather()
        {
            // Arrange
            // -> set redis mock to return null
            redisCacheMock.Setup(x => x.Get(It.IsAny<string>())).Returns((byte[])null);
            // -> set weather repository mock to return null
            weatherRepositoryMock.Setup(x => x.GetWeatherByCityAndCountryAndDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync((Weather)null);

            // Act
            var result = await useCase.GetWeather("lisbon");

            // Assert
            // -> expect the _weatherRepository.AddWeather(newWeather); to be called
            weatherRepositoryMock.Verify(x => x.AddWeather(It.IsAny<Weather>()), Times.Once);
            Assert.IsNotNull(result);
        }
    }
}

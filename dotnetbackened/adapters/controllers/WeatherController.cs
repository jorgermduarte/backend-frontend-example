using dotnetbackened.application.factories.interfaces;
using dotnetbackened.application.usecases.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotnetbackened.adapters.controllers
{
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherUseCaseFactory _weatherUseCaseFactory;
        public WeatherController(IWeatherUseCaseFactory weatherUseCaseFactory)
        {
            _weatherUseCaseFactory = weatherUseCaseFactory;
        }

        [HttpGet]
        [Route("/weather/{country}/{city}")]
        public async Task<string> GetWeather(string country, string city)
        {
            var weatherUseCase = _weatherUseCaseFactory.Create(country);
            var weatherResult = await weatherUseCase.GetWeather(city);
            return weatherResult;
        }
    }
}

using dotnetbackened.adapters.attributes;
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

        [HttpDelete]
        [Route("/manage/weather/{country}/{city}/{hour}")]
        [BasicAuth(BasicAuthAttribute.EUserRoles.Admin, BasicAuthAttribute.EUserRoles.Manager)]
        public async Task<IActionResult> DeleteWeather(string country, string city, int hour)
        {
            var weatherUseCase = _weatherUseCaseFactory.Create(country);
            var weatherResult = await weatherUseCase.DeleteWeather(country,city, hour);
            return weatherResult ? Ok("Deleted Successfully") : throw new KeyNotFoundException();
        }

    }
}

using dotnetbackened.adapters.apis;
using dotnetbackened.application.usecases.interfaces;
using dotnetbackened.enterprise.entities;
using dotnetbackened.enterprise.interfaces;

namespace dotnetbackened.application.usecases
{
    public class PortugalWeatherUseCase: IWeatherUseCase
    {
        private readonly OpenWeatherMapService _openWeatherMapService;

        private readonly IWeatherRepository _weatherRepository;

        private static readonly string[] _portugueseCitiesLowered = new string[] { "lisbon", "porto", "vila nova de gaia", "amadora", "braga", "funchal", "coimbra", "setúbal", "quarteira", "aveiro", "viseu", "leiria", "faro", "barreiro", "covilhã", "viana do castelo", "figueira da foz", "ponta delgada", "guarda", "santarém" };

        public PortugalWeatherUseCase(OpenWeatherMapService openWeatherMapService, IWeatherRepository weatherRepository)
        {
            _openWeatherMapService = openWeatherMapService;
            _weatherRepository = weatherRepository;
        }

        public async Task<string> GetWeather(string city)
        {
            if(_portugueseCitiesLowered.Contains(city.ToLower()))
            {
                var weatherData = await _openWeatherMapService.GetWeather(city);
                var currentHour = DateTime.Now.Hour;
                // remove hours , minutes and seconds from the date
                // based on today's day create a date wtih 00:00:00 hours
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                var country = "Portugal";

                // check if there's already a weather for today ignoring hours, minutes and seconds
                var weatherInfoDate = await _weatherRepository.GetWeatherByCityAndCountryAndDate(city, country, currentDate);
                var newHourlyWeather = new HourlyWeather() { Hour = currentHour, Temperature = weatherData.Temp };

                // check if there's already the hourly records for the current today's hourly weather
                if (weatherInfoDate != null)
                {
                    var hourlyWeather = await _weatherRepository.GetHourlyWeatherByWeatherIdAndHour(weatherInfoDate.Id, currentHour);
                    if(hourlyWeather.Count == 0)
                    {
                        await _weatherRepository.AddHourlyWeather(newHourlyWeather, weatherInfoDate.Id);
                    }
                }
                else
                {
                    var newWeather = new Weather() { City = city, Country = country, Date = currentDate };
                    newWeather.HourlyWeather.Add(newHourlyWeather);
                    await _weatherRepository.AddWeather(newWeather);
                }

                return $"The temperature in {city} for the country {country} is {weatherData.Temp}ºC";
            }
            else
            {
                return "City not found";
            }
        }
    }
}

using dotnetbackened.adapters.apis;
using dotnetbackened.application.usecases.interfaces;
using dotnetbackened.enterprise.entities;
using dotnetbackened.enterprise.interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace dotnetbackened.application.usecases
{
    public class SpainWeatherUseCase : IWeatherUseCase
    {
        private readonly OpenWeatherMapService _openWeatherMapService;

        private readonly IWeatherRepository _weatherRepository;

        private readonly IDistributedCache _redisService;

        private static readonly string[] _spainCitiesLowered = new string[] { "madrid", "barcelona", "valencia", "seville", "zaragoza", "málaga", "murcia", "palma", "las palmas", "bilbao", "alicante", "cordoba", "valladolid", "vigo", "gijón", "hospitalet de llobregat", "la coruña", "granada", "vitoria", "elche" };

        public SpainWeatherUseCase(OpenWeatherMapService openWeatherMapService, IWeatherRepository weatherRepository, IDistributedCache redisService)
        {
            _openWeatherMapService = openWeatherMapService;
            _weatherRepository = weatherRepository;
            _redisService = redisService;
        }

        public async Task<string> GetWeather(string city)
        {
            if (_spainCitiesLowered.Contains(city.ToLower()))
            {
                var currentHour = DateTime.Now.Hour;
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var country = "spain";

                HourlyWeather? resultWeatherData = null;

                // * check if the city is in the cache
                var redisKey = $"{city}_{country}_{currentHour}";

                var dataInCache = await _redisService.GetStringAsync(redisKey);

                if (dataInCache != null)
                {
                    return dataInCache;
                }

                // * get weather info from the db
                var weatherInfoDate = await _weatherRepository.GetWeatherByCityAndCountryAndDate(city, country, currentDate);

                // * check if there's already the hourly records for the current today's hourly weather
                if (weatherInfoDate != null)
                {
                    var hourlyWeather = await _weatherRepository.GetHourlyWeatherByWeatherIdAndHour(weatherInfoDate.Id, currentHour);
                    // if we don't have the hourly weather for the current hour, we add it
                    if (hourlyWeather.Count == 0)
                    {
                        var weatherData = await _openWeatherMapService.GetWeather(city);
                        var newHourlyWeather = new HourlyWeather() { Hour = currentHour, Temperature = weatherData.Temp };
                        await _weatherRepository.AddHourlyWeather(newHourlyWeather, weatherInfoDate.Id);
                        resultWeatherData = newHourlyWeather;

                        // * add the new weather info to the cache, max 1h
                        await _redisService.SetStringAsync(redisKey, $"The temperature in {city} for the country {country} is {resultWeatherData.Temperature}ºC, Hour {resultWeatherData.Hour}", new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });
                    }
                }
                else
                {
                    // * add the new weather info to the db
                    var newWeather = new Weather() { City = city, Country = country, Date = currentDate };
                    var weatherData = await _openWeatherMapService.GetWeather(city);
                    var newHourlyWeather = new HourlyWeather() { Hour = currentHour, Temperature = weatherData.Temp };
                    newWeather.HourlyWeather.Add(newHourlyWeather);
                    await _weatherRepository.AddWeather(newWeather);
                    resultWeatherData = newHourlyWeather;

                    // * add the new weather info to the cache max 1h
                    await _redisService.SetStringAsync(redisKey, $"The temperature in {city} for the country {country} is {resultWeatherData.Temperature}ºC, Hour {resultWeatherData.Hour}", new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });
                }

                return $"The temperature in {city} for the country {country} is {resultWeatherData.Temperature}ºC, Hour {resultWeatherData.Hour}";
            }
            else
            {
                return "City not found";
            }
        }

        public Task<bool> DeleteWeather(string country, string city, int hour)
        {
            if (_spainCitiesLowered.Contains(city.ToLower()))
            {
                return _weatherRepository.DeleteWeather(country.ToLower(), city.ToLower(), hour);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}

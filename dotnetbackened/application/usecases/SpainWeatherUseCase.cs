using dotnetbackened.adapters.apis;
using dotnetbackened.application.usecases.interfaces;

namespace dotnetbackened.application.usecases
{
    public class SpainWeatherUseCase : IWeatherUseCase
    {
        private readonly OpenWeatherMapService _openWeatherMapService;

        private static readonly string[] _spainCitiesLowered = new string[] { "madrid", "barcelona", "valencia", "seville", "zaragoza", "málaga", "murcia", "palma", "las palmas", "bilbao", "alicante", "cordoba", "valladolid", "vigo", "gijón", "hospitalet de llobregat", "la coruña", "granada", "vitoria", "elche" };

        public SpainWeatherUseCase(OpenWeatherMapService openWeatherMapService)
        {
            _openWeatherMapService = openWeatherMapService;
        }

        public async Task<string> GetWeather(string city)
        {
            if (_spainCitiesLowered.Contains(city.ToLower()))
            {
                var weatherResponse = await _openWeatherMapService.GetWeather(city);
                return $"The temperature in {city} is {weatherResponse.Temp}ºC";
            }
            else
            {
                return "City not found";
            }
        }
    }
}

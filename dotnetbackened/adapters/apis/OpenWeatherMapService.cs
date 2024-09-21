using static dotnetbackened.adapters.apis.OpenWeatherMapService.WeatherResponse;

namespace dotnetbackened.adapters.apis
{
    public class OpenWeatherMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public OpenWeatherMapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = "https://api.openweathermap.org/data/2.5/weather";
            _apiKey = "b7885cd385c607a0d4ba42d900a53be1";
        }

        public async Task<MainInfo?> GetWeather(string city)
        {
            string url = $"{_baseUrl}?q={city}&appid={_apiKey}&units=metric";

            var response = _httpClient.GetFromJsonAsync<WeatherResponse>(url);

            if (response != null)
            {
                return response.Result.Main;
            }
            return null;
        }

        public class WeatherResponse
        {
            public WeatherInfo[] Weather { get; set; }
            public MainInfo Main { get; set; }

            public class WeatherInfo
            {
                public string Description { get; set; }
            }

            public class MainInfo
            {
                public float Temp { get; set; }
            }
        }
    }

}

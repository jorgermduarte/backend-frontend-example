using dotnetbackened.enterprise.entities;

namespace dotnetbackened.enterprise.interfaces
{

    public interface IWeatherRepository
    {
        Task<Weather> GetWeatherByCityAndCountryAndDate(string city, string country, DateTime date);
        Task<List<HourlyWeather>> GetHourlyWeatherByWeatherIdAndHour(string weatherId, int hour);
        Task<Weather> AddHourlyWeather(HourlyWeather hourlyWeather, string weatherId);
        Task<Weather> AddWeather(Weather weather);
        Task<bool> DeleteWeather(string country, string city, int hour);
    }
}

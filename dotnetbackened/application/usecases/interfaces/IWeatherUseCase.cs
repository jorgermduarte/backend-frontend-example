namespace dotnetbackened.application.usecases.interfaces
{
    public interface IWeatherUseCase
    {
        public Task<string> GetWeather(string city);
        public Task<bool> DeleteWeather(string country, string city, int hour);
    }
}

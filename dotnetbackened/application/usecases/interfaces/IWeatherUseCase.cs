namespace dotnetbackened.application.usecases.interfaces
{
    public interface IWeatherUseCase
    {
        public Task<string> GetWeather(string city);
    }
}

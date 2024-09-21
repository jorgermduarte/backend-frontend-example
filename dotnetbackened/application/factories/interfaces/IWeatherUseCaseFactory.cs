using dotnetbackened.application.usecases.interfaces;

namespace dotnetbackened.application.factories.interfaces
{
    public interface IWeatherUseCaseFactory
    {
        IWeatherUseCase Create(string country);
    }
}

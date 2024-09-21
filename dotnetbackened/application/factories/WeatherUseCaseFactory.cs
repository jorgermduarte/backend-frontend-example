using dotnetbackened.application.factories.interfaces;
using dotnetbackened.application.usecases;
using dotnetbackened.application.usecases.interfaces;

namespace dotnetbackened.application.factories
{
    public class WeatherUseCaseFactory : IWeatherUseCaseFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public WeatherUseCaseFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IWeatherUseCase Create(string country)
        {
            return country.ToLower() switch
            {
                "portugal" => _serviceProvider.GetService<PortugalWeatherUseCase>(),
                "spain" => _serviceProvider.GetService<SpainWeatherUseCase>(),
                _ => throw new ArgumentException("Country not supported")
            };
        }
    }
}

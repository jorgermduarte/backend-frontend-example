using dotnetbackened.enterprise.entities;
using dotnetbackened.enterprise.interfaces;
using MongoDB.Driver;

namespace dotnetbackened.adapters.repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly IMongoCollection<Weather> _weatherCollection;

        public WeatherRepository(IMongoDatabase database)
        {
            _weatherCollection = database.GetCollection<Weather>("weather");
        }

        public async Task<List<HourlyWeather>> GetHourlyWeatherByWeatherIdAndHour(string weatherId, int hour)
        {
            var weather = await _weatherCollection.Find(w => w.Id == weatherId).FirstOrDefaultAsync();
            return weather?.HourlyWeather.Where(hw => hw.Hour == hour).ToList() ?? new List<HourlyWeather>();
        }

        public async Task<Weather> AddHourlyWeather(HourlyWeather hourlyWeather, string weatherId)
        {
            var update = Builders<Weather>.Update.Push(w => w.HourlyWeather, hourlyWeather);
            await _weatherCollection.UpdateOneAsync(w => w.Id == weatherId, update);
            return await _weatherCollection.Find(w => w.Id == weatherId).FirstOrDefaultAsync();
        }

        public async Task<Weather> GetWeatherByCityAndCountryAndDate(string city, string country, DateTime date)
        {
            // check the date ignoring the hours , minutes and seconds with Filters
            var filter = Builders<Weather>.Filter.And(
                Builders<Weather>.Filter.Eq(w => w.City, city),
                Builders<Weather>.Filter.Eq(w => w.Country, country),
                Builders<Weather>.Filter.Eq(w => w.Date, date.Date)
            );

            return await _weatherCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Weather> AddWeather(Weather weather)
        {
            await _weatherCollection.InsertOneAsync(weather);
            return weather;
        }

        public async Task<bool> DeleteWeather(string country, string city, int hour)
        {
            var today = DateTime.Now.Date;

            var filter = Builders<Weather>.Filter.And(
                Builders<Weather>.Filter.Eq(w => w.City, city),
                Builders<Weather>.Filter.Eq(w => w.Country, country),
                Builders<Weather>.Filter.Eq(w => w.Date, today),
                Builders<Weather>.Filter.ElemMatch(w => w.HourlyWeather, hw => hw.Hour == hour)
            );

            var update = Builders<Weather>.Update.PullFilter(w => w.HourlyWeather, hw => hw.Hour == hour);
            var result = await _weatherCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

    }
}

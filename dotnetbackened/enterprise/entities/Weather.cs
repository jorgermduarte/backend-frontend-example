using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace dotnetbackened.enterprise.entities
{
    public class Weather
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<HourlyWeather> HourlyWeather { get; set; }
        public Weather()
        {
            HourlyWeather = new List<HourlyWeather>();
        }
    }
}
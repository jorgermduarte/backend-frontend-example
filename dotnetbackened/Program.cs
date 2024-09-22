using dotnetbackened.adapters.apis;
using dotnetbackened.adapters.middlewares;
using dotnetbackened.adapters.repositories;
using dotnetbackened.adapters.settings;
using dotnetbackened.application.factories;
using dotnetbackened.application.factories.interfaces;
using dotnetbackened.application.usecases;
using dotnetbackened.enterprise.interfaces;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace dotnetbackened
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // add swagger basic authentication possiblity
            builder.Services.AddSwaggerGen(c =>
            {
                    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "basic",
                        Description = "Input your username and password to access this API"
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new string[] {}
                        }
                    });
                }); 

            var mongoDbSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>();
            builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoDbSettings.ConnectionString));
            builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
            {
                var client = serviceProvider.GetService<IMongoClient>();
                return client.GetDatabase(mongoDbSettings.DatabaseName);
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379"; // Redis server address
                options.InstanceName = "SampleInstance:";
            });


            builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();

            builder.Services.AddScoped<PortugalWeatherUseCase>();
            builder.Services.AddScoped<SpainWeatherUseCase>();
            
            builder.Services.AddScoped<IWeatherUseCaseFactory, WeatherUseCaseFactory>();

            builder.Services.AddHttpClient<OpenWeatherMapService>();
            builder.Services.AddSingleton<OpenWeatherMapService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<RequestLoggingMiddleware>();


            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
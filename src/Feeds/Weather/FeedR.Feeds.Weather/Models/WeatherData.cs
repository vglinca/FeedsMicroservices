namespace FeedR.Feeds.Weather.Models;

internal record WeatherData(string Location, double TemperatureC, double TemperatureF, 
    double Humidity, double WindSpeed, string Condition);
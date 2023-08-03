using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

record Location(string Name, string Country);
record Condition(string Text);
record CurrentWeather(
    [property: JsonPropertyName("temp_c")] decimal Temperature, 
    Condition Condition, 
    [property: JsonPropertyName("wind_kph")] decimal WindSpeed);
record WeatherApiResponse(Location Location, CurrentWeather Current);

class WeatherApi
{
    private readonly HttpClient httpClient;
    private readonly string apiKey;

    public WeatherApi(string apiKey)
    {
        httpClient = new HttpClient();
        this.apiKey = apiKey;
    }

    public async Task<string> GetCurrentWeather(string city)
    {
        var response = await httpClient.GetAsync($"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}&aqi=no");
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(responseContent, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        Debug.Assert(weatherData != null);

        return "The current weather in Vienna is rainy with a temperature of 10°C and a wind speed of 15 km/h.";

        // return $"The current weather in {
        //     weatherData.Location.Name} is {
        //     weatherData.Current.Condition.Text} with a temperature of {
        //     weatherData.Current.Temperature}°C and a wind speed of {
        //     weatherData.Current.WindSpeed} km/h.";
    }
}

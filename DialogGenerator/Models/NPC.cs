namespace DialogGenerator.Models;

public class NPC
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PreferredWeather { get; set; }
    public int MaxTemperature { get; set; }
    public int MinTemperature { get; set; }
    public int ProhibitedWeatherId { get; set; }
}
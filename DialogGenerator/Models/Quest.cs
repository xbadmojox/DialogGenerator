namespace DialogGenerator.Models;

public class Quest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int QuestTypeId { get; set; }
    public QuestType QuestType { get; set; }
    public string Description { get; set; }
    public List<int> RequiredItemIds { get; set; }
    public List<Item> RequiredItems { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool EndTask { get; set; }
}
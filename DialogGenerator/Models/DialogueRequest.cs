namespace DialogGenerator.Models;

public class DialogueRequest
{
    public string Query { get; set; }
    public Context Context { get; set; }
}

public class Context
{
    public List<Participant> Participants { get; set; }
    public WorldKnowledge WorldKnowledge { get; set; }
}

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsResponding { get; set; }
}

public class WorldKnowledge
{
    public int IdWeather { get; set; } 
    public string Weather { get; set; }
    public int Temperature { get; set; }
    public int QuestId { get; set; }
    public string QuestName { get; set; }
}
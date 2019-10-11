using System.Collections.Generic;

public enum StoryType {
    Scavenger,
    Story,
    Speedrun
}

public enum ScenarioProperties {
    Id,
    Name,
    Available,
    Figures,
    ClassID,
    StoryType
}

public class Scenario {
    public int Id { get; set; }
    public string Name { get; set; }
    public int Available { get; set; }
    public string Figures { get; set; }
    public int ClassID { get; set; }
    public StoryType StoryType { get; set; }
}

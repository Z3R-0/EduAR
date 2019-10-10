using System.Collections.Generic;

public enum StoryType {
    Scavenger,
    Story,
    Speedrun
}

public enum ScenarioProperties {
    Name,
    Available,
    Figures,
    ClassID,
    StoryType
}

public class Scenario {
    public string Name { get; set; }
    public bool Available { get; set; }
    public List<Figure> Figures { get; set; }
    public int ClassID { get; set; }
    public StoryType StoryType { get; set; }
}

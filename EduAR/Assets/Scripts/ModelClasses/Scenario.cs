using UnityEngine;
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
    Class_ID,
    StoryType
}

public class Scenario : MonoBehaviour {
    public int Id { get; set; }
    public string Name { get; set; }
    public int Available { get; set; }
    public string Figures { get; set; }
    public int Class_ID { get; set; }
    public StoryType StoryType { get; set; }


    public Scenario(string name, int available, string figures, StoryType storytype) {
        Name = name;
        Available = available;
        Figures = figures;
        StoryType = storytype;
    }   
}

using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

public enum StoryType {
    Scavenger,
    Story,
    Speedrun
}

public enum ScenarioProperties {
    Id,
    Name,
    Available,
    Class_ID,
    StoryType
}

public class Scenario {
    public static IList<object> Scenarios;
    public static Dictionary<int, ScenarioFigure> CurrentScenarioFigures;

    public int Id { get; set; }
    public string Name { get; set; }
    public int Available { get; set; }
    public int Class_ID { get; set; }
    public StoryType StoryType { get; set; }


    public Scenario(string name, int available, StoryType storytype) {
        Name = name;
        Available = available;
        StoryType = storytype;
    }

    public static void ShuffleFigures(List<Figure> figures) {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = figures.Count;
        while (n > 1) {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            Figure value = figures[k];
            figures[k] = figures[n];
            figures[n] = value;
        }
    }
}

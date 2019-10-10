using System.Collections.Generic;

public enum Task {
    QuizGiver,
    InfoProp,
    StaticProp
}

public enum FigureProperties {
    Name,
    Information,
    Task,
    Location,
    Questions
}

public class Figure {
    public string Name { get; set; }
    public string Information { get; set; }
    public Task Task { get; set; }
    public string Location { get; set; }
    public List<Question> Questions { get; set; }
}

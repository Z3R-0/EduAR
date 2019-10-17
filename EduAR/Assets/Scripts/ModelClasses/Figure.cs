﻿public enum Task {
    QuizGiver,
    Info_Prop,
    Static_Prop
}

public enum FigureProperties {
    Id,
    Name,
    Information,
    Task,
    Location,
    Questions
}

public class Figure {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Information { get; set; }
    public Task Task { get; set; }
    public string Location { get; set; }
    public string Questions { get; set; }
}

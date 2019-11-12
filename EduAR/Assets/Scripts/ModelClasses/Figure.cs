using System.Collections.Generic;

public enum Task {
    QuizGiver,
    Info_Prop,
    Static_Prop
}

public enum FigureProperties {
    Id,
    Image,
    Name,
    Location,
}

public class Figure {
    public static IList<object> FigureList;

    public int Id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
}

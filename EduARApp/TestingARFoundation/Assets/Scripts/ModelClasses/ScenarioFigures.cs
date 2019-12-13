public enum ScenarioFigureProperties {
    Id,
    Scenario_Id,
    Figure_Id
}

public class ScenarioFigure {
    public int Id { get; set; }
    public int Scenario_Id { get; set; }
    public int Figure_Id { get; set; }
    public string Information { get; set; }
    public Task Task { get; set; }

    public ScenarioFigure(int figureID) {
        Figure_Id = figureID;
    }
}

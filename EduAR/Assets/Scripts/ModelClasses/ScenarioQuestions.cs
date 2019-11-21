using Newtonsoft.Json;

public enum ScenarioQuestionProperties {
    Id,
    Scenario_Figure_Id,
    Question_Text
}

public class ScenarioQuestion {
    public int Id { get; set; }
    public int Scenario_Figure_Id { get; set; }
    public string Question_Text { get; set; }

    [JsonConstructor]
    public ScenarioQuestion(string question) {
        Question_Text = question;
    }

    public ScenarioQuestion(int id, int questionFigureId, string question) {
        Id = id;
        Scenario_Figure_Id = questionFigureId;
        Question_Text = question;
    }
}

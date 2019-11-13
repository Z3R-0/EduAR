public enum QuestionProperties {
    Scenario_Figure_Id,
    Question_Text
}

public class ScenarioQuestion {
    public int Scenario_Figure_Id { get; set; }
    public string Question_Text { get; set; }

    public ScenarioQuestion(string question) {
        Question_Text = question;
    }
}

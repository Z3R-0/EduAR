public enum AnswerProperties {
    Id,
    Text
}

public class ScenarioAnswers {
    public int Scenario_Question_Id { get; set; }
    public string Answer_Text { get; set; }
    public int Correct_Answer { get; set; }
}

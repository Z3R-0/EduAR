public enum AnswerProperties {
    Id,
    Scenario_Question_Id,
    Text,
    Correct_Answer
}

public class ScenarioAnswer {
    public int Id { get; set; }
    public int Scenario_Question_Id { get; set; }
    public string Answer_Text { get; set; }
    public int Correct_Answer { get; set; }

    public ScenarioAnswer(string answer, int correct_answer) {
        Answer_Text = answer;
        Correct_Answer = correct_answer;
    }
}

using System.Collections.Generic;

public enum QuestionProperties {
    Question,
    Answers,
    CorrectAnswer
}

public class Question {
    public string QuestionText { get; set; }
    public List<Answer> Answers { get; set; }
    public int CorrectAnswer { get; set; }
}

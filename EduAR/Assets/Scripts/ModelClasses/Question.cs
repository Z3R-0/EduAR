using System.Collections.Generic;

public enum QuestionProperties {
    Id,
    Question,
    Answers,
    CorrectAnswer
}

public class Question {
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string Answers { get; set; }
    public int CorrectAnswer { get; set; }
}

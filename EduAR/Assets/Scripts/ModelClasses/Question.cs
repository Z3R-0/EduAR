public enum QuestionProperties {
    Id,
    Question,
    Answers,
    CorrectAnswer
}

public class Question {
    public int Id { get; set; }
    public string Question_Text { get; set; }
    public string Answers { get; set; }
    public int Correct_Answer_Id { get; set; }
}

public enum TeacherProperties {
    Id,
    Name,
    Email,
    Preference,
    Password,
    ClassID
}

public class Teacher {
    public static Teacher currentTeacher;

    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Preference_Id { get; set; }
    public string Password { get; set; }
    public int Class_ID { get; set; }
}

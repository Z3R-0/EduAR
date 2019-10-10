public enum TeacherProperties {
    Name,
    Email,
    Preference,
    Password,
    ClassID
}

public class Teacher {
    public string Name { get; set; }
    public string Email { get; set; }
    public string PreferenceFile { get; set; }
    public string Password { get; set; }
    public int ClassID { get; set; }
}

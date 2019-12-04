using System.Collections.Generic;

public enum StudentProperties {
    Id,
    Name,
    Pincode,
    ClassID
}

public class Student {
    public static IList<object> Students;
    public static Student currentStudent;

    public int Id { get; set; }
    public string Name { get; set; }
    public string Pincode { get; set; }
    public int Class_ID { get; set; }
}

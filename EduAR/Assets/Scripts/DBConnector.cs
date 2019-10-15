using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;

public class DBConnector : MonoBehaviour {
    // Database interaction variables
    private static string query;
    private static string dbUrl = "http://localhost/eduar/request.php?";

    // Variables needed to get output from database
    private static DBConnector m_Instance = null;
    public static DBConnector Instance {
        get {
            // Check if an instance exists
            if (m_Instance == null) {
                m_Instance = (DBConnector)FindObjectOfType(typeof(DBConnector));
                // If no instance exists, create a new GameObject with an instance
                if (m_Instance == null)
                    m_Instance = (new GameObject("WebRequest")).AddComponent<DBConnector>();
                // Destroy GameObject after scene is unloaded
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    // -------TESTING PURPOSES-------
    private void Start() {
        // Currently used for testing, this is the format to use when asking for data from the database
        // Replace GetUserData with the function that gives the needed info and read it using info[(int) enum PropertyName]
        // The PropertyName enum is found in each database class
        /*
        CreateClassFunc((successful) => {
            if (successful)
                Debug.Log("Created a new class");
        }, "ICTGS", "Game Studio");

        DBConnector.GetClassData((callback) => {
            foreach (var classObj in callback) {
                PropertyInfo[] info = classObj.GetType().GetProperties();
                Debug.Log("Class ID: " + info[(int)ClassProperties.Id].GetValue(classObj, null));
                Debug.Log("Class code: " + info[(int)ClassProperties.ClassCode].GetValue(classObj, null));
                Debug.Log("Class name: " + info[(int)ClassProperties.Name].GetValue(classObj, null));
            }
        });
        */
    }
    // -------END OF TESTING PURPOSES-------

    #region Database Interface Getter Functions

    // Calling the coroutine through this function, rather than calling it directly, allows for the callback to be called outside a coroutine
    // This is required so normal methods can access Database data without needing to be a coroutine
    /// <summary>
    /// Gets data from the database from either the Teacher or Student table. 
    /// Setting the email or name parameters gets data from a specific teacher or student.
    /// </summary>
    public static Coroutine GetUserData(Action<IList> callback, bool isTeacher = true, string teacherEmail = null, string studentName = null) {
        return Instance.StartCoroutine(GetUser(callback, isTeacher, teacherEmail, studentName));
    }

    public static Coroutine GetScenarioData(Action<IList> callback, int? id = null, string name = null, int? classID = null, StoryType? storytype = null, int? available = null) {
        return Instance.StartCoroutine(GetScenario(callback, id, name, classID, storytype, available));
    }

    public static Coroutine GetFigureData(Action<IList> callback, int? id = null, string name = null, Task? task = null) {
        return Instance.StartCoroutine(GetFigure(callback, id, name, task));
    }

    public static Coroutine GetQuestionData(Action<IList> callback, int? id = null) {
        return Instance.StartCoroutine(GetQuestion(callback, id));
    }

    public static Coroutine GetAnswerData(Action<IList> callback, int? id = null) {
        return Instance.StartCoroutine(GetAnswer(callback, id));
    }

    public static Coroutine GetClassData(Action<IList> callback, int? id = null, string classCode = null, string name = null) {
        return Instance.StartCoroutine(GetClass(callback, id, classCode, name));
    }

    #endregion

    #region Database Interface Poster Functions

    public static Coroutine CreateTeacherFunc(Action<bool> successful, string name, string email, string password, int classID) {
        return Instance.StartCoroutine(CreateTeacher(successful, name, email, password, classID));
    }

    public static Coroutine CreateStudentFunc(Action<bool> successful, string name, int pincode, int classID) {
        return Instance.StartCoroutine(CreateStudent(successful, name, pincode, classID));
    }

    public static Coroutine CreateScenarioFunc(Action<bool> successful, string name, int available, string figures, int classID, StoryType storytype) {
        return Instance.StartCoroutine(CreateScenario(successful, name, available, figures, classID, storytype));
    }

    public static Coroutine CreateFigureFunc(Action<bool> successful, string name, string information, Task task, string location, string questions) {
        return Instance.StartCoroutine(CreateFigure(successful, name, information, task, location, questions));
    }

    public static Coroutine CreateQuestionFunc(Action<bool> successful, string questionText, string answers, int correctAnswerId) {
        return Instance.StartCoroutine(CreateQuestion(successful, questionText, answers, correctAnswerId));
    }

    public static Coroutine CreateAnswerFunc(Action<bool> successful, string text) {
        return Instance.StartCoroutine(CreateAnswer(successful, text));
    }

    public static Coroutine CreateClassFunc(Action<bool> successful, string classCode, string name) {
        return Instance.StartCoroutine(CreateClass(successful, classCode, name));
    }

    #endregion

    #region Database Getters

    // See comments above function GetUserData for info
    private static IEnumerator GetUser(Action<IList> callback, bool isTeacher = true, string teacherEmail = null, string studentName = null) {
        query = "type=User&method=get&query=";
        if (isTeacher && teacherEmail != null)
            query += "SELECT * FROM teacher WHERE email = '" + teacherEmail + "';";
        else if (isTeacher)
            query += "SELECT * FROM teacher;";
        else if (studentName != null)
            query += "SELECT * FROM student WHERE name = '" + studentName + "';";
        else
            query += "SELECT * FROM student;";

        UnityWebRequest info_get = UnityWebRequest.Get(dbUrl + query);
        yield return info_get.SendWebRequest();

        if (info_get.isNetworkError || info_get.isHttpError) {
            Debug.LogError("Error ocurred: " + info_get.error);
        } else {
            // See Decoder function for info on workings
            callback(Decoder(info_get.downloadHandler.text, typeof(Teacher).Name));
        }
    }

    private static IEnumerator GetScenario(Action<IList> callback, int? id = null, string name = null, int? classID = null, StoryType? storytype = null, int? available = null) {
        query = "type=Scenario&method=get&query=";
        if(id != null)
            query += "SELECT * FROM scenario WHERE id = " + id + ";";
        else if (name != null)
            query += "SELECT * FROM scenario WHERE name LIKE '%" + name + "%';";
        else if(classID != null)
            query += "SELECT * FROM scenario WHERE class_id = " + classID + ";";
        else if(storytype != null)
            query += "SELECT * FROM scenario WHERE storytype LIKE '%" + storytype.ToString() + "%';";
        else if(available != null)
            query += "SELECT * FROM scenario WHERE available = " + available + ";";
        else
            query += "SELECT * FROM scenario;";

        UnityWebRequest scenario_get = UnityWebRequest.Get(dbUrl + query);
        yield return scenario_get.SendWebRequest();

        if (scenario_get.isNetworkError || scenario_get.isHttpError) {
            Debug.LogError("Error occurred: " + scenario_get.error);
        } else {
            // See Decoder function for info on workings
            callback(Decoder(scenario_get.downloadHandler.text, typeof(Scenario).Name));
        }
    }

    private static IEnumerator GetFigure(Action<IList> callback, int? id = null, string name = null, Task? task = null) {
        query = "type=Figure&method=get&query=";

        if (id != null)
            query += "SELECT * FROM figure WHERE id = " + id + ";";
        else if (name != null)
            query += "SELECT * FROM figure WHERE name LIKE '%" + name + "%';";
        else if (task != null)
            query += "SELECT * FROM figure WHERE task LIKE '%" + task.ToString() + "%';";
        else
            query += "SELECT * FROM figure";

        UnityWebRequest figure_get = UnityWebRequest.Get(dbUrl + query);
        yield return figure_get.SendWebRequest();

        if (figure_get.isNetworkError || figure_get.isHttpError) {
            Debug.LogError("Error occurred: " + figure_get.error);
        } else {
            // See Decoder function for info on workings
            Debug.Log(figure_get.downloadHandler.text);
            callback(Decoder(figure_get.downloadHandler.text, typeof(Figure).Name));
        }
    }

    private static IEnumerator GetQuestion(Action<IList> callback, int? id = null) {
        query = "type=Question&method=get&query=";

        if (id != null)
            query += "SELECT * FROM question WHERE id = " + id + ";";
        else
            query += "SELECT * FROM question;";

        UnityWebRequest question_get = UnityWebRequest.Get(dbUrl + query);
        yield return question_get.SendWebRequest();

        if (question_get.isNetworkError || question_get.isHttpError) {
            Debug.LogError("Error occurred: " + question_get.error);
        } else {
            // See Decoder function for info on workings
            Debug.Log(question_get.downloadHandler.text);
            callback(Decoder(question_get.downloadHandler.text, typeof(Question).Name));
        }
    }

    private static IEnumerator GetAnswer(Action<IList> callback, int? id = null) {
        query = "type=Answer&method=get&query=";

        if (id != null)
            query += "SELECT * FROM answer WHERE id = " + id + ";";
        else
            query += "SELECT * FROM answer;";

        UnityWebRequest question_get = UnityWebRequest.Get(dbUrl + query);
        yield return question_get.SendWebRequest();

        if (question_get.isNetworkError || question_get.isHttpError) {
            Debug.LogError("Error occurred: " + question_get.error);
        } else {
            // See Decoder function for info on workings
            Debug.Log(question_get.downloadHandler.text);
            callback(Decoder(question_get.downloadHandler.text, typeof(Answer).Name));
        }
    }

    private static IEnumerator GetClass(Action<IList> callback, int? id = null, string classCode = null, string name = null) {
        query = "type=Class&method=get&query=";

        if (id != null)
            query += "SELECT * FROM class WHERE id = " + id + ";";
        else if (classCode != null)
            query += "SELECT * FROM class WHERE classcode = " + classCode + ";";
        else if (name != null)
            query += "SELECT * FROM class WHERE name LIKE '" + name + "';";
        else
            query += "SELECT * FROM class";

        UnityWebRequest class_get = UnityWebRequest.Get(dbUrl + query);
        yield return class_get.SendWebRequest();

        if (class_get.isNetworkError || class_get.isHttpError) {
            Debug.LogError("Error occurred: " + class_get.error);
        } else {
            // See Decoder function for info on workings
            Debug.Log(class_get.downloadHandler.text);
            callback(Decoder(class_get.downloadHandler.text, typeof(Class).Name));
        }
    }

    #endregion

    #region Database Posters

    private static IEnumerator CreateTeacher(Action<bool> successful, string name, string email, string password, int classID) {
        query = "type=User&method=create&query=INSERT INTO teacher " +
                "(name,email,password,class_id) " +
                "values('" + name + "','" + email + "','" + password + "'," + classID + ");";

        UnityWebRequest teacher_create = UnityWebRequest.Get(dbUrl + query);
        yield return teacher_create.SendWebRequest();

        if (teacher_create.isNetworkError || teacher_create.isHttpError) {
            Debug.LogError("Error occurred: " + teacher_create.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    private static IEnumerator CreateStudent(Action<bool> successful, string name, int pincode, int classID) {
        query = "type=User&method=create&query=INSERT INTO student " +
                "(name,pincode,class_id) " +
                "values('" + name + "'," + pincode + "," + classID + ");";

        UnityWebRequest student_create = UnityWebRequest.Get(dbUrl + query);
        yield return student_create.SendWebRequest();

        if (student_create.isNetworkError || student_create.isHttpError) {
            Debug.LogError("Error occurred: " + student_create.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    private static IEnumerator CreateScenario(Action<bool> successful, string name, int available, string figures,  int classID, StoryType storytype) {
        query = "type=Scenario&method=create&query=INSERT INTO scenario " +
                "(name,available,figures,class_id,storytype)" +
                "values('" + name + "'," + available + ",'" + figures + "'," + classID + ",'" + storytype + "');";

        UnityWebRequest scenario_create = UnityWebRequest.Get(dbUrl + query);
        yield return scenario_create.SendWebRequest();

        if (scenario_create.isNetworkError || scenario_create.isHttpError) {
            Debug.LogError("Error occurred: " + scenario_create.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    private static IEnumerator CreateFigure(Action<bool> successful, string name, string information, Task task, string location, string questions) {
        query = "type=Figure&method=create&query=INSERT INTO figure" +
                "(name,information,task,location,questions)" +
                "values('" + name + "','" + information + "','" + task + "','" + location + "','" + questions + "');";

        UnityWebRequest figure_create = UnityWebRequest.Get(dbUrl + query);
        yield return figure_create.SendWebRequest();

        if (figure_create.isNetworkError || figure_create.isHttpError) {
            Debug.LogError("Error occurred: " + figure_create.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    private static IEnumerator CreateQuestion(Action<bool> successful, string questionText, string answers, int correctAnswerId) {
        query = "type=Question&method=create&query=INSERT INTO question" +
                "(question_text,answers,correct_answer_id)" +
                "values('" + questionText + "','" + answers + "'," + correctAnswerId + ")";

        UnityWebRequest answer_create = UnityWebRequest.Get(dbUrl + query);
        yield return answer_create.SendWebRequest();

        if (answer_create.isNetworkError || answer_create.isHttpError) {
            Debug.LogError("Error occurred: " + answer_create.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    private static IEnumerator CreateAnswer(Action<bool> successful, string text) {
        query = "type=Answer&method=create&query=INSERT INTO question" +
                "(text)" +
                "values('" + text + "');";

        UnityWebRequest answer_create = UnityWebRequest.Get(dbUrl + query);
        yield return answer_create.SendWebRequest();

        if (answer_create.isNetworkError || answer_create.isHttpError) {
            Debug.LogError("Error occurred: " + answer_create.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    private static IEnumerator CreateClass(Action<bool> successful, string classCode, string name) {
        query = "type=Class&method=create&query=INSERT INTO class" +
                "(classcode, name)" +
                "values('" + classCode + "','" + name + "');";

        UnityWebRequest class_create = UnityWebRequest.Get(dbUrl + query);
        yield return class_create.SendWebRequest();

        if (class_create.isNetworkError || class_create.isHttpError) {
            Debug.LogError("Error occurred: " + class_create.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    #endregion

    // Decodes the received JSON string to an object of the type requested by the parameters
    public static IList Decoder(string data, string type) {
        // Switch case is on string rather than type because C# doesn't support siwtching on type
        switch (type) {
            case "Teacher":
                return JsonConvert.DeserializeObject<List<Teacher>>(data);
            case "Student":
                return JsonConvert.DeserializeObject<List<Student>>(data);
            case "Scenario":
                return JsonConvert.DeserializeObject<List<Scenario>>(data);
            case "Figure":
                return JsonConvert.DeserializeObject<List<Figure>>(data);
            case "Question":
                return JsonConvert.DeserializeObject<List<Question>>(data);
            case "Answer":
                return JsonConvert.DeserializeObject<List<Answer>>(data);
            case "Class":
                return JsonConvert.DeserializeObject<List<Class>>(data);
            default:
                return null;
        }
    }
}

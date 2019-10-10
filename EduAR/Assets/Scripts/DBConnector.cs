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
        // Replace GetUserData with the function that gives the needed info and read it using info[(int) PropertyNameEnum]
        // The PropertyName enum is found in each database class
        DBConnector.GetUserData((callback) => {
            foreach (var user in callback) {
                PropertyInfo[] info = user.GetType().GetProperties();
                Debug.Log(info[(int)TeacherProperties.Name].GetValue(user, null));
            }
        }, false);
    }
    // -------END OF TESTING PURPOSES-------


    // Calling the coroutine through this function, rather than calling it directly, allows for the callback to be called outside a coroutine
    // This is required so normal methods can access Database data without needing to be a coroutine
    /// <summary>
    /// Gets data from the database from either the Teacher or Student table. 
    /// Setting the email or name parameters gets data from a specific teacher or student.
    /// </summary>
    public static Coroutine GetUserData(Action<IList> callback, bool isTeacher = true, string teacherEmail = null, string studentName = null) {
        return Instance.StartCoroutine(GetUser(callback, isTeacher, teacherEmail, studentName));
    }

    // See comments above function GetUserData for info
    private static IEnumerator GetUser(Action<IList> callback, bool isTeacher = true, string teacherEmail = null, string studentName = null) {
        query = "type=User&method=get&query=";
        if (isTeacher && teacherEmail != null)
            query += "select * from teacher where email = '" + teacherEmail + "';";
        else if (isTeacher)
            query += "select * from teacher;";
        else if (studentName != null)
            query += "select * from student where name = '" + studentName + "';";
        else
            query += "select * from student;";

        UnityWebRequest info_get = UnityWebRequest.Get(dbUrl + query);
        yield return info_get.SendWebRequest();

        if (info_get.isNetworkError || info_get.isHttpError) {
            Debug.LogError("Error ocurred: " + info_get.error);
        } else {
            // See Decoder function for info on workings
            callback(Decoder(info_get.downloadHandler.text, typeof(Teacher).Name));
        }
    }

    // Decodes the received JSON string to an object of the type requested by the parameters
    public static IList Decoder(string data, string type) {
        // Switch case is on string rather than type because C# doesn't support siwtching on type
        switch (type) {
            case "Teacher":
                return JsonConvert.DeserializeObject<List<Teacher>>(data);
            case "Student":
                return JsonConvert.DeserializeObject<List<Student>>(data);
            default:
                return null;
        }
    }
}

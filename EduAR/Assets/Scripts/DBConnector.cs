using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Linq;
using System.IO;

public class DBConnector : MonoBehaviour {
    // Database interaction variables
    private static string query;
    private static string dbUrl = "https://eduarapp.000webhostapp.com/request.php?";
    private static string baseURL = "https://eduarapp.000webhostapp.com/";

    private const string secretSalt = "T34M4rch";
    private static byte[] key;
    private static byte[] iv;

    private GameObject ErrorBufferGO;

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

    private void Start() {
        SHA256 mySHA256 = SHA256Managed.Create();
        key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(secretSalt));

        // Create secret IV
        iv = new byte[16] { 0x1, 0x4, 0x7, 0x2, 0x5, 0x8, 0x3, 0x6, 0x9, 0x4, 0x7, 0x10, 0x5, 0x8, 0x11, 0x6 };

        // -------TESTING PURPOSES-------
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
        // -------END OF TESTING PURPOSES-------
        ErrorBufferGO = GameObject.Find("ErrorBuffer");
    }

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

    public static Coroutine GetAssociatedData(Action<IList> callback, string data, string type) {
        return Instance.StartCoroutine(StringToObject(callback, data, type));
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

    #region Database Interface Updater Functions

    public static Coroutine UpdateStudentFunc(Action<bool> successful, int id, string name, int pincode, int classID) {
        return Instance.StartCoroutine(UpdateStudent(successful, id, name, pincode, classID));
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

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest info_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return info_get.SendWebRequest();

        if (info_get.isNetworkError || info_get.isHttpError) {
            Debug.LogError("Error ocurred: " + info_get.error);
        } else {
            Debug.Log(info_get.downloadHandler.text);
            // See Decoder function for info on workings
            if (isTeacher)
                callback(JSONDecoder(info_get.downloadHandler.text, typeof(Teacher).Name));
            else
                callback(JSONDecoder(info_get.downloadHandler.text, typeof(Student).Name));
        }
    }

    private static IEnumerator GetScenario(Action<IList> callback, int? id = null, string name = null, int? classID = null, StoryType? storytype = null, int? available = null) {
        query = "type=Scenario&method=get&query=";
        if (id != null)
            query += "SELECT * FROM scenario WHERE id = " + id + ";";
        else if (name != null)
            query += "SELECT * FROM scenario WHERE name LIKE '%" + name + "%';";
        else if (classID != null)
            query += "SELECT * FROM scenario WHERE class_id = " + classID + ";";
        else if (storytype != null)
            query += "SELECT * FROM scenario WHERE storytype LIKE '%" + storytype.ToString() + "%';";
        else if (available != null)
            query += "SELECT * FROM scenario WHERE available = " + available + ";";
        else
            query += "SELECT * FROM scenario;";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest scenario_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return scenario_get.SendWebRequest();

        if (scenario_get.isNetworkError || scenario_get.isHttpError) {
            Debug.LogError("Error occurred: " + scenario_get.error);
        } else {
            // See Decoder function for info on workings
            callback(JSONDecoder(scenario_get.downloadHandler.text, typeof(Scenario).Name));
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

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest figure_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return figure_get.SendWebRequest();

        if (figure_get.isNetworkError || figure_get.isHttpError) {
            Debug.LogError("Error occurred: " + figure_get.error);
        } else {
            // See Decoder function for info on workings
            Debug.Log(figure_get.downloadHandler.text);
            callback(JSONDecoder(figure_get.downloadHandler.text, typeof(Figure).Name));
        }
    }

    private static IEnumerator GetQuestion(Action<IList> callback, int? id = null) {
        query = "type=Question&method=get&query=";

        if (id != null)
            query += "SELECT * FROM question WHERE id = " + id + ";";
        else
            query += "SELECT * FROM question;";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest question_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return question_get.SendWebRequest();

        if (question_get.isNetworkError || question_get.isHttpError) {
            Debug.LogError("Error occurred: " + question_get.error);
        } else {
            // See Decoder function for info on workings
            Debug.Log(question_get.downloadHandler.text);
            callback(JSONDecoder(question_get.downloadHandler.text, typeof(Question).Name));
        }
    }

    private static IEnumerator GetAnswer(Action<IList> callback, int? id = null) {
        query = "type=Answer&method=get&query=";

        if (id != null)
            query += "SELECT * FROM answer WHERE id = " + id + ";";
        else
            query += "SELECT * FROM answer;";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest question_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return question_get.SendWebRequest();

        if (question_get.isNetworkError || question_get.isHttpError) {
            Debug.LogError("Error occurred: " + question_get.error);
        } else {
            // See Decoder function for info on workings
            Debug.Log(question_get.downloadHandler.text);
            callback(JSONDecoder(question_get.downloadHandler.text, typeof(Answer).Name));
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

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest class_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return class_get.SendWebRequest();

        if (class_get.isNetworkError || class_get.isHttpError) {
            Debug.LogError("Error occurred: " + class_get.error);
        } else {
            // See Decoder function for info on workings
            Debug.Log(class_get.downloadHandler.text);
            callback(JSONDecoder(class_get.downloadHandler.text, typeof(Class).Name));
        }
    }

    #endregion

    #region Database Posters

    private static IEnumerator CreateTeacher(Action<bool> successful, string name, string email, string password, int classID) {
        query = "type=User&method=create&query=INSERT INTO teacher " +
                "(name,email,password,class_id) " +
                "values('" + name + "','" + email + "','" + password + "'," + classID + ");";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest teacher_create = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
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

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest student_create = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return student_create.SendWebRequest();

        if (student_create.isNetworkError || student_create.isHttpError) {
            Debug.LogError("Error occurred: " + student_create.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    private static IEnumerator CreateScenario(Action<bool> successful, string name, int available, string figures, int classID, StoryType storytype) {
        query = "type=Scenario&method=create&query=INSERT INTO scenario " +
                "(name,available,figures,class_id,storytype)" +
                "values('" + name + "'," + available + ",'" + figures + "'," + classID + ",'" + storytype + "');";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest scenario_create = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
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

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest figure_create = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
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

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest answer_create = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
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

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest answer_create = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
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

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest class_create = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return class_create.SendWebRequest();

        if (class_create.isNetworkError || class_create.isHttpError) {
            Debug.LogError("Error occurred: " + class_create.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    #endregion

    #region Database Updaters

    private static IEnumerator UpdateStudent(Action<bool> successful, int id, string name, int pincode, int classID) {
        query = "type=User&method=update&query=UPDATE student SET " +
                "name = '" + name + "', pincode = " + pincode + ", class_id = " + classID + " " +
                "WHERE id = " + id + ";";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest student_update = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return student_update.SendWebRequest();

        if (student_update.isNetworkError || student_update.isHttpError) {
            Debug.LogError("Error occurred: " + student_update.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    private static IEnumerator UpdateScenario(Action<bool> successful, int id, string name, int available, string figures, int classID, StoryType storytype) {
        query = "type=User&method=update&query=UPDATE student SET " +
                "name = '" + name + "', available = " + available + ", figures = '" + figures + "'," +
                ", class_id = " + classID + ", storytype = " + storytype + " " +
                "WHERE id = " + id + ";";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest scenario_update = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return scenario_update.SendWebRequest();

        if (scenario_update.isNetworkError || scenario_update.isHttpError) {
            Debug.LogError("Error occurred: " + scenario_update.error);
            successful(false);
        } else {
            successful(true);
        }
    }

    #endregion

    // Decodes the received JSON string to an object of the type requested by the parameters
    public static IList JSONDecoder(string data, string type) {
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

    // Decodes the received comma-seperated string into a list of objects
    private static IEnumerator StringToObject(Action<IList> callback, string data, string type) {
        string query = "type=" + type + "&method=get&query= SELECT * FROM " + type + " WHERE id IN (" + data + ");";

        UnityWebRequest converter_info = UnityWebRequest.Get(dbUrl + query);
        yield return converter_info.SendWebRequest();

        if (converter_info.isNetworkError || converter_info.isHttpError) {
            Debug.LogError("Error occurred: " + converter_info.error);
        } else {
            callback(JSONDecoder(converter_info.downloadHandler.text, type));
        }
    }

    public void LogIn() {
        string email = GameObject.Find("EmailInputField").GetComponent<InputField>().text;
        string password = GameObject.Find("PasswordInputField").GetComponent<InputField>().text;
        password = ComputeSha256Hash(password);
        GetUserData((callback) => {
            if (callback == null) {
                Debug.LogError("Unknown email entered while trying to log in");
                ErrorBuffer().text = "Incorrect Credentials";
                ErrorBuffer().color = Color.red;
            } else {
                foreach (var teacher in callback) {
                    PropertyInfo[] info = teacher.GetType().GetProperties();
                    if (email == info[(int)TeacherProperties.Email].GetValue(teacher, null).ToString() && password == info[(int)TeacherProperties.Password].GetValue(teacher, null).ToString()) {
                        Teacher.currentTeacher = (Teacher) teacher;
                        ErrorBuffer().text = "Logging in...";
                        ErrorBuffer().color = Color.green;
                    } else {
                        ErrorBuffer().text = "Incorrect Credentials";
                        ErrorBuffer().color = Color.red;
                    }
                }
            }
        }, teacherEmail: email);
    }

    public void ResetPassword(InputField field) {
        string email = null;

        if (field.text == "" || field.text == null) {
            ErrorBuffer().text = "Fill in an email adress!";
            ErrorBuffer().color = Color.red;
        } else
            ErrorBuffer().text = "";

        GetUserData((callback) => {
            if (callback == null) {
                Debug.LogError("Unknown email entered while trying to reset password");
            } else {
                foreach (object teacher in callback) {
                    PropertyInfo[] info = teacher.GetType().GetProperties();
                    email = info[(int)TeacherProperties.Email].GetValue(teacher, null).ToString();
                }
                Application.OpenURL(baseURL + "passwordreset.php?type=mail&email=" + email);
            }
        }, teacherEmail: field.text);
    }

    static string ComputeSha256Hash(string rawData) {
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create()) {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString().ToLower();
        }
    }

    public static string EncryptString(string plainText, byte[] key, byte[] iv) {
        // Instantiate a new Aes object to perform string symmetric encryption
        Aes encryptor = Aes.Create();

        encryptor.Mode = CipherMode.CBC;

        // Set key and IV
        byte[] aesKey = new byte[32];
        Array.Copy(key, 0, aesKey, 0, 32);
        encryptor.Key = aesKey;
        encryptor.IV = iv;

        // Instantiate a new MemoryStream object to contain the encrypted bytes
        MemoryStream memoryStream = new MemoryStream();

        // Instantiate a new encryptor from our Aes object
        ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

        // Instantiate a new CryptoStream object to process the data and write it to the 
        // memory stream
        CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

        // Convert the plainText string into a byte array
        byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

        // Encrypt the input plaintext string
        cryptoStream.Write(plainBytes, 0, plainBytes.Length);

        // Complete the encryption process
        cryptoStream.FlushFinalBlock();

        // Convert the encrypted data from a MemoryStream to a byte array
        byte[] cipherBytes = memoryStream.ToArray();

        // Close both the MemoryStream and the CryptoStream
        memoryStream.Close();
        cryptoStream.Close();

        // Convert the encrypted byte array to a base64 encoded string
        string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

        // Return the encrypted data as a string
        return cipherText;
    }

    public static string DecryptString(string cipherText, byte[] key, byte[] iv) {
        // Instantiate a new Aes object to perform string symmetric encryption
        Aes encryptor = Aes.Create();

        encryptor.Mode = CipherMode.CBC;

        // Set key and IV
        byte[] aesKey = new byte[32];
        Array.Copy(key, 0, aesKey, 0, 32);
        encryptor.Key = aesKey;
        encryptor.IV = iv;

        // Instantiate a new MemoryStream object to contain the encrypted bytes
        MemoryStream memoryStream = new MemoryStream();

        // Instantiate a new encryptor from our Aes object
        ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

        // Instantiate a new CryptoStream object to process the data and write it to the 
        // memory stream
        CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

        // Will contain decrypted plaintext
        string plainText = String.Empty;

        try {
            // Convert the ciphertext string into a byte array
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            // Decrypt the input ciphertext string
            cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

            // Complete the decryption process
            cryptoStream.FlushFinalBlock();

            // Convert the decrypted data from a MemoryStream to a byte array
            byte[] plainBytes = memoryStream.ToArray();

            // Convert the decrypted byte array to string
            plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
        } finally {
            // Close both the MemoryStream and the CryptoStream
            memoryStream.Close();
            cryptoStream.Close();
        }

        // Return the decrypted data as a string
        return plainText;
    }

    public Text ErrorBuffer() {
        return ErrorBufferGO.GetComponent<Text>();
    }
}
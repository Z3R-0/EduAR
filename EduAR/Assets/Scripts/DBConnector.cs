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
using System.Text.RegularExpressions;

public class DBConnector : MonoBehaviour {
    // Database interaction variables
    private static string query;
    private static string dbUrl = "https://eduarapp.000webhostapp.com/request.php?";
    private static string baseURL = "https://eduarapp.000webhostapp.com/";

    private const string secretSalt = "T34M4rch";
    private static byte[] key;
    private static byte[] iv;

    private static bool noConnection = false;
    private static List<long> responseCodes = new List<long>(new long[] { 400, 401, 403, 404, 408 });
    private int logInAttempts = 0;

    private GameObject ErrorBufferGO;
    private PanelHandler panelHandler;
    private UITranslator translator;

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

    private void Awake() {
        SHA256 mySHA256 = SHA256Managed.Create();
        key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(secretSalt));

        // Create secret IV
        iv = new byte[16] { 0x1, 0x4, 0x7, 0x2, 0x5, 0x8, 0x3, 0x6, 0x9, 0x4, 0x7, 0x10, 0x5, 0x8, 0x11, 0x6 };
    }

    private void Start() {
        // initialize panel handler
        panelHandler = GameObject.Find("MainCanvas").GetComponent<PanelHandler>();
        translator = GameObject.Find("MainCanvas").GetComponent<UITranslator>();

        // initialize static list of all students within class
        Student.Students = new List<object>();
        Scenario.Scenarios = new List<object>();
        translator.LoadFigureList();

        // -------TESTING PURPOSES-------
        // Currently used for testing, this is the format to use when asking for data from the database
        // Replace GetUserData with the function that gives the needed info and read it using info[(int) enum PropertyName]
        // The PropertyName enum is found in each database class

        //CreateClassFunc((successful) => {
        //    if (successful)
        //        Debug.Log("Created a new class");
        //}, "ICTGS", "Game Studio");
        //DBConnector.GetClassData((callback) => {
        //    foreach (var classObj in callback) {
        //        PropertyInfo[] info = classObj.GetType().GetProperties();
        //        Debug.Log("Class ID: " + info[(int)ClassProperties.Id].GetValue(classObj, null));
        //        Debug.Log("Class code: " + info[(int)ClassProperties.ClassCode].GetValue(classObj, null));
        //        Debug.Log("Class name: " + info[(int)ClassProperties.Name].GetValue(classObj, null));
        //    }
        //});

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
    public static Coroutine GetUserData(Action<IList> callback, bool isTeacher = true, string teacherEmail = null, string studentName = null, int? class_ID = null) {
        return Instance.StartCoroutine(GetUser(callback, isTeacher, teacherEmail, studentName, class_ID));
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

    public static Coroutine GetMaxScenarioIdFunc(Action<int> callback) {
        return Instance.StartCoroutine(GetMaxScenarioId(callback));
    }

    #endregion

    #region Database Interface Poster Functions

    public static Coroutine CreateTeacherFunc(Action<bool> successful, string name, string email, string password, int classID) {
        return Instance.StartCoroutine(CreateTeacher(successful, name, email, password, classID));
    }

    public static Coroutine CreateStudentFunc(Action<bool> successful, string name, string pincode, int classID) {
        return Instance.StartCoroutine(CreateStudent(successful, name, pincode, classID));
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

    #region Database Interface UpdateOrCreate Functions

    public static Coroutine SaveScenarioContentFunc(Action<bool> successful, Dictionary<ScenarioQuestion, List<ScenarioAnswer>> QnA, string name, int available, int classID, StoryType storytype, int? id = null) {
        return Instance.StartCoroutine(SaveScenarioContent(successful, QnA, name, available, classID, storytype, id));
    }

    public static Coroutine SaveScenarioFunc(Action<int> successful, string name, int available, int classID, StoryType storytype, int? id = null) {
        return Instance.StartCoroutine(SaveScenario(successful, name, available, classID, storytype, id));
    }

    #endregion

    #region Database Getters

    // See comments above function GetUserData for info
    private static IEnumerator GetUser(Action<IList> callback, bool isTeacher = true, string teacherEmail = null, string studentName = null, int? studentPin = null, int? class_ID = null) {
        query = "type=User&method=get&query=";

        if (isTeacher && teacherEmail != null)
            query += "SELECT * FROM teacher WHERE email = '" + teacherEmail + "';";
        else if (isTeacher && class_ID != null)
            query += "SELECT * FROM teacher WHERE class_id = " + class_ID + ";";
        else if (isTeacher)
            query += "SELECT * FROM teacher;";
        else if (studentName != null && studentPin != null)
            query += "SELECT * FROM student WHERE name = '" + studentName + "' AND pincode = " + studentPin + ";";
        else if (class_ID != null)
            query += "SELECT * FROM student WHERE class_id = " + class_ID + ";";
        else
            query += "SELECT * FROM student;";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest info_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return info_get.SendWebRequest();

        if (info_get.isNetworkError || info_get.isHttpError) {
            Debug.LogError("Error ocurred: " + info_get.error);
            if (responseCodes.Contains(info_get.responseCode))
                noConnection = true;
        } else {
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
            // See Decoder function info on workings
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
            callback(JSONDecoder(question_get.downloadHandler.text, typeof(Figure).Name));
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
            callback(JSONDecoder(question_get.downloadHandler.text, typeof(ScenarioAnswer).Name));
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

    private static IEnumerator CreateStudent(Action<bool> successful, string name, string pincode, int classID) {
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

    #endregion

    #region Database UpdateOrCreators

    private static IEnumerator SaveScenario(Action<int> callback, string name, int available, int classID, StoryType storytype, int? id = null) {
        if (id == null)
            yield return GetMaxScenarioIdFunc((temp) => { id = temp; });

        query = "type=Scenario&method=createOrUpdate&query=INSERT INTO scenario (id, name, available, class_id, storytype)" +
                "VALUES (" + id + ", '" + name + "'," + available + "," + classID + ",'" + storytype + "') ON DUPLICATE KEY " +
                "UPDATE name = '" + name + "', available = " + available + ", class_id = " + classID + ", storytype = '" + storytype + "';";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest scenario_updateOrCreate = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return scenario_updateOrCreate.SendWebRequest();

        if (scenario_updateOrCreate.isNetworkError || scenario_updateOrCreate.isHttpError) {
            Debug.LogError("Error occurred: " + scenario_updateOrCreate.error);
        } else {
            callback(int.Parse(Regex.Replace(scenario_updateOrCreate.downloadHandler.text, "[^0-9]+", string.Empty)));
        }
    }

    private static IEnumerator SaveScenarioContent(Action<bool> successful, Dictionary<ScenarioQuestion, List<ScenarioAnswer>> QnA, string name, int available, int classID, StoryType storytype, int? id = null) {
        int? scenarioId = null;

        name = "Test";

        yield return SaveScenarioFunc((callback) => {
            scenarioId = callback;
        }, name, available, classID, storytype, id);

        if (scenarioId != null) {
            query = "type=Figure&method=createOrUpdate&query=" + ScenarioQueryBuilder((int)scenarioId, QnA);

            string encryptedQuery = "&key=" + EncryptString(query, key, iv);
            Debug.Log(query);

            UnityWebRequest figure_createOrUpdate = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
            yield return figure_createOrUpdate.SendWebRequest();

            if (figure_createOrUpdate.isNetworkError || figure_createOrUpdate.isHttpError) {
                Debug.LogError("Error occurred: " + figure_createOrUpdate.error);
                successful(false);
            } else {
                Debug.Log(figure_createOrUpdate.downloadHandler.text);
                successful(true);
            }
        } else {
            Debug.LogError("Scenario ID could not be set, probably due to a network error (should appear above, if not YOU FUCKED UP)");
        }
    }

    private static IEnumerator GetMaxScenarioId(Action<int> callback) {
        query = "type=ScenarioAI&method=get&query=SELECT `AUTO_INCREMENT` FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_SCHEMA = 'id11398216_eduar' AND TABLE_NAME = 'scenario';";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest autoincrement_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return autoincrement_get.SendWebRequest();

        if (autoincrement_get.isNetworkError || autoincrement_get.isHttpError) {
            Debug.LogError("Error occurred: " + autoincrement_get.error);
        } else {
            callback(int.Parse(Regex.Replace(autoincrement_get.downloadHandler.text, "[^0-9]+", string.Empty)));
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
                return JsonConvert.DeserializeObject<List<ScenarioQuestion>>(data);
            case "Answer":
                return JsonConvert.DeserializeObject<List<ScenarioAnswer>>(data);
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
        
        // Attempt to find a valid login by searching for the email address
        GetUserData((callback) => {
            if (callback == null) {
                if (noConnection && logInAttempts <= 3) {
                    LogIn();
                    ++logInAttempts;
                } else {
                    // Incorrect email error
                    ErrorBuffer().text = "Incorrect Credentials";
                    ErrorBuffer().color = Color.red;
                    noConnection = false;
                }
            } else {
                foreach (var teacher in callback) {
                    PropertyInfo[] info = teacher.GetType().GetProperties();
                    if (email == info[(int)TeacherProperties.Email].GetValue(teacher, null).ToString() && password == info[(int)TeacherProperties.Password].GetValue(teacher, null).ToString()) {
                        // Correct login detected, set the current teacher
                        Teacher.currentTeacher = (Teacher)teacher;
                        if (panelHandler != null)
                            panelHandler.LoggedIn();
                    } else {
                        // Incorrect password error
                        ErrorBuffer().text = "Incorrect Credentials";
                        ErrorBuffer().color = Color.red;
                    }
                    noConnection = false;
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

        // If correct email is entered, send a reset mail to the mail address
        GetUserData((callback) => {
            if (callback != null) {
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

    private static string ScenarioQueryBuilder(int scenarioId, Dictionary<ScenarioQuestion, List<ScenarioAnswer>> QnA) {
        string query = "SET @scenario_id=" + scenarioId + ";";
        foreach (ScenarioFigure f in Scenario.CurrentScenarioFigures) {
            query += "INSERT INTO scenario_figures (scenario_id, figure_id, task, information) VALUES(@scenario_id, " + f.Figure_Id + ", '" + f.Task + "', '" + f.Information + "')" +
                    "ON DUPLICATE KEY UPDATE scenario_id = @scenario_id, figure_id = " + f.Figure_Id + ", task = '" + f.Task + "', information = '" + f.Information + "';" +
                    "SET @scenario_figure_id = LAST_INSERT_ID();";
            foreach (var question in QnA) {
                query += "INSERT INTO scenario_questions (scenario_figure_id, question_text) VALUES(@scenario_figure_id, '" + question.Key.Question_Text + "')" +
                    "ON DUPLICATE KEY UPDATE scenario_figure_id = @scenario_figure_id, question_text = '" + question.Key.Question_Text + "';" +
                    "SET @scenario_question_id = LAST_INSERT_ID();";
                foreach(ScenarioAnswer a in question.Value) {
                    query += "INSERT INTO scenario_answers (scenario_question_id, answer_text, correct_answer) VALUES(@scenario_question_id, '" + a.Answer_Text + "', " + a.Correct_Answer + ")" +
                    "ON DUPLICATE KEY UPDATE scenario_question_id = @scenario_question_id, answer_text = '" + a.Answer_Text + "', correct_answer = " + a.Correct_Answer + ";";
                }
            }
        }
        return query;
    }

    public Text ErrorBuffer() {
        return ErrorBufferGO.GetComponent<Text>();
    }
}
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

    private static List<long> responseCodes = new List<long>(new long[] { 400, 401, 403, 404, 408 });
    private int logInAttempts = 0;
    private static int WebRequestAttempts = 0;
    private static int MaxWebRequestAttempts = 20;
    
    private PanelHandler panelHandler;

    public static GameObject MainCanvas;

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
        MainCanvas = GameObject.Find("MainCanvas");

        SHA256 mySHA256 = SHA256Managed.Create();
        key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(secretSalt));

        // Create secret IV
        iv = new byte[16] { 0x1, 0x4, 0x7, 0x2, 0x5, 0x8, 0x3, 0x6, 0x9, 0x4, 0x7, 0x10, 0x5, 0x8, 0x11, 0x6 };
    }

    #region Database Interface Getter Functions

    // Calling the coroutine through this function, rather than calling it directly, allows for the callback to be called outside a coroutine
    // This is required so normal methods can access Database data without needing to be a coroutine
    /// <summary>
    /// Gets data from the database from either the Teacher or Student table. 
    /// Setting the email or name parameters gets data from a specific teacher or student.
    /// </summary>
    public static Coroutine GetUserData(Action<IList> callback, bool isTeacher = true, string teacherEmail = null, string studentName = null, int? studentPin = null, int? class_ID = null) {
        return Instance.StartCoroutine(GetUser(callback, isTeacher, teacherEmail, studentName, studentPin, class_ID));
    }

    public static Coroutine GetScenarioData(Action<IList> callback, int? id = null, string name = null, int? classID = null, StoryType? storytype = null, int? available = null) {
        return Instance.StartCoroutine(GetScenario(callback, id, name, classID, storytype, available));
    }

    public static Coroutine GetFigureData(Action<IList> callback, int? id = null, string name = null) {
        return Instance.StartCoroutine(GetFigure(callback, id, name));
    }

    public static Coroutine GetScenarioFigureData(Action<IList> callback, int? id = null, int? scenario_id = null, int? figure_id = null) {
        return Instance.StartCoroutine(GetScenarioFigure(callback, id, scenario_id, figure_id));
    }

    public static Coroutine GetQuestionData(Action<IList> callback, int? id = null, int? scenario_figure_id = null) {
        return Instance.StartCoroutine(GetQuestion(callback, id, scenario_figure_id));
    }

    public static Coroutine GetAnswerData(Action<IList> callback, int? id = null, int? scenario_question_id = null) {
        return Instance.StartCoroutine(GetAnswer(callback, id, scenario_question_id));
    }

    public static Coroutine GetClassData(Action<IList> callback, int? id = null, string classCode = null, string name = null) {
        return Instance.StartCoroutine(GetClass(callback, id, classCode, name));
    }

    public static Coroutine GetAssociatedData(Action<IList> callback, string data, string type) {
        return Instance.StartCoroutine(StringToObject(callback, data, type));
    }

    #endregion

    #region Database Interface Updater Functions

    public static Coroutine UpdateStudentFunc(Action<bool> successful, int id, string name, int pincode, int classID) {
        return Instance.StartCoroutine(UpdateStudent(successful, id, name, pincode, classID));
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
            if (responseCodes.Contains(info_get.responseCode) && WebRequestAttempts <= MaxWebRequestAttempts) {
                ++WebRequestAttempts;
                GetUserData(callback, isTeacher, teacherEmail, studentName, studentPin, class_ID);
            }
        } else {
            WebRequestAttempts = 0;
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
            if (responseCodes.Contains(scenario_get.responseCode) && WebRequestAttempts <= MaxWebRequestAttempts) {
                ++WebRequestAttempts;
                GetScenarioData(callback, id, name, classID, storytype, available);
            }
        } else {
            WebRequestAttempts = 0;
            // See Decoder function info on workings
            callback(JSONDecoder(scenario_get.downloadHandler.text, typeof(Scenario).Name));
        }
    }

    private static IEnumerator GetFigure(Action<IList> callback, int? id = null, string name = null) {
        query = "type=Figure&method=get&query=";

        if (id != null)
            query += "SELECT * FROM figure WHERE id = " + id + ";";
        else if (name != null)
            query += "SELECT * FROM figure WHERE name LIKE '%" + name + "%';";
        else
            query += "SELECT * FROM figure";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest figure_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return figure_get.SendWebRequest();

        if (figure_get.isNetworkError || figure_get.isHttpError) {
            Debug.LogError("Error occurred: " + figure_get.error);
            if (responseCodes.Contains(figure_get.responseCode) && WebRequestAttempts <= MaxWebRequestAttempts) {
                ++WebRequestAttempts;
                GetFigureData(callback, id, name);
            }
        } else {
            WebRequestAttempts = 0;
            // See Decoder function for info on workings
            callback(JSONDecoder(figure_get.downloadHandler.text, typeof(Figure).Name));
        }
    }

    private static IEnumerator GetScenarioFigure(Action<IList> callback, int? id = null, int? scenario_id = null, int? figure_id = null) {
        query = "type=Figure&method=get&query=";

        if (id != null)
            query += "SELECT * FROM scenario_figure WHERE id = " + id + ";";
        else if (scenario_id != null)
            query += "SELECT * FROM scenario_figure WHERE scenario_id = " + scenario_id + ";";
        else if (figure_id != null)
            query += "SELECT * FROM scenario_figure WHERE figure_id = " + figure_id + ";";
        else
            query += "SELECT * FROM scenario_figure";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest scenario_figure_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return scenario_figure_get.SendWebRequest();

        if (scenario_figure_get.isNetworkError || scenario_figure_get.isHttpError) {
            Debug.LogError("Error occurred: " + scenario_figure_get.error);
            if (responseCodes.Contains(scenario_figure_get.responseCode) && WebRequestAttempts <= MaxWebRequestAttempts) {
                ++WebRequestAttempts;
                GetScenarioFigureData(callback, id, scenario_id, figure_id);
            }
        } else {
            WebRequestAttempts = 0;
            // See Decoder function for info on workings
            callback(JSONDecoder(scenario_figure_get.downloadHandler.text, typeof(ScenarioFigure).Name));
        }
    }

    private static IEnumerator GetQuestion(Action<IList> callback, int? id = null, int? scenario_figure_id = null) {
        query = "type=Question&method=get&query=";

        if (id != null)
            query += "SELECT * FROM scenario_question WHERE id = " + id + ";";
        else if (scenario_figure_id != null)
            query += "SELECT * FROM scenario_question WHERE scenario_figure_id = " + scenario_figure_id + ";";
        else
            query += "SELECT * FROM scenario_question;";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest question_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return question_get.SendWebRequest();

        if (question_get.isNetworkError || question_get.isHttpError) {
            Debug.LogError("Error occurred: " + question_get.error);
            if (responseCodes.Contains(question_get.responseCode) && WebRequestAttempts <= MaxWebRequestAttempts) {
                ++WebRequestAttempts;
                GetQuestionData(callback, id, scenario_figure_id);
            }
        } else {
            WebRequestAttempts = 0;
            // See Decoder function for info on workings
            callback(JSONDecoder(question_get.downloadHandler.text, typeof(ScenarioQuestion).Name));
        }
    }

    private static IEnumerator GetAnswer(Action<IList> callback, int? id = null, int? scenario_question_id = null) {
        query = "type=Answer&method=get&query=";

        if (id != null)
            query += "SELECT * FROM scenario_answer WHERE id = " + id + ";";
        else if (scenario_question_id != null)
            query += "SELECT * FROM scenario_answer WHERE scenario_question_id = " + scenario_question_id + ";";
        else
            query += "SELECT * FROM scenario_answer;";

        string encryptedQuery = "&key=" + EncryptString(query, key, iv);
        UnityWebRequest answer_get = UnityWebRequest.Get(dbUrl + query + encryptedQuery);
        yield return answer_get.SendWebRequest();

        if (answer_get.isNetworkError || answer_get.isHttpError) {
            Debug.LogError("Error occurred: " + answer_get.error);
            if (responseCodes.Contains(answer_get.responseCode) && WebRequestAttempts <= MaxWebRequestAttempts) {
                ++WebRequestAttempts;
                GetAnswerData(callback, id, scenario_question_id);
            }
        } else {
            WebRequestAttempts = 0;
            // See Decoder function for info on workings
            callback(JSONDecoder(answer_get.downloadHandler.text, typeof(ScenarioAnswer).Name));
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
            if (responseCodes.Contains(class_get.responseCode) && WebRequestAttempts <= MaxWebRequestAttempts) {
                ++WebRequestAttempts;
                GetClassData(callback, id, classCode, name);
            }
        } else {
            WebRequestAttempts = 0;
            // See Decoder function for info on workings
            callback(JSONDecoder(class_get.downloadHandler.text, typeof(Class).Name));
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
            if (responseCodes.Contains(student_update.responseCode) && WebRequestAttempts <= MaxWebRequestAttempts) {
                ++WebRequestAttempts;
                UpdateStudentFunc(successful, id, name, pincode, classID);
            } else
                successful(false);
        } else {
            WebRequestAttempts = 0;
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
            case "ScenarioFigure":
                return JsonConvert.DeserializeObject<List<ScenarioFigure>>(data);
            case "ScenarioQuestion":
                return JsonConvert.DeserializeObject<List<ScenarioQuestion>>(data);
            case "ScenarioAnswer":
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
}
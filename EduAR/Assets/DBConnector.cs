using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DBConnector : MonoBehaviour {
    private string query;
    private string dbUrl = "http://localhost/eduar/request.php?";

    private void Start() {
        GetUser(teacherEmail: "test@test.nl");
        GetUser(studentName: "Robert Bisschop");
    }

    public IEnumerator GetUser(bool isTeacher = true, string teacherEmail = null, string studentName = null) {
        if (isTeacher && teacherEmail != null)
            query = "select * from teacher where email = " + teacherEmail + ";";
        else if (isTeacher)
            query = "select * from teacher;";
        else if (studentName != null)
            query = "select * from student where name = "+ studentName + ";";
        else
            query = "select * from student;";

        UnityWebRequest info_get = UnityWebRequest.Get(dbUrl + query);
        yield return info_get.SendWebRequest();

        if (info_get.isNetworkError || info_get.isHttpError) {
            Debug.LogError("Error ocurred: " + info_get.error);
        } else {
            yield return info_get.downloadHandler.text;
        }
    }


}

using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStudentInfo : MonoBehaviour {

    public void ChangeStudentInfoName(InputField field) {
        int id;

        DBConnector.GetUserData((callback) => {
        foreach (object student in callback) {
                PropertyInfo[] info = student.GetType().GetProperties();
                id = int.Parse(info[(int)StudentProperties.Id].GetValue(student, null).ToString());
                InputField[] inputs = field.transform.parent.GetComponentsInChildren<InputField>();

                DBConnector.UpdateStudentFunc((successful) => {
                    Debug.Log("Updated student name to: " + inputs[0].text);    
                    field.text = inputs[0].text;
                }, id, name: inputs[0].text);
            }
        }, isTeacher: false, studentName: field.text);
    }

    public void ChangeStudentInfoPincode(InputField field) {
        int id;

        DBConnector.GetUserData((callback) => {
            foreach (object student in callback) {
                PropertyInfo[] info = student.GetType().GetProperties();
                id = int.Parse(info[(int)StudentProperties.Id].GetValue(student, null).ToString());
                InputField[] inputs = field.transform.parent.GetComponentsInChildren<InputField>();

                DBConnector.UpdateStudentFunc((successful) => {
                    Debug.Log("Updated student pincode to: " + inputs[1].text);
                }, id, pincode: int.Parse(inputs[1].text));
            }
        }, isTeacher: false, studentName: field.text);
    }
}

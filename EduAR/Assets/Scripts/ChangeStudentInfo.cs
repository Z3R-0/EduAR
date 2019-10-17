using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStudentInfo : MonoBehaviour {
    int id;

    public void ChangeStudentInfoName(InputField field) {
        DBConnector.GetUserData((callback) => {
        foreach (object student in callback) {
                PropertyInfo[] info = student.GetType().GetProperties();
                id = int.Parse(info[(int)StudentProperties.Id].GetValue(student, null).ToString());
                InputField[] inputs = field.transform.parent.GetComponentsInChildren<InputField>();

                DBConnector.UpdateStudentFunc((successful) => { 
                    field.text = inputs[0].text;
                }, id, name: inputs[0].text);
            }
        }, isTeacher: false, studentName: field.text);
    }

    public void ChangeStudentInfoPincode(InputField field) {
        DBConnector.GetUserData((callback) => {
            foreach (object student in callback) {
                PropertyInfo[] info = student.GetType().GetProperties();
                id = int.Parse(info[(int)StudentProperties.Id].GetValue(student, null).ToString());
                InputField[] inputs = field.transform.parent.GetComponentsInChildren<InputField>();

                DBConnector.UpdateStudentFunc((successful) => { }, id, pincode: int.Parse(inputs[1].text));
            }
        }, isTeacher: false, studentName: field.text);
    }
}

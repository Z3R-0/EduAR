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

                int pin = int.Parse(info[(int)StudentProperties.Pincode].GetValue(student, null).ToString());
                int classID = int.Parse(info[(int)StudentProperties.ClassID].GetValue(student, null).ToString());

                DBConnector.UpdateStudentFunc((successful) => { 
                    field.text = inputs[0].text;
                }, id, inputs[0].text, pin, classID);
            }
        }, isTeacher: false, studentName: field.text);
    }

    public void ChangeStudentInfoPincode(InputField field) {
        DBConnector.GetUserData((callback) => {
            foreach (object student in callback) {
                PropertyInfo[] info = student.GetType().GetProperties();
                id = int.Parse(info[(int)StudentProperties.Id].GetValue(student, null).ToString());
                InputField[] inputs = field.transform.parent.GetComponentsInChildren<InputField>();

                string name = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
                int classID = int.Parse(info[(int)StudentProperties.ClassID].GetValue(student, null).ToString());

                DBConnector.UpdateStudentFunc((successful) => { }, id, name, int.Parse(inputs[1].text), classID);
            }
        }, isTeacher: false, studentName: field.text);
    }
}

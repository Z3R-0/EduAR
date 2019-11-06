using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStudentInfo : MonoBehaviour {
    int id;

    public void ChangeStudentInfoName(InputField field) {
        foreach (object student in Student.Students) {
            PropertyInfo[] info = student.GetType().GetProperties();
            InputField[] inputs = field.transform.parent.GetComponentsInChildren<InputField>();
            id = int.Parse(info[(int)StudentProperties.Id].GetValue(student, null).ToString());

            if (id == int.Parse(inputs[3].text)) {
                int pin = int.Parse(info[(int)StudentProperties.Pincode].GetValue(student, null).ToString());
                int classID = int.Parse(info[(int)StudentProperties.ClassID].GetValue(student, null).ToString());

                DBConnector.UpdateStudentFunc((successful) => {
                    info[(int)StudentProperties.Name].SetValue(student, inputs[0].text);
                    field.text = inputs[0].text;
                }, id, inputs[0].text, pin, classID);
            }
        }
    }

    public void ChangeStudentInfoPincode(InputField field) {
        foreach (object student in Student.Students) {
            PropertyInfo[] info = student.GetType().GetProperties();
            InputField[] inputs = field.transform.parent.GetComponentsInChildren<InputField>();
            id = int.Parse(info[(int)StudentProperties.Id].GetValue(student, null).ToString());

            if (id == int.Parse(inputs[3].text)) {
                string name = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
                int classID = int.Parse(info[(int)StudentProperties.ClassID].GetValue(student, null).ToString());
                DBConnector.UpdateStudentFunc((successful) => {
                    info[(int)StudentProperties.Pincode].SetValue(student, int.Parse(inputs[1].text));
                    field.text = inputs[1].text;
                }, id, name, int.Parse(inputs[1].text), classID);
            }
        }
    }
}

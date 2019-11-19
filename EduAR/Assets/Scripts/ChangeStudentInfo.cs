using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStudentInfo : MonoBehaviour {
    private Text studentErrorBuffer;
    int studentId;

    private void Start() {
        studentErrorBuffer = GameObject.Find("StudentListErrorBuffer").GetComponent<Text>();
    }

    public void ChangeStudentInfoFunc(InputField field) {
        studentErrorBuffer.text = "";
        foreach (object student in Student.Students) {
            PropertyInfo[] info = student.GetType().GetProperties();
            InputField[] inputs = field.transform.parent.GetComponentsInChildren<InputField>();
            studentId = int.Parse(info[(int)StudentProperties.Id].GetValue(student, null).ToString());

            if(inputs[1].text.Length < 4) {
                studentErrorBuffer.color = Color.red;
                studentErrorBuffer.text = "Pincode bestaat uit 4 nummers (niet meer, niet minder)";
            }

            if (studentId == int.Parse(inputs[2].text)) {
                int classID = int.Parse(info[(int)StudentProperties.ClassID].GetValue(student, null).ToString());

                DBConnector.UpdateStudentFunc((successful) => {
                    info[(int)StudentProperties.Name].SetValue(student, inputs[0].text);
                    studentErrorBuffer.color = Color.green;
                    studentErrorBuffer.text = "Student succesvol aangepast";
                }, studentId, inputs[0].text, int.Parse(inputs[1].text), classID);
            }
        }
    }
}

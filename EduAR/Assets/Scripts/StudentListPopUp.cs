using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class StudentListPopUp : MonoBehaviour {
    [SerializeField]
    private GameObject studentContainerPrefab;

    public void TogglePanel(GameObject panel) {
        if (panel.activeInHierarchy)
            panel.SetActive(false);
        else
            panel.SetActive(true);
    }

    private void Start() {
        DBConnector.GetUserData((callback) => {
            foreach (var student in callback) {
                Student.Students.Add(student);
                PropertyInfo[] info = student.GetType().GetProperties();
                Instantiate(studentContainerPrefab, GameObject.Find("Grid").transform);
                InputField[] inputs = studentContainerPrefab.GetComponentsInChildren<InputField>();
                inputs[0].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
                inputs[1].text = info[(int)StudentProperties.Pincode].GetValue(student, null).ToString();
                inputs[2].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
                inputs[3].text = info[(int)StudentProperties.Id].GetValue(student, null).ToString();
            }
        }, isTeacher: false);
    }

    private void Update() {
        foreach (var student in Student.Students) {
            PropertyInfo[] info = student.GetType().GetProperties();
            InputField[] inputs = studentContainerPrefab.GetComponentsInChildren<InputField>();
            inputs[0].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
            inputs[1].text = info[(int)StudentProperties.Pincode].GetValue(student, null).ToString();
            inputs[2].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
            inputs[3].text = info[(int)StudentProperties.Id].GetValue(student, null).ToString();
        }
    }
}

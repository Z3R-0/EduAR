using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class StudentListHandler : MonoBehaviour {
    [SerializeField]
    private GameObject StudentListPrefab;
    [SerializeField]
    private Transform StudentPrefabParent;

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
                Instantiate(StudentListPrefab, StudentPrefabParent.transform);
                Text[] texts = StudentListPrefab.GetComponentsInChildren<Text>();
                InputField[] inputs = StudentListPrefab.GetComponentsInChildren<InputField>();
                inputs[0].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
                inputs[1].text = info[(int)StudentProperties.Pincode].GetValue(student, null).ToString();
                texts[2].text = info[(int)StudentProperties.Id].GetValue(student, null).ToString();
                texts[3].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
            }
        }, isTeacher: false);
    }

    private void Update() {
        foreach (var student in Student.Students) {
            PropertyInfo[] info = student.GetType().GetProperties();
            Text[] texts = StudentListPrefab.GetComponentsInChildren<Text>();
            InputField[] inputs = StudentListPrefab.GetComponentsInChildren<InputField>();
            inputs[0].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
            inputs[1].text = info[(int)StudentProperties.Pincode].GetValue(student, null).ToString();
            texts[2].text = info[(int)StudentProperties.Id].GetValue(student, null).ToString();
            texts[3].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
        }
    }
}

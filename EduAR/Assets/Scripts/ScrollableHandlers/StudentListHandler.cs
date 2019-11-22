using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class StudentListHandler : MonoBehaviour {
    [SerializeField]
    private GameObject StudentListPrefab;
    [SerializeField]
    private Transform StudentPrefabParent;

    private void OnEnable() {
        InitializeStudentList();
    }

    public void Clear() {
        foreach (Transform child in StudentPrefabParent) {
            Destroy(child.gameObject);
        }
    }

    private void InitializeStudentList() {
        DBConnector.GetUserData((callback) => {
            foreach (var student in callback) {
                Student.Students.Add(student);
                PropertyInfo[] info = student.GetType().GetProperties();
                Instantiate(StudentListPrefab, StudentPrefabParent.transform);
                InputField[] inputs = StudentListPrefab.GetComponentsInChildren<InputField>();
                inputs[0].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
                inputs[1].text = info[(int)StudentProperties.Pincode].GetValue(student, null).ToString();
                inputs[2].text = info[(int)StudentProperties.Id].GetValue(student, null).ToString();
            }

            Clear();

            foreach (var student in callback) {
                Student.Students.Add(student);
                PropertyInfo[] info = student.GetType().GetProperties();
                Instantiate(StudentListPrefab, StudentPrefabParent.transform);
                InputField[] inputs = StudentListPrefab.GetComponentsInChildren<InputField>();
                inputs[0].text = info[(int)StudentProperties.Name].GetValue(student, null).ToString();
                inputs[1].text = info[(int)StudentProperties.Pincode].GetValue(student, null).ToString();
                inputs[2].text = info[(int)StudentProperties.Id].GetValue(student, null).ToString();
            }
        }, isTeacher: false, class_ID: Teacher.currentTeacher.Class_ID);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITranslator : MonoBehaviour {
    [SerializeField]
    private InputField scenarioNameInputField;
    [SerializeField]
    private Toggle scenarioAvailableToggle;
    [SerializeField]
    private Dropdown scenarioStoryTypeDropDown;
    [SerializeField]
    private InputField studentNameInputField;
    [SerializeField]
    private InputField studentPincodeInputField;

    private PanelHandler panelHandler;

    private List<string> createScenarioStrings = new List<string>(new string[] { "name", "available", "figures", "class_id", "storytype" });
    private List<string> addStudentStrings = new List<string>(new string[] { "name", "pincode", "class_id" });
    private string figures;

    private void Start() {
        panelHandler = GameObject.Find("MainCanvas").GetComponent<PanelHandler>();
    }

    public void CreateNewScenario() {
        Dictionary<string, object> info = GetScenarioCreationValues();

        try {
            foreach (var item in info) {
                if (!createScenarioStrings.Contains(item.Key))
                    throw new System.ArgumentException();
            }
        } catch (System.Exception e) {
            Debug.LogError("Dictionary not properly initialized");
        }

        DBConnector.CreateScenarioFunc((successful) => {
            if (successful)
                Debug.Log("Successfully created scenario"); // Add a feature to let users know of success or error
            else
                Debug.LogError("Something went wrong, read error above for more info");
        }, (string)info[createScenarioStrings[0]], (int)info[createScenarioStrings[1]], (string)info[createScenarioStrings[2]], (int)info[createScenarioStrings[3]], (StoryType)info[createScenarioStrings[4]]);
    }

    public void AddStudent() {
        Dictionary<string, object> info = GetStudentAddValues();

        try {
            foreach (var item in info) {
                if (!addStudentStrings.Contains(item.Key))
                    throw new System.ArgumentException();
            }
        } catch (System.Exception e) {
            Debug.LogError("Dictionary not properly initialized");
        }

        DBConnector.CreateStudentFunc((successful) => {
            if (successful) { 
                DBConnector.GetUserData((callback) => {
                    Student.Students.Add(callback[0]);    
                }, isTeacher: false, studentName: (string)info[addStudentStrings[0]]);
                panelHandler.CloseAddStudent();
            } else {
                Debug.LogError("Something went wrong, read error above for more info");
            }
        }, (string) info[addStudentStrings[0]], (int) info[addStudentStrings[1]], (int) info[addStudentStrings[2]]);
    }

    private Dictionary<string, object> GetScenarioCreationValues() {
        Dictionary<string, object> result = new Dictionary<string, object>();

        result.Add("name", scenarioNameInputField.text);
        if (scenarioAvailableToggle.isOn)
            result.Add("available", 1);
        else
            result.Add("available", 0);
        result.Add("figures", UpdateFigureList());
        result.Add("class_id", Teacher.currentTeacher.Class_ID);
        result.Add("storytype", scenarioStoryTypeDropDown.itemText);

        return result;
    }

    private Dictionary<string, object> GetStudentAddValues() {
        Dictionary<string, object> result = new Dictionary<string, object>();

        result.Add("name", studentNameInputField.text);
        result.Add("pincode", int.Parse(studentPincodeInputField.text));
        result.Add("class_id", Teacher.currentTeacher.Class_ID);

        return result;
    }

    public string UpdateFigureList() {
        throw new System.NotImplementedException();
    }

    public void AddFigureParameters() {
        // Add questions, answers and informational text to the appropiate figures here
        throw new System.NotImplementedException();
    }
}

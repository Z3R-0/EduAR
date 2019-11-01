using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioEdit : MonoBehaviour {
    [SerializeField]
    private InputField nameInputField;
    [SerializeField]
    private Toggle availableToggle;
    [SerializeField]
    private Dropdown storyTypeDropDown;

    private List<string> strings = new List<string>(new string[] { "name", "available", "figures", "classid", "story type" });
    private string class_id;
    private string figures;
    

    public void CreateNewScenario() {
        Dictionary<string, object> info = GetScenarioCreationValues();

        foreach(var item in info) {
            if (!strings.Contains(item.Key))
                throw new System.ArgumentException();
        }

        DBConnector.CreateScenarioFunc((successful) => {
            if (successful)
                Debug.Log("You did it!");
            else
                Debug.LogError("Something went wrong, read error above for more info");
        },(string) info[strings[0]], (int) info[strings[1]], (string) info[strings[2]], (int) info[strings[3]], (StoryType) info[strings[4]]);
    }

    private Dictionary<string, object> GetScenarioCreationValues() {
        Dictionary<string, object> result = new Dictionary<string, object>();

        result.Add("name", nameInputField.text);
        if (availableToggle.isOn)
            result.Add("available", 1);
        else
            result.Add("available", 0);
        result.Add("figures", figures);
        result.Add("class id", Teacher.currentTeacher.Class_ID);
        result.Add("story type", storyTypeDropDown.itemText);

        return result;
    }

    public void UpdateFigureList() {
        throw new System.NotImplementedException();
    }

    public void AddFigureParameters() {
        // Add questions, answers and informational text to the appropiate figures here
        throw new System.NotImplementedException();
    }
}

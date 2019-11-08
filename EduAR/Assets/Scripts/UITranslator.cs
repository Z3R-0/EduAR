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
    private Text hiddenScenarioIdField;
    [SerializeField]
    private InputField studentNameInputField;
    [SerializeField]
    private InputField studentPincodeInputField;
    [SerializeField]
    private Text hiddenStudentIdField;

    private PanelHandler panelHandler;
    private FigurePanel figurePanelRef;
    private Dictionary<GameObject, FigurePanel> propertiesPanels;

    private List<string> createScenarioStrings = new List<string>(new string[] { "name", "available", "figures", "class_id", "storytype" });
    private List<string> addStudentStrings = new List<string>(new string[] { "name", "pincode", "class_id" });
    private string figures;

    private void Start() {
        panelHandler = GameObject.Find("MainCanvas").GetComponent<PanelHandler>();
        figurePanelRef = GameObject.Find("PanelHolder").GetComponent<FigurePanel>();
        propertiesPanels = new Dictionary<GameObject, FigurePanel>();
    }

    public void CreateNewScenario() {
        Dictionary<string, object> info = GetScenarioCreationValues();

        try {
            foreach (var item in info) {
                if (!createScenarioStrings.Contains(item.Key))
                    throw new System.ArgumentException();
            }
        } catch (System.Exception e) {
            Debug.LogError("Dictionary not properly initialized, error: " + e.Message);
        }

        //DBConnector.CreateScenarioFunc((successful) => {
        //    if (successful)
        //        Debug.Log("Successfully created scenario"); // Add a feature to let users know of success or error
        //    else
        //        Debug.LogError("Something went wrong, read error above for more info");
        //}, (string)info[createScenarioStrings[0]], (int)info[createScenarioStrings[1]], (string)info[createScenarioStrings[2]], (int)info[createScenarioStrings[3]], (StoryType)info[createScenarioStrings[4]]);
    }

    public void AddStudent() {
        Dictionary<string, object> info = GetStudentAddValues();

        try {
            foreach (var item in info) {
                if (!addStudentStrings.Contains(item.Key))
                    throw new System.ArgumentException();
            }
        } catch (System.Exception e) {
            Debug.LogError("Dictionary not properly initialized, error: " + e.Message);
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
        }, (string)info[addStudentStrings[0]], (int)info[addStudentStrings[1]], (int)info[addStudentStrings[2]]);
    }

    private Dictionary<string, object> GetScenarioCreationValues() {
        Dictionary<string, object> result = new Dictionary<string, object>();

        result.Add("name", scenarioNameInputField.text);
        if (scenarioAvailableToggle.isOn)
            result.Add("available", 1);
        else
            result.Add("available", 0);
        result.Add("figures", GetCurrentScenarioFigureList());
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

    public string GetCurrentScenarioFigureList() {
        throw new System.NotImplementedException();
    }

    public void AddFigure(Text hiddenFigureId) {
        if (Scenario.CurrentScenarioFigures == null)
            Scenario.CurrentScenarioFigures = new List<object>();

        Figure temp = null;

        foreach (Figure f in Figure.FigureList) {
            if (f.Id == int.Parse(hiddenFigureId.text)) {
                temp = f;
            }
        }
        Scenario.CurrentScenarioFigures.Add(temp);
        GameObject newPanel = figurePanelRef.InstantiatePanel();
        propertiesPanels.Add(newPanel, newPanel.GetComponent<FigurePanel>());
    }
    
    public void AddQuestion() {
        figurePanelRef.InstantiateQuestion();
    }

    public void AddAnswer(Transform parentQuestion) {
        figurePanelRef.InstantiateAnswer(parentQuestion);
    }

    // TODO ADD ANSWER/QUESTION/FIGURE REMOVERS --------------------------------------------------------------------------------------------------------------------------------------
    // PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT PLEASE DO IT

    public void SaveScenarioButtonFunc() {
        List<ScenarioQuestions> questions = null;
        List<ScenarioAnswers> answers = null;

        foreach (var panel in propertiesPanels) {
            propertiesPanels[panel.Key] = figurePanelRef.UpdateParameters(panel.Value);
            questions = FigurePanelUIConverterQuestions(propertiesPanels[panel.Key]);
            answers = FigurePanelUIConverterAnswers(propertiesPanels[panel.Key]);
        }

        int available = 0;
        if (scenarioAvailableToggle.isOn)
            available = 1;

        foreach (Figure f in Scenario.CurrentScenarioFigures) {
            DBConnector.SaveScenarioContentFunc((successful) => {

            }, questions, answers, scenarioNameInputField.text, available, Teacher.currentTeacher.Class_ID, (StoryType)scenarioStoryTypeDropDown.value, int.Parse(hiddenScenarioIdField.text));
        }
    }

    private List<ScenarioQuestions> FigurePanelUIConverterQuestions(FigurePanel fp) {
        throw new System.NotImplementedException();
    }

    private List<ScenarioAnswers> FigurePanelUIConverterAnswers(FigurePanel fp) {
        throw new System.NotImplementedException();
    }

    public void LoadFigureList() {
        Figure.FigureList.Clear();
        DBConnector.GetFigureData((callback) => {
            foreach (object figure in callback) {
                Figure.FigureList.Add(figure);
            }
        });
    }

    public void LoadScenarioDetails(Text hiddenScenarioId) {
        hiddenScenarioIdField.text = hiddenScenarioId.text;
        Debug.Log("HiddenId = " + hiddenScenarioId.ToString());
        if (hiddenScenarioId.text == "" || hiddenScenarioId == null) {
            scenarioAvailableToggle.isOn = true;
            scenarioNameInputField.text = "";
            scenarioStoryTypeDropDown.value = (int)StoryType.Scavenger;
        } else {
            Scenario temp = null;

            foreach (Scenario s in Scenario.Scenarios) {
                if (s.Id == int.Parse(hiddenScenarioIdField.text))
                    temp = s;
            }

            if (temp != null) {
                if (temp.Available == 0)
                    scenarioAvailableToggle.isOn = false;
                else
                    scenarioAvailableToggle.isOn = true;
                scenarioNameInputField.text = temp.Name;
                scenarioStoryTypeDropDown.value = (int)temp.StoryType;
            } else {
                Debug.LogError("Something went wrong while trying to load scenario details of scenario id: " + hiddenScenarioIdField.text);
            }
        }
    }
}

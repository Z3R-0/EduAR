using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    private GameObject FigureListPrefab;
    [SerializeField]
    private Transform FigurePrefabParent;
    [SerializeField]
    private GameObject StudentListPrefab;
    [SerializeField]
    private Transform StudentPrefabParent;

    private PanelHandler panelHandler;
    private FigurePanel figurePanelRef;
    private Dictionary<GameObject, FigurePanel> propertiesPanels;

    private List<string> createScenarioStrings = new List<string>(new string[] { "name", "available", "figures", "class_id", "storytype" });
    private List<string> addStudentStrings = new List<string>(new string[] { "name", "pincode", "class_id" });
    private string figures;

    private void Start() {
        panelHandler = GameObject.Find("MainCanvas").GetComponent<PanelHandler>();
        figurePanelRef = GameObject.Find("MainCanvas").GetComponent<FigurePanel>();
        propertiesPanels = new Dictionary<GameObject, FigurePanel>();
    }

    //public void CreateNewScenario() {
    //    Dictionary<string, object> info = GetScenarioCreationValues();

    //    try {
    //        foreach (var item in info) {
    //            if (!createScenarioStrings.Contains(item.Key))
    //                throw new System.ArgumentException();
    //        }
    //    } catch (System.Exception e) {
    //        Debug.LogError("Dictionary not properly initialized, error: " + e.Message);
    //    }

    //    //DBConnector.CreateScenarioFunc((successful) => {
    //    //    if (successful)
    //    //        Debug.Log("Successfully created scenario"); // Add a feature to let users know of success or error
    //    //    else
    //    //        Debug.LogError("Something went wrong, read error above for more info");
    //    //}, (string)info[createScenarioStrings[0]], (int)info[createScenarioStrings[1]], (string)info[createScenarioStrings[2]], (int)info[createScenarioStrings[3]], (StoryType)info[createScenarioStrings[4]]);
    //}

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
                    Student student = (Student)callback[0];
                    PropertyInfo[] properties = student.GetType().GetProperties();
                    GameObject studentPrefab = Instantiate(StudentListPrefab, StudentPrefabParent.transform);
                    Text[] texts = studentPrefab.GetComponentsInChildren<Text>();
                    InputField[] inputs = studentPrefab.GetComponentsInChildren<InputField>();
                    inputs[0].text = properties[(int)StudentProperties.Name].GetValue(student, null).ToString();
                    inputs[1].text = properties[(int)StudentProperties.Pincode].GetValue(student, null).ToString();
                    texts[2].text = properties[(int)StudentProperties.Id].GetValue(student, null).ToString();
                    texts[3].text = properties[(int)StudentProperties.Name].GetValue(student, null).ToString();
                }, isTeacher: false, studentName: (string)info[addStudentStrings[0]]);
                panelHandler.CloseAddStudent();
            } else {
                Debug.LogError("Something went wrong, read error above for more info");
            }
        }, (string)info[addStudentStrings[0]], (string)info[addStudentStrings[1]], (int)info[addStudentStrings[2]]);
    }

    //private Dictionary<string, object> GetScenarioCreationValues() {
    //    Dictionary<string, object> result = new Dictionary<string, object>();

    //    result.Add("name", scenarioNameInputField.text);
    //    if (scenarioAvailableToggle.isOn)
    //        result.Add("available", 1);
    //    else
    //        result.Add("available", 0);
    //    result.Add("figures", GetCurrentScenarioFigureList());
    //    result.Add("class_id", Teacher.currentTeacher.Class_ID);
    //    result.Add("storytype", scenarioStoryTypeDropDown.itemText);

    //    return result;
    //}

    private Dictionary<string, object> GetStudentAddValues() {
        Dictionary<string, object> result = new Dictionary<string, object>();

        result.Add("name", studentNameInputField.text);
        result.Add("pincode", studentPincodeInputField.text);
        result.Add("class_id", Teacher.currentTeacher.Class_ID);

        return result;
    }

    //public string GetCurrentScenarioFigureList() {
    //    throw new System.NotImplementedException();
    //}

    public void AddFigure(string hiddenFigureId) {
        if (Scenario.CurrentScenarioFigures == null)
            Scenario.CurrentScenarioFigures = new List<ScenarioFigure>();

        ScenarioFigure temp = null;

        foreach (Figure f in Figure.FigureList) {
            if (f.Id == int.Parse(hiddenFigureId)) {
                temp = new ScenarioFigure(f.Id);
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
        List<Dictionary<ScenarioQuestion, List<ScenarioAnswer>>> scenarioQnA = new List<Dictionary<ScenarioQuestion, List<ScenarioAnswer>>>();
        Dictionary<GameObject, FigurePanel> PanelTemp = new Dictionary<GameObject, FigurePanel>();
        int? hiddenId = null;

        foreach (var panel in propertiesPanels) {
            PanelTemp[panel.Key] = figurePanelRef.UpdateParameters(panel.Value);
        }

        foreach (var panel in PanelTemp) {
            scenarioQnA.Add(QnATranslator(panel.Value));
        }

        int available = 0;
        if (scenarioAvailableToggle.isOn)
            available = 1;

        if (hiddenScenarioIdField.text != "")
            hiddenId = int.Parse(hiddenScenarioIdField.text);

        foreach(Dictionary<ScenarioQuestion, List<ScenarioAnswer>> dick in scenarioQnA) {
            foreach(var question in dick) {
                foreach(var answer in question.Value) {
                }
            }
        }

        for (int i = 0; i <= Scenario.CurrentScenarioFigures.Count - 1; i++) {
            DBConnector.SaveScenarioContentFunc((successful) => {
            }, scenarioQnA[i], scenarioNameInputField.text, available, Teacher.currentTeacher.Class_ID, (StoryType)scenarioStoryTypeDropDown.value, hiddenId);
        }
    }

    private Dictionary<ScenarioQuestion, List<ScenarioAnswer>> QnATranslator(FigurePanel fp) {
        Dictionary<ScenarioQuestion, List<ScenarioAnswer>> result = new Dictionary<ScenarioQuestion, List<ScenarioAnswer>>();

        foreach (var question in fp.questionsAndAnswers) {
            List<ScenarioAnswer> answersTemp = new List<ScenarioAnswer>();
            foreach (var answer in question.Value) {
                int trueOrFalse = 0;
                if (answer.Value)
                    trueOrFalse = 1;
                answersTemp.Add(new ScenarioAnswer(answer.Key.text, trueOrFalse));
            }
            result.Add(new ScenarioQuestion(question.Key.text), answersTemp);
        }

        return result;
    }

    public void LoadFigureList() {
        Figure.FigureList.Clear();
        DBConnector.GetFigureData((callback) => {
            foreach (object figure in callback) {
                Figure.FigureList.Add((Figure)figure);
                InstantiateFigureList((Figure)figure);
            }
        });
    }

    private void InstantiateFigureList(Figure f) {
        GameObject instance = Instantiate(FigureListPrefab, FigurePrefabParent);
        foreach(Transform go in instance.transform) {
            if (go.tag == "FigureName")
                go.GetComponent<Text>().text = f.Name;
            if (go.tag == "FigureHiddenId")
                go.GetComponent<Text>().text = f.Id.ToString();
        }
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

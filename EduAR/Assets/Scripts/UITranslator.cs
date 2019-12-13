using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    private Text CreateScenarioErrorBuffer;

    private PanelHandler panelHandler;
    private FigurePanel figurePanelRef;
    private Dictionary<GameObject, FigurePanel> propertiesPanels;
    private Dictionary<GameObject, FigurePanel> toBeRemovedPanels;
    private Dictionary<ScenarioFigure, Dictionary<ScenarioQuestion, List<ScenarioAnswer>>> figuresWithQnA = new Dictionary<ScenarioFigure, Dictionary<ScenarioQuestion, List<ScenarioAnswer>>>();


    private List<string> createScenarioStrings = new List<string>(new string[] { "name", "available", "figures", "class_id", "storytype" });
    private List<string> addStudentStrings = new List<string>(new string[] { "name", "pincode", "class_id" });

    private void Start() {
        // Set necessary references
        panelHandler = DBConnector.MainCanvas.GetComponent<PanelHandler>();
        figurePanelRef = DBConnector.MainCanvas.GetComponent<FigurePanel>();
        // Initialize lists 
        propertiesPanels = new Dictionary<GameObject, FigurePanel>();
        toBeRemovedPanels = new Dictionary<GameObject, FigurePanel>();
        if (Scenario.CurrentScenarioFigures == null)
            Scenario.CurrentScenarioFigures = new Dictionary<int, ScenarioFigure>();
    }

    /// <summary>
    /// Clear all lists containing information of current scenario info. Call this when opening a new scenario (new, empty or otherwise)
    /// </summary>
    public void Clear() {
        if (Scenario.CurrentScenarioFigures != null)
            Scenario.CurrentScenarioFigures.Clear();
        hiddenScenarioIdField.text = "";
        figuresWithQnA.Clear();
        propertiesPanels.Clear();
        GameObject[] extFigures = GameObject.FindGameObjectsWithTag("ScenarioFigure");
        foreach (GameObject figure in extFigures) {
            Destroy(figure);
        }
    }

    public void RemoveNewPanel(GameObject panel) {
        propertiesPanels.Remove(panel);
    }

    public void RemoveExistingPanel(GameObject panel) {
        toBeRemovedPanels.Add(panel, panel.GetComponent<FigurePanel>());
        propertiesPanels.Remove(panel);
    }

    /// <summary>
    /// Adds a student to the database and student list
    /// </summary>
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
                }, isTeacher: false, studentName: (string)info[addStudentStrings[0]]);
                panelHandler.CloseAddStudent();
            } else {
                Debug.LogError("Something went wrong, read error above for more info");
            }
        }, (string)info[addStudentStrings[0]], (string)info[addStudentStrings[1]], (int)info[addStudentStrings[2]]);
    }

    private Dictionary<string, object> GetStudentAddValues() {
        Dictionary<string, object> result = new Dictionary<string, object>();

        result.Add("name", studentNameInputField.text);
        result.Add("pincode", studentPincodeInputField.text);
        result.Add("class_id", Teacher.currentTeacher.Class_ID);

        return result;
    }

    /// <summary>
    /// Adds a new ScenarioFigure to the current scenario, adding all required info to it
    /// </summary>
    /// <param name="hiddenFigureId">ID of the Figure this ScenarioFigure is based on</param>
    /// <param name="figure">Existing ScenarioFigure to copy into the scenario</param>
    /// <returns></returns>
    public GameObject AddFigure(string hiddenFigureId, ScenarioFigure figure = null) {
        ScenarioFigure temp = null;
        string image = "";

        // Get values from the Figure to add to this ScenarioFigure
        foreach (Figure f in Figure.FigureList) {
            if (f.Id == int.Parse(hiddenFigureId)) {
                
                if (figure != null)
                    temp = figure;
                else
                    temp = new ScenarioFigure(f.Id);
                image = f.Image;
            }
        }
        
        Scenario.CurrentScenarioFigures.Add(FigurePanel.currentIndex, temp);
        GameObject newPanel = figurePanelRef.InstantiatePanel();
        if (figure != null) {
            newPanel.GetComponentInChildren<Slider>().GetComponent<Text>().text = figure.Id.ToString();
            newPanel.GetComponent<FigurePanel>().hiddenScenarioFigureId = figure.Id;
        }
        newPanel.GetComponent<FigurePanel>().hiddenIndex = FigurePanel.currentIndex + 1;
        FigurePanel.currentIndex++;

        Image[] images = newPanel.GetComponentsInChildren<Image>();
        foreach (Image i in images) {
            if (i.tag == "FigureImage")
                i.sprite = Resources.Load<Sprite>(image);
        }
        propertiesPanels.Add(newPanel, newPanel.GetComponent<FigurePanel>());
        return newPanel;
    }

    public void AddQuestion() {
        figurePanelRef.InstantiateQuestion();
    }

    public void AddAnswer(Transform parentQuestion) {
        figurePanelRef.InstantiateAnswer(parentQuestion);
    }

    /// <summary>
    /// Function that is called when the Save button is pressed. Gets all inputted information and formats it for inserting into the database
    /// </summary>
    public void SaveScenarioButtonFunc() {
        List<Dictionary<ScenarioQuestion, List<ScenarioAnswer>>> scenarioQnA = new List<Dictionary<ScenarioQuestion, List<ScenarioAnswer>>>();
        Dictionary<GameObject, FigurePanel> currentFigurePanels = new Dictionary<GameObject, FigurePanel>();

        int? hiddenId = null;

        if (scenarioNameInputField.text == "") {
            CreateScenarioErrorBuffer.color = Color.red;
            CreateScenarioErrorBuffer.text = "Geef het scenario een naam";
            Invoke("ClearText", 2f);
            return;
        }

        // Get all the text from inputfields
        foreach (var panel in propertiesPanels) {
            currentFigurePanels[panel.Key] = panel.Value.UpdateParameters(panel.Value);
        }

        // Get any IDs of existing questions and answers and additional database information
        foreach (var panel in currentFigurePanels) {
            scenarioQnA.Add(QnATranslator(panel.Value));
        }

        int available = 0;
        if (scenarioAvailableToggle.isOn)
            available = 1;

        if (hiddenScenarioIdField.text != "")
            hiddenId = int.Parse(hiddenScenarioIdField.text);

        DBConnector.SaveScenarioContentFunc((successful) => {
            if (hiddenScenarioIdField.text == "") {
                DBConnector.GetMaxScenarioIdFunc((callback) => {
                    hiddenScenarioIdField.text = (callback - 1).ToString();
                });
            }
            if (toBeRemovedPanels.Count > 0) {
                DBConnector.DeleteScenarioContentFunc((successfulDelete) => { }, toBeRemovedPanels);
            }
            CreateScenarioErrorBuffer.color = Color.green;
            CreateScenarioErrorBuffer.text = "Scenario opgeslagen";

            Invoke("ClearText", 2f);

        }, scenarioQnA, scenarioNameInputField.text, available, Teacher.currentTeacher.Class_ID, (StoryType)scenarioStoryTypeDropDown.value, hiddenId);
    }

    private void ClearText() {
        CreateScenarioErrorBuffer.text = "";
    }

    /// <summary>
    /// Gets hidden values and additional info of a scenario necessary for updating the database
    /// </summary>
    /// <param name="fp">FigurePanel to use for formatting information</param>
    /// <returns></returns>
    private Dictionary<ScenarioQuestion, List<ScenarioAnswer>> QnATranslator(FigurePanel fp) {
        Dictionary<ScenarioQuestion, List<ScenarioAnswer>> result = new Dictionary<ScenarioQuestion, List<ScenarioAnswer>>();

        foreach (var question in fp.questionsAndAnswers) {
            List<ScenarioAnswer> answersTemp = new List<ScenarioAnswer>();
            foreach (var answer in question.Value) {
                int trueOrFalse = 0;
                if (answer.Value)
                    trueOrFalse = 1;
                if (answer.Key.GetComponentInChildren<Slider>().gameObject.GetComponent<Text>().text != "") {
                    int answerHiddenId = int.Parse(answer.Key.GetComponentInChildren<Slider>().gameObject.GetComponent<Text>().text);
                    int answerQuestionHiddenId = int.Parse(answer.Key.GetComponentInChildren<Dropdown>().gameObject.GetComponent<Text>().text);
                    answersTemp.Add(new ScenarioAnswer(answerHiddenId, answerQuestionHiddenId, answer.Key.text, trueOrFalse));
                } else
                    answersTemp.Add(new ScenarioAnswer(answer.Key.text, trueOrFalse));
            }
            if (question.Key.transform.parent.GetComponentInChildren<Slider>().gameObject.GetComponent<Text>().text != "") {
                int questionHiddenId = int.Parse(question.Key.transform.parent.GetComponentInChildren<Slider>().gameObject.GetComponent<Text>().text);
                int questionFigureHiddenId = int.Parse(question.Key.transform.parent.GetComponentInChildren<Dropdown>().gameObject.GetComponent<Text>().text);
                result.Add(new ScenarioQuestion(questionHiddenId, questionFigureHiddenId, question.Key.text), answersTemp);
            } else
                result.Add(new ScenarioQuestion(question.Key.text), answersTemp);
        }

        return result;
    }

    /// <summary>
    /// Initializes the list on the left of the create scenario window, containing all figures that can be added to a scenario
    /// </summary>
    public void LoadFigureList() {
        Figure.FigureList.Clear();
        DBConnector.GetFigureData((callback) => {
            foreach (object figure in callback) {
                Figure.FigureList.Add((Figure)figure);
                InstantiateFigureList((Figure)figure);
            }
        });
    }

    /// <summary>
    /// Update the figure list to contain all its info from the database
    /// </summary>
    /// <param name="f"></param>
    private void InstantiateFigureList(Figure f) {
        GameObject instance = Instantiate(FigureListPrefab, FigurePrefabParent);
        foreach (Transform go in instance.transform) {
            if (go.tag == "FigureImage")
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>(f.Image);
            if (go.tag == "FigureName")
                go.GetComponent<Text>().text = f.Name;
            if (go.tag == "FigureHiddenId")
                go.GetComponent<Text>().text = f.Id.ToString();
        }
    }

    /// <summary>
    /// Load an existing scenario
    /// </summary>
    /// <param name="hiddenScenarioId">ID of the scenario to load (found in each scenario list item prefab)</param>
    public void LoadScenarioDetails(Text hiddenScenarioId) {
        Clear();
        hiddenScenarioIdField.text = hiddenScenarioId.text;

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
                scenarioNameInputField.text = temp.Name;
                scenarioStoryTypeDropDown.value = (int)temp.StoryType;

                DBConnector.GetScenarioFigureData((callback) => {
                    foreach (object figure in callback) {
                        Dictionary<ScenarioQuestion, List<ScenarioAnswer>> QuestionsAndAnswers = new Dictionary<ScenarioQuestion, List<ScenarioAnswer>>();
                        PropertyInfo[] info = figure.GetType().GetProperties();
                        ScenarioFigure tempFigure = (ScenarioFigure)figure;

                        DBConnector.GetQuestionData((questionCallback) => {
                            foreach (object question in questionCallback) {
                                ScenarioQuestion tempQuestion = (ScenarioQuestion)question;

                                DBConnector.GetAnswerData((answerCallback) => {
                                    List<ScenarioAnswer> answers = new List<ScenarioAnswer>();
                                    foreach (object answer in answerCallback) {
                                        ScenarioAnswer tempAnswer = (ScenarioAnswer)answer;
                                        answers.Add(tempAnswer);
                                    }
                                    QuestionsAndAnswers.Add(tempQuestion, answers);
                                    InitializeScenarioFigure(tempFigure, QuestionsAndAnswers);
                                }, scenario_question_id: tempQuestion.Id);
                            }
                            figuresWithQnA.Add(tempFigure, QuestionsAndAnswers);
                        }, scenario_figure_id: tempFigure.Id);
                    }
                }, scenario_id: int.Parse(hiddenScenarioId.text));
            } else {
                Debug.LogError("Something went wrong while trying to load scenario details of scenario id: " + hiddenScenarioIdField.text);
            }
        }
    }

    /// <summary>
    /// Initializes a figures and then its questions and answers
    /// </summary>
    /// <param name="figure">Figure to instantiate</param>
    /// <param name="QuestionsAndAnswers">Dictionary of questions and answers to instantiate</param>
    private void InitializeScenarioFigure(ScenarioFigure figure, Dictionary<ScenarioQuestion, List<ScenarioAnswer>> QuestionsAndAnswers) {
        GameObject newPanel = AddFigure(figure.Figure_Id.ToString(), figure);
        FigurePanel fp = newPanel.GetComponent<FigurePanel>();

        foreach (var QnA in QuestionsAndAnswers) {
            GameObject questionTemp = fp.InstantiateQuestion(QnA.Key.Question_Text, QnA.Key.Scenario_Figure_Id, QnA.Key.Id);
            Dictionary<GameObject, ScenarioAnswer> answers = new Dictionary<GameObject, ScenarioAnswer>();
            foreach (var answer in QnA.Value) {
                answers.Add(fp.InstantiateAnswer(questionTemp.transform, answer.Answer_Text, answer.Scenario_Question_Id, answer.Id), answer);
            }
            foreach (var answerGOPair in answers) {
                if (answerGOPair.Value.Correct_Answer == 1)
                    answerGOPair.Key.GetComponentInChildren<Toggle>().isOn = true;
            }
        }
        figurePanelRef.resetPanelsFunc();
    }
}

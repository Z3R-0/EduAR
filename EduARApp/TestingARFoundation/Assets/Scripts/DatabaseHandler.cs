using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseHandler : MonoBehaviour {
    [SerializeField]
    private InputField nameInputField;
    [SerializeField]
    private InputField pinCodeInputField;
    [SerializeField]
    private Text LogInErrorBuffer;

    private static PanelHandler panelHandler;

    private static Dictionary<ScenarioFigure, Dictionary<ScenarioQuestion, List<ScenarioAnswer>>> FiguresWithQuestionsAndAnswers = new Dictionary<ScenarioFigure, Dictionary<ScenarioQuestion, List<ScenarioAnswer>>>();

    private void Awake() {
        panelHandler = GameObject.Find("MainCanvas").GetComponent<PanelHandler>();
    }

    private void Start() {
        ARSessionHandler.DisableAR();
    }

    /// <summary>
    /// Called when the log in button is pressed
    /// </summary>
    public void StudentLogIn() {
        string name = nameInputField.text;
        int pinCode = int.Parse(pinCodeInputField.text);

        DBConnector.GetUserData((callback) => {
            if (callback == null)   // Incorrect login detected
                PrintError("Oeps, dat klopt niet", Color.red);
            else {
                foreach(object student in callback) {
                    // Correct login detected, set current student
                    Student.currentStudent = (Student)student;
                    nameInputField.text = "";
                    pinCodeInputField.text = "";
                    PrintError("", Color.black);
                    panelHandler.SwitchPanel(Panel.Level.ToString());
                }
            }
        }, isTeacher: false, studentName: name, studentPin: pinCode);
    }

    public void OpenScenario(string scenarioId) {
        StartCoroutine(LoadScenarioCoroutine(int.Parse(scenarioId)));
        // Prepare first prefab for placement
        // Initialize UI with questions and answers
    }

    private IEnumerator LoadScenarioCoroutine(int scenarioId) {
        yield return StartCoroutine(DBConnector.GetScenarioFigureIEnumerator((figures) => {
            if (figures != null) {
                Dictionary<ScenarioQuestion, List<ScenarioAnswer>> QuestionsAndAnswers = new Dictionary<ScenarioQuestion, List<ScenarioAnswer>>();

                foreach (object scenarioFigure in figures) {
                    DBConnector.GetQuestionData((questions) => {
                        foreach (object question in questions) {
                            DBConnector.GetAnswerData((answers) => {
                                List<ScenarioAnswer> tempAnswers = new List<ScenarioAnswer>();
                                foreach (object answer in answers) {
                                    tempAnswers.Add((ScenarioAnswer)answer);
                                }
                                Debug.Log("Adding lists");
                                QuestionsAndAnswers.Add((ScenarioQuestion)question, tempAnswers);
                                FiguresWithQuestionsAndAnswers.Add((ScenarioFigure)scenarioFigure, QuestionsAndAnswers);
                            }, scenario_question_id: ((ScenarioQuestion)question).Id);
                        }
                    }, scenario_figure_id: ((ScenarioFigure)scenarioFigure).Id);
                }
            } else {
                Debug.LogError("no figures were found...");
            }
        }, scenario_id: scenarioId));
        yield return new WaitForSeconds(3f);

        Debug.Log(FiguresWithQuestionsAndAnswers.Count);

        ARSessionHandler.EnableAR();
        panelHandler.SwitchPanel(Panel.Play.ToString());

    }

    /// <summary>
    /// Print to the ErrorBuffer text object in the LogIn panel (max 24 chars)
    /// </summary>
    /// <param name="text">The text to print</param>
    /// <param name="color">Color of the text</param>
    private void PrintError(string text, Color color) {
        if (text.Length <= 24) {
            LogInErrorBuffer.color = color;
            LogInErrorBuffer.text = text;
        } else {
            throw new System.ArgumentException("Maximum of 24 characters");
        }
    }
}

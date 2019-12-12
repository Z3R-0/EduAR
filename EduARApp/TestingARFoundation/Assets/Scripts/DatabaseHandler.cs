using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseHandler : MonoBehaviour {
    [SerializeField]
    private InputField nameInputField;
    [SerializeField]
    private InputField pinCodeInputField;
    [SerializeField]
    private Text LogInErrorBuffer;
    [SerializeField]
    private GameObject loadingSymbol;
    

    private bool LoadingScenario = false;

    public static GameObject MainCanvas;
    private static PanelHandler panelHandler;

    public static Dictionary<ScenarioFigure, Dictionary<ScenarioQuestion, List<ScenarioAnswer>>> FiguresWithQuestionsAndAnswers = new Dictionary<ScenarioFigure, Dictionary<ScenarioQuestion, List<ScenarioAnswer>>>();
    public static List<Figure> figureModels = new List<Figure>();

    private string output;
    private string stack;
    private string myLog;
    Vector2 scrollPosition;

    private void Awake() {
        panelHandler = GameObject.Find("MainCanvas").GetComponent<PanelHandler>();
        MainCanvas = GameObject.Find("MainCanvas");
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
                foreach (object student in callback) {
                    // Correct login detected, set current student
                    Student.currentStudent = (Student)student;
                    nameInputField.text = "";
                    pinCodeInputField.text = "";
                    PrintError("", Color.black);
                    panelHandler.SwitchPanel(Panel.Level.ToString());
                    SetLoadingSymbolActive(false);
                }
            }
        }, isTeacher: false, studentName: name, studentPin: pinCode);
    }

    public void SetLoadingSymbolActive(bool IsLoading) {
        loadingSymbol.SetActive(IsLoading);
    }

    public void OpenScenario(string scenarioId) {
        SetLoadingSymbolActive(true);
        StartCoroutine(LoadScenarioCoroutine(int.Parse(scenarioId)));
    }

    private IEnumerator LoadScenarioCoroutine(int scenarioId) {
        LoadingScenario = true;

        yield return StartCoroutine(DBConnector.GetScenarioFigureIEnumerator((figures) => {
            if (figures != null) {
                ScenarioFigure lastFigure;
                ScenarioQuestion lastQuestion;
                ScenarioAnswer lastAnswer;

                foreach (object scenarioFigure in figures) {
                    Dictionary<ScenarioQuestion, List<ScenarioAnswer>> QuestionsAndAnswers = new Dictionary<ScenarioQuestion, List<ScenarioAnswer>>();
                    lastFigure = (ScenarioFigure)scenarioFigure;

                    DBConnector.GetQuestionData((questions) => {
                        foreach (object question in questions) {
                            lastQuestion = (ScenarioQuestion)question;
                            DBConnector.GetAnswerData((answers) => {

                                List<ScenarioAnswer> tempAnswers = new List<ScenarioAnswer>();
                                foreach (object answer in answers) {
                                    lastAnswer = (ScenarioAnswer)answer;
                                    tempAnswers.Add((ScenarioAnswer)answer);

                                    StartCoroutine(LoadingDelayBecauseAsyncIsFun((isDone) => {
                                        if (isDone)
                                            LoadingScenario = false;
                                    }));
                                }
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

        while (LoadingScenario)
            yield return null;

        bool loadingFigures = true;

        foreach (ScenarioFigure scenarioFig in FiguresWithQuestionsAndAnswers.Keys) {
            yield return DBConnector.GetFigureData((figures) => {
                int i = 0;
                foreach (object figure in figures) {
                    figureModels.Add((Figure)figure);
                    ++i;
                    if (i >= figures.Count && scenarioFig.Id == FiguresWithQuestionsAndAnswers.Keys.Last().Id)
                        loadingFigures = false;
                }
            }, id: scenarioFig.Figure_Id);
        }

        while (loadingFigures)
            yield return null;

        SetLoadingSymbolActive(false);
        ARSessionHandler.SetContent(Resources.Load<GameObject>(figureModels[0].Location));
        panelHandler.SwitchPanel(Panel.Play.ToString());
    }

    private IEnumerator LoadingDelayBecauseAsyncIsFun(Action<bool> isDone) {
        isDone(false);
        yield return new WaitForSeconds(1f);
        isDone(true);
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

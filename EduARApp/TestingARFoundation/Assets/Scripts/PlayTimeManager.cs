using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayTimeManager : MonoBehaviour {
    private static int correctlyAnsweredQuestions;
    private static int? totalQuestions = null;

    private ScenarioFigure currentFigure;
    private ScenarioQuestion currentQuestion;
    private List<ScenarioAnswer> currentAnswers = new List<ScenarioAnswer>();

    [SerializeField]
    private GameObject questionPanel;
    [SerializeField]
    private GameObject resultsPopup;

    private UITranslator translator;
    private ARTapToPlaceObject arTap;
    private PanelHandler panelHandler;

    private void Start() {
        translator = gameObject.GetComponent<UITranslator>();
        arTap = gameObject.GetComponent<ARTapToPlaceObject>();
        panelHandler = DatabaseHandler.MainCanvas.GetComponent<PanelHandler>();
    }

    public void InitializePlayUI() {
        if (totalQuestions == null) {
            totalQuestions = GetTotalQuestions();
            currentFigure = DatabaseHandler.FiguresWithQuestionsAndAnswers.Keys.First();
        }

        questionPanel.SetActive(true);
        
        currentQuestion = DatabaseHandler.FiguresWithQuestionsAndAnswers[currentFigure].Keys.First();
        currentAnswers = GetAnswers(currentQuestion);

        ARInteraction.AREnabled = false;

        translator.SetQuestionText(currentQuestion.Question_Text);
        translator.ShowAnswers(currentAnswers.Count, currentAnswers);
    }

    public void Next() {
        if (isCorrectAnswer())
            ++correctlyAnsweredQuestions;

        StartCoroutine(WaitForNextQuestion());
    }

    private IEnumerator WaitForNextQuestion() {
        yield return new WaitForSeconds(3f);
        translator.ClearToggles();
        NextQuestion();
    }

    private void NextQuestion() {
        ScenarioQuestion nextQuestion = null;
        try {
            nextQuestion = DatabaseHandler.FiguresWithQuestionsAndAnswers[currentFigure].Keys.SkipWhile(k => k != currentQuestion).Skip(1).First();
            Debug.Log(currentFigure.Id);
        } catch {

        }
        if (nextQuestion != null) {
            currentQuestion = nextQuestion;

            currentAnswers = GetAnswers(currentQuestion);

            translator.SetQuestionText(currentQuestion.Question_Text);
            translator.ShowAnswers(currentAnswers.Count, currentAnswers);
        } else {
            try {
                currentFigure = DatabaseHandler.FiguresWithQuestionsAndAnswers.Keys.SkipWhile(k => k != currentFigure).Skip(1).First();
            } catch {
                currentFigure = null;
            }

            if (currentFigure == null) {
                StartCoroutine(ShowResults());
            } else {

                panelHandler.OpenPlayPopUp();

                Destroy(GameObject.FindGameObjectWithTag("ARContent"));
                arTap.contentToPlace = GetNextFigureById(currentFigure.Figure_Id);
                arTap.isPlaced = false;
                ARInteraction.AREnabled = true;
                questionPanel.SetActive(false);

                currentQuestion = DatabaseHandler.FiguresWithQuestionsAndAnswers[currentFigure].Keys.First();
            }
        }
    }

    private IEnumerator ShowResults() {
        panelHandler.RunPopUp(PopUp.Results);

        GameObject.FindGameObjectWithTag("CorrectQuestions").GetComponent<Text>().text = correctlyAnsweredQuestions.ToString();
        GameObject.FindGameObjectWithTag("TotalQuestions").GetComponent<Text>().text = totalQuestions.ToString();

        Destroy(GameObject.FindGameObjectWithTag("ARContent"));
        arTap.contentToPlace = null;
        arTap.isPlaced = false;
        ARInteraction.AREnabled = false;
        questionPanel.SetActive(false);

        yield return new WaitForSeconds(3f);
    }

    private GameObject GetNextFigureById(int id) {
        return Resources.Load<GameObject>(DatabaseHandler.figureModels.Where(figure => figure.Id == id).First().Location);
    }

    private bool isCorrectAnswer() {
        foreach (GameObject answer in translator.answers) {
            if (answer.GetComponent<Toggle>().isOn && answer.GetComponent<Text>().text == "true") {
                return true;
            }
        }
        return false;
    }

    private List<ScenarioAnswer> GetAnswers(ScenarioQuestion question) {
        List<ScenarioAnswer> answers = new List<ScenarioAnswer>();

        foreach (var QuestionsAndAnswers in DatabaseHandler.FiguresWithQuestionsAndAnswers[currentFigure]) {
            if (QuestionsAndAnswers.Key == currentQuestion) {
                foreach (var answer in QuestionsAndAnswers.Value) {
                    answers.Add(answer);
                }
            }
        }

        return answers;
    }

    private int GetTotalQuestions() {
        int total = 0;

        foreach (var QuestionAndAnswerPair in DatabaseHandler.FiguresWithQuestionsAndAnswers.Values) {
            total += QuestionAndAnswerPair.Count;
        }

        if (total == 0)
            throw new Exception("No questions and answers were found");
        return total;
    }
}

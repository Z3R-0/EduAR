using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayTimeManager : MonoBehaviour {
    private static int correctlyAnsweredQuestions;
    private static int totalQuestions;

    private ScenarioFigure currentFigure;
    private ScenarioQuestion currentQuestion;
    private List<ScenarioAnswer> currentAnswers = new List<ScenarioAnswer>();

    [SerializeField]
    private GameObject questionPanel;

    private UITranslator translator;

    private void Start() {
        translator = gameObject.GetComponent<UITranslator>();
    }

    /*
    ----TO DO LIST----
    1. Clear all toggles on opening next question
    2. Prepare next figure
    3. Set popup
    */

    public void InitializePlayUI() {
        totalQuestions = GetTotalQuestions();
        questionPanel.SetActive(true);

        currentFigure = DatabaseHandler.FiguresWithQuestionsAndAnswers.Keys.First();
        currentQuestion = DatabaseHandler.FiguresWithQuestionsAndAnswers[currentFigure].Keys.First();
        currentAnswers = GetAnswers(currentQuestion);

        translator.SetQuestionText(currentQuestion.Question_Text);
        translator.ShowAnswers(currentAnswers.Count, currentAnswers);
    }

    public void Next() {
        StartCoroutine(WaitForNextQuestion());
    }

    private IEnumerator WaitForNextQuestion() {
        yield return new WaitForSeconds(3f);

        NextQuestion();
    }

    private void NextQuestion() {
        ScenarioQuestion nextQuestion = DatabaseHandler.FiguresWithQuestionsAndAnswers[currentFigure].Keys.SkipWhile(k => k != currentQuestion).Skip(1).First();

        if (nextQuestion.Question_Text != "") {
            currentQuestion = nextQuestion;
        } else {
            currentFigure = DatabaseHandler.FiguresWithQuestionsAndAnswers.Keys.SkipWhile(k => k != currentFigure).Skip(1).First();
            currentQuestion = DatabaseHandler.FiguresWithQuestionsAndAnswers[currentFigure].Keys.First();
        }

        currentAnswers = GetAnswers(currentQuestion);

        translator.SetQuestionText(currentQuestion.Question_Text);
        translator.ShowAnswers(currentAnswers.Count, currentAnswers);
    }

    private List<ScenarioAnswer> GetAnswers(ScenarioQuestion question) {
        List<ScenarioAnswer> answers = new List<ScenarioAnswer>();

        foreach (var QuestionsAndAnswers in DatabaseHandler.FiguresWithQuestionsAndAnswers[currentFigure]) {
            if (QuestionsAndAnswers.Key == currentQuestion) {
                foreach (var answer in QuestionsAndAnswers.Value) {
                    Debug.Log("Adding: " + answer.Answer_Text);
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

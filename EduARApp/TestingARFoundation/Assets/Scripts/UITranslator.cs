using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITranslator : MonoBehaviour {
    public Text questionText;
    public Text[] answerLabels;
    public Text[] correctAnswerText;

    public GameObject[] answers;

    public void ClearToggles() {
        foreach(GameObject go in answers)
            go.GetComponent<Toggle>().isOn = false;
    }

    public void SetQuestionText(string text) {
        questionText.text = text;
    }

    public void SetAnswerText(string text, int answer) {
        answerLabels[answer].text = text;
    }

    public void SetCorrectAnswer(int answer) {
        for (int i = 0; i < correctAnswerText.Length; i++) {
            if (i == answer)
                correctAnswerText[answer].text = "true";
            else
                correctAnswerText[i].text = "false";
        }
    }

    public void ShowAnswers(int amount) {
        if (amount == 0) {
            foreach (GameObject go in answers)
                go.SetActive(false);
        }

        amount -= 1;

        for (int i = 0; i <= answers.Length; i++) {
            answers[i].SetActive(true);
            if (i > amount)
                answers[i].SetActive(false);
        }
    }

    public void ShowAnswers(int amount, List<ScenarioAnswer> answerList) {
        if (amount == 0) {
            foreach (GameObject go in answers)
                go.SetActive(false);
        }

        amount -= 1;

        for (int i = 0; i < answers.Length; i++) {
            answers[i].SetActive(true);
            if (i > amount)
                answers[i].SetActive(false);

            if (i < answerList.Count) {
                SetAnswerText(answerList[i].Answer_Text, i);

                if (answerList[i].Correct_Answer == 1)
                    SetCorrectAnswer(i);
            }
        }
    }
}

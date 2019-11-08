using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FigurePanel : MonoBehaviour {
    [SerializeField]
    private GameObject figurePropertiesPanelPrefab;
    [SerializeField]
    private GameObject questionsPanelPrefab;
    [SerializeField]
    private GameObject answersPanelPrefab;
    [SerializeField]
    private GameObject figurePropertiesParent;
    [SerializeField]
    private GameObject questionsPrefabParent;

    public Dropdown task { get; set; }
    public Text informationFile { get; set; }
    public Dictionary<InputField, List<InputField>> questionsAndAnswers { get; set; }
    public List<Toggle> correctAnswers { get; set; }


    public GameObject InstantiatePanel() {
        return Instantiate(figurePropertiesPanelPrefab, figurePropertiesParent.transform);
    }

    public void InstantiateQuestion() {
        Instantiate(questionsPanelPrefab, questionsPrefabParent.transform);
    }

    public void InstantiateAnswer(Transform parentQuestion) {
        Instantiate(answersPanelPrefab, parentQuestion);
    }

    public FigurePanel UpdateParameters(FigurePanel panel) {
        List<InputField> answers = new List<InputField>();

        panel.task = GetComponent<Dropdown>();
        foreach (GameObject go in this.transform) {
            if (go.tag == "Information")
                panel.informationFile = go.GetComponent<Text>();
        }

        foreach(InputField question in questionsPrefabParent.transform) {
            foreach(InputField answer in question.gameObject.transform) {
                if (answer.gameObject.tag == "Answer")
                    answers.Add(answer);
            }
            foreach (Toggle correct in question.gameObject.transform) {
                if (correct.isOn)
                    correctAnswers.Add(correct);
            }
            if (question.gameObject.tag == "Question")
                panel.questionsAndAnswers.Add(question, answers);
        }

        return panel;
    }
}

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
    [SerializeField]
    private GameObject resetPanel;

    public Dropdown task { get; set; }
    public Text informationFile { get; set; }
    public Dictionary<InputField, Dictionary<InputField, bool>> questionsAndAnswers = new Dictionary<InputField, Dictionary<InputField, bool>>();


    public GameObject InstantiatePanel() {
        return Instantiate(figurePropertiesPanelPrefab, figurePropertiesParent.transform);
    }

    public void resetQnA() {
        if (this.gameObject.name != "MainCanvas") {
            resetPanel.SetActive(false);
            resetPanel.SetActive(true);
        }
    } 

    public void InstantiateQuestion() {
        GameObject question = Instantiate(questionsPanelPrefab, questionsPrefabParent.transform);
        InstantiateAnswer(question.transform);
        Invoke(nameof(FigurePanel.resetQnA), 0.02f);
    }

    public void InstantiateAnswer(Transform parentQuestion) {
        GameObject answer = Instantiate(answersPanelPrefab, parentQuestion);
        parentQuestion.GetComponent<QuestionListHandler>().AddToggle(answer);
        Invoke(nameof(FigurePanel.resetQnA), 0.02f);
    }

    public FigurePanel UpdateParameters(FigurePanel panel) {
        Dictionary<InputField, bool> answers = new Dictionary<InputField, bool>();

        panel.task = panel.GetComponent<Dropdown>();
        foreach (Transform go in panel.transform) {
            if (go.tag == "Information")
                panel.informationFile = go.GetComponent<Text>();
        }
        
        foreach(Transform question in questionsPrefabParent.transform) {
            foreach(Transform answer in question) {
                if (answer.gameObject.tag == "Answer")
                    answers.Add(answer.GetComponent<InputField>(), answer.GetComponentInChildren<Toggle>().isOn);
            }
            if (question.gameObject.tag == "Question") {
                panel.questionsAndAnswers.Add(question.GetComponentInChildren<InputField>(), answers);
            }
        }

        return panel;
    }
}

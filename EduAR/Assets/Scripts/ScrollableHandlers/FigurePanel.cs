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

    public int hiddenIndex = -1;
    public int hiddenScenarioFigureId;
    public static int currentIndex = -1;

    private float resetDelay = 0.04f;

    public Dropdown task { get; set; }
    public Text informationFile { get; set; }
    public Dictionary<InputField, Dictionary<InputField, bool>> questionsAndAnswers = new Dictionary<InputField, Dictionary<InputField, bool>>();

    public GameObject InstantiatePanel() {
        return Instantiate(figurePropertiesPanelPrefab, figurePropertiesParent.transform);
    }

    public void DestroyPanel(GameObject panel) {
        Debug.Log(panel);
        if(panel.GetComponentInChildren<QuestionListHandler>().gameObject.GetComponentInChildren<Slider>().gameObject.GetComponent<Text>().text == "")
            DBConnector.MainCanvas.GetComponent<UITranslator>().RemoveNewPanel(panel);
        else
            DBConnector.MainCanvas.GetComponent<UITranslator>().RemoveExistingPanel(panel);
        Scenario.CurrentScenarioFigures.RemoveAt(panel.GetComponent<FigurePanel>().hiddenIndex);
        Destroy(panel);
        Invoke(nameof(FigurePanel.resetPanels), resetDelay);
    }

    private void resetPanels() {
        if (this.gameObject.name != "MainCanvas") {
            resetPanel.SetActive(false);
            resetPanel.SetActive(true);
        }
    }

    public void resetPanelsFunc() {
        Invoke(nameof(FigurePanel.resetPanels), resetDelay);
    }

    public void InstantiateQuestionButton() {
        GameObject question = Instantiate(questionsPanelPrefab, questionsPrefabParent.transform);
        InstantiateAnswer(question.transform);
        Invoke(nameof(FigurePanel.resetPanels), resetDelay);
    }

    public GameObject InstantiateQuestion(string text = null, int? questionFigureId = null, int? questionId = null) {
        GameObject question = Instantiate(questionsPanelPrefab, questionsPrefabParent.transform);
        if (text != null)
            question.GetComponentInChildren<InputField>().text = text;
        else
            InstantiateAnswer(question.transform);
        if (questionId != null && questionFigureId != null) {
            question.GetComponentInChildren<Slider>().gameObject.GetComponent<Text>().text = questionId.ToString();
            question.GetComponentInChildren<Dropdown>().gameObject.GetComponent<Text>().text = questionFigureId.ToString();
        }
        Invoke(nameof(FigurePanel.resetPanels), resetDelay);
        return question;
    }

    public GameObject InstantiateAnswer(Transform parentQuestion, string text = null, int? answerQuestionId = null, int? answerId = null) {
        GameObject answer = Instantiate(answersPanelPrefab, parentQuestion);
        if (text != null)
            answer.GetComponent<InputField>().text = text;
        if (answerId != null) {
            answer.GetComponentInChildren<Slider>().gameObject.GetComponent<Text>().text = answerId.ToString();
            answer.GetComponentInChildren<Dropdown>().gameObject.GetComponent<Text>().text = answerQuestionId.ToString();
        }
        parentQuestion.GetComponent<QuestionListHandler>().AddToggle(answer);
        Invoke(nameof(FigurePanel.resetPanels), resetDelay);
        return answer;
    }

    public FigurePanel UpdateParameters(FigurePanel panel) {
        questionsAndAnswers.Clear();
        Dictionary<InputField, bool> answers = new Dictionary<InputField, bool>();

        panel.task = panel.GetComponent<Dropdown>();
        foreach (Transform go in panel.transform) {
            if (go.tag == "Information")
                panel.informationFile = go.GetComponent<Text>();
        }

        foreach (Transform question in questionsPrefabParent.transform) {
            foreach (Transform answer in question) {
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

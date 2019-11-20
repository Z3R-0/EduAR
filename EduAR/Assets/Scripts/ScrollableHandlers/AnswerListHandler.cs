using UnityEngine;

public class AnswerListHandler : MonoBehaviour {
    private FigurePanel figurePanel;
    private QuestionListHandler question;
    private Transform resetPanel;

    private void Start() {
        figurePanel = DBConnector.MainCanvas.GetComponent<FigurePanel>();
        question = transform.parent.GetComponent<QuestionListHandler>();
        resetPanel = transform.parent.transform.parent.transform.parent;
    }

    public void AddORemoveAnswer(bool isAdd) {
        if (isAdd && question.answers < 4) {
            figurePanel.InstantiateAnswer(this.gameObject.transform.parent);
            question.answers++;
        } else if (!isAdd && question.answers > 1) {
            Destroy(this.gameObject);
            question.answers--;
        }
        Invoke(nameof(AnswerListHandler.resetQnA), 0.02f);
    }

    public void resetQnA() {
        resetPanel.gameObject.SetActive(false);
        resetPanel.gameObject.SetActive(true);
    }
}

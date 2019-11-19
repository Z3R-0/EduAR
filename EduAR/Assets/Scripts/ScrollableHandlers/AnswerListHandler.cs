using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerListHandler : MonoBehaviour {
    FigurePanel figurePanel;
    QuestionListHandler question;

    private void Start() {
        figurePanel = DBConnector.MainCanvas.GetComponent<FigurePanel>();
        question = transform.parent.GetComponent<QuestionListHandler>();
    }

    public void AddORemoveAnswer(bool isAdd) {
        if (isAdd && question.answers < 4) {
            figurePanel.InstantiateAnswer(this.gameObject.transform.parent);
            question.answers++;
        } else if (!isAdd && question.answers >= 1) {
            Destroy(this.gameObject);
            question.answers--;
        }
        figurePanel.resetQnA();
    }
}

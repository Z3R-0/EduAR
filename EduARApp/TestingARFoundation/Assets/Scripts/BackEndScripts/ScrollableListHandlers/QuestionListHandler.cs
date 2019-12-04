using UnityEngine;
using UnityEngine.UI;

public class QuestionListHandler : MonoBehaviour {
    public int answers = 1;
    public ToggleGroup correctAnswerToggle;

    public void removeQuestion() {
        Destroy(this.gameObject);
    }

    public void AddToggle(GameObject answer) {
        answer.GetComponentInChildren<Toggle>().group = correctAnswerToggle;
    }
}

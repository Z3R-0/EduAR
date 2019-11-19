using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionListHandler : MonoBehaviour {
    public int answers = 1;

    public void removeQuestion() {
        Destroy(this.gameObject);
    }
}

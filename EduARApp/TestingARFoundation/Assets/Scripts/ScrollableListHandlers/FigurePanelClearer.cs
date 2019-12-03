using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigurePanelClearer : MonoBehaviour {
    private UITranslator translator;

    private void Awake() {
        translator = DBConnector.MainCanvas.GetComponent<UITranslator>();
    }

    private void OnEnable() {
        translator.Clear();
    }
}

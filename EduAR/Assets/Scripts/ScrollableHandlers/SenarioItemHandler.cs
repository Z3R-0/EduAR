using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SenarioItemHandler : MonoBehaviour {
    private PanelHandler panelHandler;
    private UITranslator translator;

    private void Start() {
        panelHandler = DBConnector.MainCanvas.GetComponent<PanelHandler>();
        translator = DBConnector.MainCanvas.GetComponent<UITranslator>();
    }

    public void OpenScenario(GameObject hiddenId) {
        panelHandler.SwitchPanel("CreateScenario");
        translator.LoadScenarioDetails(hiddenId.GetComponent<Text>());
    }

    public void DeleteScenario(GameObject hiddenId) {
        DBConnector.DeleteScenarioFunc((successful) => { }, int.Parse(hiddenId.GetComponent<Text>().text));
        Destroy(this.gameObject);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class FigureListHandler : MonoBehaviour {
    private FigurePanel figurePanel;
    private UITranslator translator;

    private void Start() {
        figurePanel = DBConnector.MainCanvas.GetComponent<FigurePanel>();
        translator = DBConnector.MainCanvas.GetComponent<UITranslator>();
    }

    public void AddFigure(GameObject hiddenID) {
        translator.AddFigure(hiddenID.GetComponent<Text>().text);
    }
}

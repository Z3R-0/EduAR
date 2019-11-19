using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureListHandler : MonoBehaviour {
    private FigurePanel figurePanel;

    private void Start() {
        figurePanel = DBConnector.MainCanvas.GetComponent<FigurePanel>();
    }

    public void AddFigure() {
        figurePanel.InstantiatePanel();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FigurePanel : MonoBehaviour {
    [SerializeField]
    private GameObject figurePropertiesPanel;
    [SerializeField]
    private GameObject questionsAndAnswersPrefab;
    [SerializeField]
    private GameObject figurePropertiesParent;

    public int id { get; set; }
    public Image figureImage { get; set; }
    public Dropdown task { get; set; }
    public Text informationFile { get; set; }
    public List<List<InputField>> questionsAndAnswers { get; set; }
    public List<ToggleGroup> correctAnswers { get; set; }


    public void InstantiatePanel(int id) {
        Instantiate(figurePropertiesPanel, figurePropertiesParent.transform);
    }
}

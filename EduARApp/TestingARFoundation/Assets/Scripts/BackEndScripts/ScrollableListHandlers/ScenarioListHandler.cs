using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioListHandler : MonoBehaviour {
    [SerializeField]
    private GameObject ScenarioListPrefab;
    [SerializeField]
    private Transform ScenarioPrefabParent;

    private DatabaseHandler dbHandler;

    private void Start() {
        dbHandler = DBConnector.MainCanvas.GetComponent<DatabaseHandler>();
    }

    private void OnEnable() {
        InitializeScenarioList();
    }

    public void Clear() {
        foreach (Transform child in ScenarioPrefabParent) {
            Destroy(child.gameObject);
        }
    }

    private void InitializeScenarioList() {
        DBConnector.GetScenarioData((callback) => {
            foreach (var scenario in callback) {
                PropertyInfo[] info = scenario.GetType().GetProperties();
                GameObject ScenarioItem = Instantiate(ScenarioListPrefab, ScenarioPrefabParent.transform);
                Text[] texts = ScenarioItem.GetComponentsInChildren<Text>();
                texts[0].text = info[(int)ScenarioProperties.Name].GetValue(scenario, null).ToString();
                texts[1].text = info[(int)ScenarioProperties.Id].GetValue(scenario, null).ToString();
                Button button = ScenarioItem.GetComponent<Button>();
                button.onClick.AddListener(delegate { dbHandler.OpenScenario(((Scenario)scenario).Id.ToString()); });
            }

            Clear();

            foreach (var scenario in callback) {
                PropertyInfo[] info = scenario.GetType().GetProperties();
                GameObject ScenarioItem = Instantiate(ScenarioListPrefab, ScenarioPrefabParent.transform);
                Text[] texts = ScenarioItem.GetComponentsInChildren<Text>();
                texts[0].text = info[(int)ScenarioProperties.Name].GetValue(scenario, null).ToString();
                texts[1].text = info[(int)ScenarioProperties.Id].GetValue(scenario, null).ToString();
                Button button = ScenarioItem.GetComponent<Button>();
                button.onClick.AddListener(delegate { dbHandler.OpenScenario(((Scenario)scenario).Id.ToString()); });
            }
        }, classID: Student.currentStudent.Class_ID);
    }
}

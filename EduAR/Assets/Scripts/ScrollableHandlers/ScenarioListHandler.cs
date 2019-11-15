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

    private void Start() {
        DBConnector.GetScenarioData((callback) => {
            foreach (var scenario in callback) {
                Scenario.Scenarios.Add(scenario);
                PropertyInfo[] info = scenario.GetType().GetProperties();
                Instantiate(ScenarioListPrefab, ScenarioPrefabParent.transform);
                Text[] texts = ScenarioListPrefab.GetComponentsInChildren<Text>();
                texts[0].text = info[(int)StudentProperties.Name].GetValue(scenario, null).ToString();
                texts[1].text = info[(int)StudentProperties.Id].GetValue(scenario, null).ToString();
            }

            foreach (Transform child in ScenarioPrefabParent) {
                Destroy(child.gameObject);
            }

            foreach (var scenario in callback) {
                Scenario.Scenarios.Add(scenario);
                PropertyInfo[] info = scenario.GetType().GetProperties();
                Instantiate(ScenarioListPrefab, ScenarioPrefabParent.transform);
                Text[] texts = ScenarioListPrefab.GetComponentsInChildren<Text>();
                texts[0].text = info[(int)StudentProperties.Name].GetValue(scenario, null).ToString();
                texts[1].text = info[(int)StudentProperties.Id].GetValue(scenario, null).ToString();
            }
        }, classID: Teacher.currentTeacher.Class_ID);
    }
}

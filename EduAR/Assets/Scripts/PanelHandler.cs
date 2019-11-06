using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Panel {
    StudentList,
    ScenarioList,
    Create,
    None
}

public enum PopUp {
    LogIn,
    Menu
}


public class PanelHandler : MonoBehaviour {

    [SerializeField]
    private List<GameObject> panels = new List<GameObject>();
    [SerializeField]
    private List<GameObject> popups = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void RunPopUp(PopUp popUp) {
        popups[(int)popUp].SetActive(true);
    }

    // Returns true if any pop up is currently running, else returns false
    //public bool IsPopUp() {
    //    for (int i = 0; i <= popups.Count; i++) {
    //        if (popups[i].activeInHierarchy)
    //            return false;
    //    }
    //    return true;
    //}

    public Panel CurrentPanel() {
        for (int i = 0; i <= panels.Count; i++) {
            if (panels[i].gameObject.activeInHierarchy == true) {
                return (Panel)i;
            }
        }
        return Panel.None;
    }

    public void SwitchPanel(int panel) {
        // hide current panel
        panels[(int)CurrentPanel()].SetActive(false);
        // show new panel
        panels[panel].SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Panel {
    Landing,
    LogIn,
    Level,
    Play
}

public enum PopUp {
    Play,
    Results
}

public class PanelHandler : MonoBehaviour {
    // Serialize the panels for use in code
    [SerializeField]
    private List<GameObject> panels = new List<GameObject>();
    // seralize the popups as a list, then transfer contents of list over to the Dictionary (Dictionaries cannot be serialized)
    [SerializeField]
    private List<GameObject> popupsList = new List<GameObject>();
    private Dictionary<PopUp, GameObject> popups = new Dictionary<PopUp, GameObject>();


    // Start is called before the first frame update
    void Start() {
        // initialize popups dictionary
        for (int i = 0; i < popupsList.Count; i++) {
            popups.Add((PopUp)i, popupsList[i]);
        }
        Screen.orientation = ScreenOrientation.Portrait;
        }
    

    /// <summary>
    /// Activates the given pop up
    /// </summary>
    /// <param name="popUp">The pop up to activate</param>
    public void RunPopUp(PopUp popUp) {
        popups[popUp].SetActive(true);
    }

    /// <summary>
    /// Closes the given pop up
    /// </summary>
    /// <param name="popUp">The pop up to close</param>
    public void ClosePopUp(PopUp popUp) {
        popups[popUp].SetActive(false);
    }

    /// <summary>
    /// Used for button OnClick methods to close the play dialog
    /// </summary>
    public void ClosePlayPopUp() {
        popups[PopUp.Play].SetActive(false);
    }

    /// <summary>
    /// Used for button OnClick methods to open the play dialog
    /// </summary>
    public void OpenPlayPopUp() {
        popups[PopUp.Play].SetActive(true);
    }

    /// <summary>
    /// Used for button OnClick methods to close the results screen
    /// </summary>
    public void CloseResultsPopUp() {
        popups[PopUp.Results].SetActive(false);
    }

    /// <summary>
    /// Used for button OnClick methods to open the results screen
    /// </summary>
    public void OpenResultsPopUp() {
        popups[PopUp.Results].SetActive(true);
    }

    public void LogOut() {
        panels[(int)CurrentPanel()].SetActive(false);
        SwitchPanel(Panel.Landing.ToString());
    }

    /// <summary>
    /// Returns a list of all currently active pop ups
    /// </summary>
    public List<PopUp> CurrentPopUps() {
        List<PopUp> result = new List<PopUp>();
        for (int i = 0; i <= popups.Count - 1; i++) {
            if (popups[(PopUp)i].gameObject.activeInHierarchy == true) {
                result.Add((PopUp)i);
            }
        }
        return result;
    }

    /// <summary>
    /// Returns the currently active panel
    /// </summary>
    public Panel CurrentPanel() {
        for (int i = 0; i <= panels.Count - 1; i++) {
            if (panels[i].gameObject.activeInHierarchy == true) {
                return (Panel)i;
            }
        }
        return Panel.Landing;
    }

    /// <summary>
    /// Switches from current panel to the given panel
    /// </summary>
    /// <param name="panel">The panel to switch to</param>
    public void SwitchPanel(string panel) {
        if (panel == "Play") {
            OpenPlayPopUp();
            CloseResultsPopUp();
            Screen.orientation = ScreenOrientation.Landscape;
        } else {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        // hide current panel
        panels[(int)CurrentPanel()].SetActive(false);
        // show new panel
        panels[(int)StringEnumDecoder(panel)].SetActive(true);
    }

    /// <summary>
    /// Converts string to Panel enum
    /// </summary>
    private Panel StringEnumDecoder(string input) {
        for (int i = 0; i < panels.Count; i++) {
            if (input == ((Panel)i).ToString()) {
                return (Panel)i;
            }
        }
        return Panel.Landing;
    }
}

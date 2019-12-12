using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSessionHandler : MonoBehaviour {
    private static GameObject ARSessionOrigin;
    private static GameObject ARSession;
    private static GameObject ScriptManager;

    private void Awake() {
        ARSessionOrigin = GameObject.Find("AR Session Origin");
        ARSession = GameObject.Find("AR Session");
        ScriptManager = GameObject.Find("ARScriptManager");
    }

    //public static void EnableAR() {
    //    ARSessionOrigin.SetActive(true);
    //    ARSession.SetActive(true);
    //    ScriptManager.GetComponent<ARTapToPlaceObject>().enabled = true;
    //    ScriptManager.GetComponent<ARInteraction>().enabled = true;
    //}

    //public static void DisableAR() {
    //    ARSessionOrigin.SetActive(false);
    //    ARSession.SetActive(false);
    //    ScriptManager.GetComponent<ARTapToPlaceObject>().enabled = false;
    //    ScriptManager.GetComponent<ARInteraction>().enabled = false;
    //}

    public static void SetContent(GameObject content) {
        ScriptManager.GetComponent<ARTapToPlaceObject>().contentToPlace = content;
    }
}

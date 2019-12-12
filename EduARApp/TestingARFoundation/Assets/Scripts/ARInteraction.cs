using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARInteraction : MonoBehaviour {
    private ARSessionOrigin arOrigin;
    private ARRaycastManager raycaster;
    private PlayTimeManager manager;

    public static bool AREnabled = true;

    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycaster = arOrigin.GetComponent<ARRaycastManager>();
        manager = gameObject.GetComponent<PlayTimeManager>();
    }

    // Update is called once per frame
    void Update() {
        if (AREnabled) {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit raycastHit;
                if (Physics.Raycast(raycast, out raycastHit, maxDistance: 1000f)) {
                    Debug.Log("Sending Raycast, hit: " + raycastHit.collider.name);
                    if (raycastHit.collider.CompareTag("ARContent")) {
                        Debug.Log("Tapped Object");
                        manager.InitializePlayUI();
                    }
                }
            }
        }
    }

    public void DisableAR() {
        AREnabled = false;
    }

    public void EnableAR() {
        AREnabled = true;
    }
}

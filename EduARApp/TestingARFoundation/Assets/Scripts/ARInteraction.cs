using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARInteraction : MonoBehaviour {
    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private Text feedbackText;
    [SerializeField]
    private Toggle[] toggles;
    private float text_time;
 
    private ARSessionOrigin arOrigin;
    private ARRaycastManager raycaster;

    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycaster = arOrigin.GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;

            if (Physics.Raycast(raycast, out raycastHit, maxDistance: 1000f)) {
                if (raycastHit.collider.CompareTag("TestContent")) {
                    ui.SetActive(true);
                }
            }
        }
        
    }
}

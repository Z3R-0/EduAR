using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.UI;

public class ARTapToPlaceObject : MonoBehaviour {
    public GameObject placementIndicator;
    public GameObject contentToPlace;

    //public Text raycastText;
    //public Text poseText;
    //public Text IndicatorText;
    //public Text IsValidText;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager raycaster;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool isPlaced = false;

    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycaster = arOrigin.GetComponent<ARRaycastManager>();
    }
    
    void Update() {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if(placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !isPlaced) {
            PlaceObject();
        } 
    }

    private void PlaceObject() {
        Instantiate(contentToPlace, placementPose.position, placementPose.rotation);
        isPlaced = true;
    }

    private void UpdatePlacementIndicator() {
        if (placementPoseIsValid) {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            //IndicatorText.text = "is active";
        } else {
            placementIndicator.SetActive(false);
            //IndicatorText.text = "not active";
        }
    }

    private void UpdatePlacementPose() {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycaster.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        //IsValidText.text = placementPoseIsValid.ToString();

        if (placementPoseIsValid) {
            //raycastText.text = "Sending raycast, hit: " + hits[0].hitType + "\n Hits Count: " + hits.Count;
            placementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}

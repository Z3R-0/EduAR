﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.UI;

public class ARTapToPlaceObject : MonoBehaviour {
    public GameObject placementIndicator;
    
    public GameObject contentToPlace;

    public GameObject popup;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager raycaster;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    public bool isPlaced = false;

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

    public void PlaceObject() {
        GameObject content = Instantiate(contentToPlace, placementPose.position, Quaternion.LookRotation(new Vector3(placementPose.rotation.x, placementPose.rotation.y, placementPose.rotation.z - 85  )));
        isPlaced = true;
    }

    private void UpdatePlacementIndicator() {
        if (placementPoseIsValid && !(popup.activeInHierarchy)) {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        } else {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose() { 
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycaster.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid) {
            placementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}

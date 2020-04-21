using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour {
    private ARSessionOrigin arSessionOrigin;
    private ARRaycastManager arRaycastManager;
    private bool placementPoseIsValid = false;
    
    private Pose placementPose;
    void Start() {
        arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = arSessionOrigin.GetComponent<ARRaycastManager>();
    }

    void Update() {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    private void UpdatePlacementIndicator() {
    }

    private void UpdatePlacementPose() {
        // Cameraの中心座標を取得
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        
        // Cameraの中心座標方向に直線を引き、その際に床を認識したら認識したそれら全てをhitsに追加する
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid) {
            placementPose = hits[0].pose;
        }
    }
}
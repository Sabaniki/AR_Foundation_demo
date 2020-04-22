using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour {
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    
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

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            PlaceObject();
        }
    }

    private void PlaceObject() {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
    }

    private void UpdatePlacementIndicator() {
        if (placementPoseIsValid) {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else placementIndicator.SetActive(false);
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
            
            // 現在のカメラオブジェクトを基準にしたZ方向(0, 0, 1)の単位ベクトル    ※UnityではZ方向が前
            var cameraForward = Camera.current.transform.forward;
            
            // normalized: 最大値が1.0のベクトルが作成される    例) (2.0, 4.0, 1.0) → (0.5, 1.0, 0.25)
            var cameraBearing = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;
            
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
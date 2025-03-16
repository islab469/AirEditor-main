using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
[RequireComponent(typeof(ARRaycastManager))]
public class AR_SpawnObjectOnPlane : MonoBehaviour
{
    // Start is called before the first frame update


    private ARRaycastManager raycastManager;
    private GameObject spawnObject;

    [SerializeField]
    private GameObject placablePrefab;

    private bool isClicking = false;

    private static List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    void Start(){
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update(){
        if (!tryGetTouchPosition(out Vector2 touchPosition)) {
            return;
        }

        if (raycastManager.Raycast(touchPosition, raycastHits,TrackableType.PlaneWithinPolygon)){
            Pose hitPose = raycastHits[0].pose;
            if (spawnObject == null)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                Vector3 cameraPosition = Camera.main.transform.position;
                float minDistance = 10.0f;
                Vector3 adjustedPosition = cameraPosition + cameraForward * minDistance;

                spawnObject = Instantiate(placablePrefab, adjustedPosition, hitPose.rotation);

            }
            else {
                spawnObject.transform.position = hitPose.position;
                spawnObject.transform.rotation = hitPose.rotation;


            }
        }
    }

    bool tryGetTouchPosition(out Vector2 touchPosition) {
        if (Input.touchCount > 0)
        {
            Debug.Log("Input.touchCount :" + Input.touchCount.ToString());
            touchPosition = Input.GetTouch(0).position;
            return true;



        }
        else {
            if (Input.GetMouseButtonDown(0))
            {
                isClicking = true;
            }
            else if (Input.GetMouseButtonUp(0)) {
                isClicking = false;
                
            }
        }
        if (isClicking) {
            touchPosition = Input.mousePosition;
            return true;
        }
        touchPosition = default;
        return false;
    }
}

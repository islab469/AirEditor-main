using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;


public class PlaceObject : MonoBehaviour
{

    [SerializeField]
    private  GameObject prefab;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    // Start is called before the first frame update
    private int prefadcount = 0;
    
    private bool objectPlaced = false;

    private void Awake()
    { 
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }
    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        planeManager.planesChanged += OnPlanesChange;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        planeManager.planesChanged -= OnPlanesChange;
    }

    private  void FingerDown (EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;

        if (raycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in hits) 
            {
                Pose hitPose = hit.pose;
                GameObject obj = Instantiate(prefab, hitPose.position, hitPose.rotation);
            }
        }
    }
    private void OnPlanesChange(ARPlanesChangedEventArgs eventArgs)
    {
        print("123");
        foreach(ARPlane aRPlane in eventArgs.added)
        {
            if (!objectPlaced && IsPlaneLargeEnough(aRPlane))
            {
                print("模型建立前");
                if (prefadcount == 0)
                {
                    
                    PlaceObjectOnPlane(aRPlane);
                    objectPlaced = true;
                    prefadcount++;
                }
            }
            
            
        }
    }

    private void PlaceObjectOnPlane(ARPlane plane)
    {
        // 获取平面的中心位置和旋转
        Vector3 planePosition = plane.transform.position;
        Quaternion planeRotation = plane.transform.rotation;

        // 获取平面的法线方向
        Vector3 planeNormal = plane.transform.up; // 法线是平面的上方向

        // 设置一个人工的偏移量，确保 prefab 离平面有适当的距离
        float offset = 0.2f;  // 调整这个值来改变物体与平面之间的距离（例如 20cm）

        // 计算调整后的位置
        Vector3 adjustedPosition = planePosition + planeNormal * offset;

        // 在该位置生成 prefab
        Instantiate(prefab, adjustedPosition, planeRotation);
    }

    private bool IsPlaneLargeEnough(ARPlane plane)
    {
        Renderer prefabRenderer = prefab.GetComponent<Renderer>();
        Vector3 prefabSize = prefabRenderer.bounds.size;
        Vector2 planeSize = plane.size;
        float planeArea = planeSize.x * planeSize.y;
        Debug.Log("Prefab size: " + prefabSize);
        Debug.Log("Plane area: " + planeArea);
        return planeArea >= prefabSize.x * prefabSize.z;
    }
}

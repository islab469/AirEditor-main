using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class ARObjectInteraction : MonoBehaviour
{
    private Transform objectTransform;
    private float initialDistance;
    private Vector3 initialScale;
    private Vector2 initialTouch0Position;
    private Vector2 initialTouch1Position;
    private Vector3 initialObjectPosition;
    private float rotationSpeed = 100f;
    private float scaleSmoothness = 10f;

    void Awake()
    {
        EnhancedTouchSupport.Enable();

        // 檢查 AR Session 狀態，這裡會提示 ARSession 是否正在啟動
        Debug.Log("🟢 AR Session State (Awake): " + ARSession.state);

        if (ARSession.state < ARSessionState.SessionTracking)
        {
            Debug.LogWarning("⚠️ AR Session is not fully started yet. Waiting...");
        }
    }

    void Start()
    {
        objectTransform = GameObject.Find("drone_costum")?.transform;

        if (objectTransform == null)
        {
            Debug.LogError("🚨 ARObjectInteraction is not attached to any object!");
        }

        Debug.Log("🟢 AR Session State (Start): " + ARSession.state);
        if (ARSession.state < ARSessionState.SessionTracking)
        {
            Debug.LogWarning("⚠️ AR Session has not fully started. Please check the AR setup!");
        }

        if (gameObject.GetComponent<ARAnchor>() == null)
        {
            Debug.LogWarning("⚠️ No AR Anchor found on this object. Consider adding one!");
        }
    }

    void Update()
    {
        // 檢查 AR Session 是否已經啟動
        if (ARSession.state < ARSessionState.SessionTracking)
        {
            Debug.LogWarning("⚠️ AR Session is not fully started yet. Waiting...");
            return;  // 等待 AR Session 完全啟動，跳過後續處理
        }

        // AR Session 已啟動，可以進行交互操作
        if (objectTransform == null)
            return;

        var activeTouches = Touch.activeTouches;

        if (activeTouches.Count == 1)
        {
            var touch = activeTouches[0];
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 rotation = objectTransform.localEulerAngles;
                float currentX = NormalizeAngle(rotation.x);
                float newX = currentX + touch.delta.y * rotationSpeed * Time.deltaTime;
                newX = Mathf.Clamp(newX, -80f, 80f);  // 限制上下旋轉
                rotation.x = newX;
                rotation.y -= touch.delta.x * rotationSpeed * Time.deltaTime;
                objectTransform.localEulerAngles = rotation;

                Debug.Log($"📌 Rotate: ΔX={touch.delta.x}, ΔY={touch.delta.y}");
            }
        }
        else if (activeTouches.Count == 2)
        {
            var touch0 = activeTouches[0];
            var touch1 = activeTouches[1];
            float currentDistance = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                initialDistance = currentDistance;
                initialScale = objectTransform.localScale;
                initialTouch0Position = touch0.screenPosition;
                initialTouch1Position = touch1.screenPosition;
                initialObjectPosition = objectTransform.position;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                if (initialDistance > 1e-5f)
                {
                    float scaleFactor = Mathf.Clamp(currentDistance / initialDistance, 0.5f, 2f);
                    objectTransform.localScale = Vector3.Lerp(objectTransform.localScale, initialScale * scaleFactor, Time.deltaTime * scaleSmoothness);
                }

                Vector2 currentTouch0Position = touch0.screenPosition;
                Vector2 currentTouch1Position = touch1.screenPosition;
                Vector2 touchDelta = (currentTouch0Position + currentTouch1Position) / 2 - (initialTouch0Position + initialTouch1Position) / 2;

                Vector3 move = new Vector3(touchDelta.x * 0.01f, 0, touchDelta.y * 0.01f);
                objectTransform.position = initialObjectPosition + move;
            }
        }
    }

    // 將角度轉換為 -180~180
    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
}

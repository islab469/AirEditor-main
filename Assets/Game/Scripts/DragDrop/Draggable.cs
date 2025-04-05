using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IPointerUpHandler,IDragHandler
{
    RectTransform rect;
    CanvasGroup canvasGroup;
    Canvas canvas;

    RectTransform lastRectTansform;
    
    private void Awake() {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = getCurnCanvas();
    }

    private Canvas getCurnCanvas() {
        Transform parent = transform.parent;
        while (parent.gameObject.GetComponent<Canvas>() == null)
        {
            parent = parent.parent;
        }
        canvas = parent.GetComponent<Canvas>();
        return canvas;

    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        //move it
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;



    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        canvasGroup.blocksRaycasts = true;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerDown");
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("PointerUp");
    }

    public void updateLastRectTansform()
    {
        lastRectTansform = GetComponent<RectTransform>();
    }

    public RectTransform getLastRectTansform()
    {
        return lastRectTansform;
    }
    public Vector2 getPos()
    {
        return GetComponent<RectTransform>().anchoredPosition;
    }
}

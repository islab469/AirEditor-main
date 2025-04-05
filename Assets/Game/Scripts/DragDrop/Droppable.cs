using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Droppable : MonoBehaviour,IDropHandler,IPointerEnterHandler
{

    void IDropHandler.OnDrop(PointerEventData eventData){
        if (eventData.pointerDrag == null) { return; }
        Debug.Log("OnDrop!");
        doOnDrop(eventData);
    }

    public virtual void doOnDrop(PointerEventData eventData) { 
    
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("PointerEnter! eventData.position:" + UIManager.ScreenPosToCanvaPos(transform,eventData.position));
    }
}

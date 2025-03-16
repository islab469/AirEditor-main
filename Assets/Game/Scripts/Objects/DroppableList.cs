using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class DroppableList : Droppable{

    //Droppable , means bind in the Object that being drop
    VerticalLayoutGroup verticalLayoutGroup;
    RectTransform dropRect;
    List<Vector2> targetChildPos = new List<Vector2>();

    private void Awake()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        dropRect = GetComponent<RectTransform>();   
    }

    private void Update(){
        lerpChildPositions(Time.deltaTime,10.0f);
        //lerpChildPositions();
    }
    public override void doOnDrop(PointerEventData eventData)
    {
        GameObject dragObject = eventData.pointerDrag;
        
        RectTransform dragRect = dragObject.GetComponent<RectTransform>();
        Vector2 dragPos = dragRect.anchoredPosition;
        


        int pos = 0;
        dragPos = UIManager.getRealUIPosition(dragRect.transform);
        //Debug.Log(transform.childCount);

        // only 1 object
        if (transform.childCount == 1) {
            Vector2 curnPos;
            curnPos = UIManager.getRealUIPosition(transform.GetChild(0));

            if (curnPos.y < dragPos.y)
            { //on very top 
                pos = 0;
                //Debug.Log("It's top !" + 0.ToString());


            }
          
            else if (curnPos.y >= dragPos.y)
            {
                pos = 1;
                //Debug.Log("It's last !" +  1.ToString());
            }

        }
        // 2 object
        else if (transform.childCount >= 2)
        {
            Vector2 curnPos;
            Vector2 nextPos;
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                curnPos = UIManager.getRealUIPosition(transform.GetChild(i));
                nextPos = UIManager.getRealUIPosition(transform.GetChild(i + 1));
           
                if (curnPos.y < dragPos.y && i == 0)
                { //on very top 
                    pos = 0;
                    //Debug.Log("It's top !" + 0.ToString());
                    break;


                }
                else if (curnPos.y > dragPos.y && nextPos.y < dragPos.y)
                { //mid

                    pos = i + 1;
                    //Debug.Log("It's mid !" + i.ToString());
                    break;


                }
                else if (nextPos.y > dragPos.y && i + 1 == transform.childCount - 1)
                {
                    pos = i + 2;
                    //Debug.Log("It's last !" + i.ToString());
                    break;
                }

            }
        }

        //The pos that the object is going in, is ensured
        /*
        1.drag object into the list , save pos
        2.drag object out the list,now,the UI is in their init pos
        3.verticalLayoutGroup.enabled = false , can change elements dynamically to the saved pos
        4.drag object back to the list
        5.objects lerp to position
        6.enabled = true
        

        */
        //

        verticalLayoutGroup.enabled = false;

        dragObject.transform.SetParent(transform,true);
        dragObject.transform.SetSiblingIndex(pos);
        forceUpdateLayout();
        dragRect.anchorMax = new Vector2(0.0f, 1.0f);
        dragRect.anchorMin = new Vector2(0.0f, 1.0f);
        forceUpdateLayout();
        Vector2 dragInitPos = dragRect.anchoredPosition;
        //Vector2 dragInitPos = UIManager.getRelativeUIPosition(transform, dragObject.transform);
        Debug.Log("dragObject pos 1 : " + dragRect.name + " " + dragRect.anchoredPosition);

        //1.drag object into the list , save pos

        verticalLayoutGroup.enabled = true;
        
        forceUpdateLayout();

        
        saveChildPositions();
        //2.drag object out the list, now, the UI is in their init pos

        dragObject.transform.SetParent(UIManager.getCurnCanvas(transform).transform,true);
        Debug.Log("dragObject pos 2 : " + dragRect.name + " " + dragRect.anchoredPosition);
        forceUpdateLayout();
        //3.verticalLayoutGroup.enabled = false , can change elements dynamically to the saved pos
        verticalLayoutGroup.enabled = false;
        //4.drag object back to the list
        dragObject.transform.SetParent(transform, true);
        dragObject.transform.SetSiblingIndex(pos);
        forceUpdateLayout();
        Debug.Log("dragObject pos 3 : " + dragRect.name + " " + dragRect.anchoredPosition);
        
        dragRect.anchoredPosition = dragInitPos;
        dragRect.anchorMax = new Vector2(0.0f, 1.0f);
        dragRect.anchorMin = new Vector2(0.0f, 1.0f);
        Debug.Log("dragObject pos dragInitPos : " + dragRect.name + " " + dragInitPos);
        forceUpdateLayout();
        //5.lerp object back to the list

        //6.enabled = true
        //verticalLayoutGroup.enabled = true;


        // Debug.Log("verticalLayoutGroup.enabled : " + verticalLayoutGroup.enabled); 


    }

    private void saveChildPositions() {
        targetChildPos.Clear();
        for (int i = 0; i < transform.childCount; i++){
            Draggable draggable = transform.GetChild(i).GetComponent<Draggable>();
            targetChildPos.Add(draggable.getPos());
            //Debug.Log(UIManager.getRealUIPosition(draggable.transform));
        }


    }


    private void lerpChildPositions(float delta,float speed)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform from = transform.GetChild(i).GetComponent<RectTransform>();
            Vector2 target = targetChildPos[i];
            from.anchoredPosition = 
                Vector2.Lerp(from.anchoredPosition,
                target,
                speed * delta

                )
                ;

            //Debug.Log(from.anchoredPosition + " : " + target);
        }
    }
    private void forceUpdateLayout() {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }


}

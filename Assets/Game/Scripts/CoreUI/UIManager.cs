using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UIManager
{
    public static Canvas getCurnCanvas(Transform transform)
    {
        Transform parent = transform.parent;
        while (parent.gameObject.GetComponent<Canvas>() == null)
        {
            parent = parent.parent;
        }
        Canvas canvas = parent.GetComponent<Canvas>();
        return canvas;
    }

    public static Vector2 getRealUIPosition(Transform transform)
    {
        return RectTransformUtility.CalculateRelativeRectTransformBounds(UIManager.getCurnCanvas(transform).transform, transform).center;

    }

    public static Vector2 getRelativeUIPosition(Transform root, Transform child)
    {
        /*
        Vector3 worldPosition = child.position;

        // 將世界位置轉換為相對於 reference 的本地位置
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.GetComponent<RectTransform>(),
            RectTransformUtility.WorldToScreenPoint(null, worldPosition),
            null,
            out localPosition
        );

        return localPosition;
        
        */
        return RectTransformUtility.CalculateRelativeRectTransformBounds(root, child).center;

    }

    public static Vector2 ScreenPosToCanvaPos(Transform transform,Vector2 ScreenPoint) {
        Vector2 local = Vector2.zero;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(getCurnCanvas(transform).GetComponent<RectTransform>(), ScreenPoint, null,out local);
        return local; 


    }
}

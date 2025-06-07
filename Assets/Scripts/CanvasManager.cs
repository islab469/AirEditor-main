using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
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
}

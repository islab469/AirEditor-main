using UnityEngine;

public class AnswerButton : MonoBehaviour
{
    private System.Action onClick;

    public void Setup(System.Action clickAction)
    {
        onClick = clickAction;
    }

    void OnMouseDown()
    {
        if (onClick != null)
        {
            onClick.Invoke();
        }
    }
}


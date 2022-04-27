using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDrag : MonoBehaviour, IDragHandler//, IPointerDownHandler
{
    [SerializeField] private RectTransform windowToDrag;
    [SerializeField] private Canvas canvas;
    public void OnDrag(PointerEventData eventData){
        //throw new System.NotImplementedException();
        //Debug.Log("Dragging Window");
        if (CursorInScreen())
            windowToDrag.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    private bool CursorInScreen() {
        Vector3 cursorViewPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        return !(cursorViewPoint.x < 0 || cursorViewPoint.x > 1 || cursorViewPoint.y < 0 || cursorViewPoint.y > 1);
    }

    //maybe not needed, but if multiple windows of same priority:
    /*public void OnPointerDown(PointerEventData eventData){
        windowToDrag.SetAsLastSibling();
    }*/
}

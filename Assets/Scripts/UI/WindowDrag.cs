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
        windowToDrag.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    //maybe not needed, but if multiple windows of same priority:
    /*public void OnPointerDown(PointerEventData eventData){
        windowToDrag.SetAsLastSibling();
    }*/
}

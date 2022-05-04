//All code developed by Team 11
//This is a testing script to experiment with detecting mouse hover over a UI element

using UnityEngine;
using UnityEngine.EventSystems;

public class TestingOnHover : EventTrigger
{
    [SerializeField] private GameObject testButton;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("On pointer enter called");
      
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("On pointer exit called");
     
    }
}

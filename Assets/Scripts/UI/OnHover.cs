//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHover : EventTrigger
{
    //Prefab of UI label on hover
    [SerializeField] private GameObject labelPrefab;
    // Start is called before the first frame update
   public override void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverLabel newLabel = Instantiate(labelPrefab, this.transform).GetComponent<OnHoverLabel>();
        newLabel.InitiateLabel(this.transform.parent.gameObject);
        Debug.Log("On pointer enter called");
      
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("On pointer exit called");
     
    }
}

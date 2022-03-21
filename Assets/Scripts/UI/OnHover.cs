//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour
{
    //Prefab of UI label on hover
    [SerializeField] private GameObject labelPrefab;
    
   public void OnHoverMouse(){
        Debug.Log("On pointer enter called");
        OnHoverLabel newLabel = Instantiate(labelPrefab, Input.mousePosition, Quaternion.identity).GetComponent<OnHoverLabel>();
        newLabel.CreateLabel();
        
   }

    public void OnHoverExit()
    {
        Debug.Log("On pointer exit called");
        
    }
}

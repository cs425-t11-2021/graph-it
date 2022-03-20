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
        //OnHoverLabel newLabel = Instantiate(labelPrefab).GetComponent<OnHoverLabel>();
        //newLabel.CreateLabel(this.transform.parent.gameObject);
        Debug.Log("On pointer enter called");
   }

    public void OnHoverExit()
    {
        Debug.Log("On pointer exit called");
     
    }
}

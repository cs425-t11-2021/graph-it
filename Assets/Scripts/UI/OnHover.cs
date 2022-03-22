//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour
{
    //Prefab of UI label on hover
    [SerializeField] private GameObject labelPrefab;
    OnHoverLabel newLabel;
    
   public void OnHoverMouse(){
        Debug.Log("On pointer enter called");
        newLabel = Instantiate(labelPrefab, UIManager.Singleton.mainCanvas.transform).GetComponent<OnHoverLabel>();
        newLabel.CreateLabel();
        newLabel.transform.SetAsLastSibling();
        newLabel.transform.position = this.transform.position;
   }

    public void OnHoverExit()
    {
        Debug.Log("On pointer exit called");
        newLabel.OnNotHover();
    }
}

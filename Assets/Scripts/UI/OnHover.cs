//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour
{
    //Prefab of UI label on hover
    [SerializeField] private GameObject labelPrefab;
    [SerializeField] private string labelDescription;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 size;
    private OnHoverLabel newLabel;
    
   public void OnHoverMouse(){
        this.newLabel = Instantiate(labelPrefab, UIManager.Singleton.mainCanvas.transform).GetComponent<OnHoverLabel>();
        this.newLabel.CreateLabel(this.labelDescription, this.size);
        this.newLabel.transform.SetAsLastSibling();
        this.newLabel.transform.position = this.transform.position + this.offset;
   }

    public void OnHoverExit()
    {
        this.newLabel.OnNotHover();
    }
}

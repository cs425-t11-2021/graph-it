//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnHoverLabel : MonoBehaviour
{
    //Prefab of UI label on hover
    [SerializeField] private GameObject labelPrefab;
    private TMP_Text label;
    //the text component of the label when the user hovers over the UI element

    private void Awake()
    {
        //getting component references
        this.label = GetComponent<TMP_Text>();
    }

    public void CreateLabel(GameObject parentObject){
        this.Instantiate(labelPrefab).GetComponent<OnHoverLabel>();
        this.label.SetText(parentObject.GetComponent<TMP_Text>().text);
    }

    public void OnNotHover(){
        Destroy(this.gameObject);
    }
}

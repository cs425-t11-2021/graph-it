//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnHoverLabel : MonoBehaviour
{
    
    private TMP_Text label;
    //the text component of the label when the user hovers over the UI element
    private RectTransform rect;

    private void Awake()
    {
        //getting component references
        this.label = GetComponentInChildren<TMP_Text>();
        this.rect = GetComponent<RectTransform>();
    }

    public void CreateLabel(string labelText, Vector2 labelSize){
        this.label.text = labelText;
        this.rect.sizeDelta = labelSize;
    }
}

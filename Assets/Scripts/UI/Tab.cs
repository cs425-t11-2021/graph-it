using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tab : MonoBehaviour
{
    public GraphInstance GraphInstance { get; set; }

    public string Label
    {
        set
        {
            this.tabLabel.text = value;
        }
    }
    private TMP_Text tabLabel;

    private void Awake()
    {
        this.tabLabel = GetComponentInChildren<TMP_Text>();
    }

    public void SwitchToInstance()
    {
        Controller.Singleton.ChangeActiveInstance(this.GraphInstance);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tab : MonoBehaviour
{
    // Graph instance associated with this tab
    private GraphInstance graphInstance;
    // Toggle button associated with the tab, for visual purpose
    private ToggleButton toggle;
    // TMP text component for the tab label
    private TMP_Text tabLabel;

    private void Awake()
    {
        // Get component references
        this.tabLabel = GetComponentInChildren<TMP_Text>();
        this.toggle = GetComponent<ToggleButton>();
        
        // When the active instance changes, highlight the tab is the tab is associated with the new active instance
        Controller.Singleton.OnInstanceChanged += (instance) =>
        {
            this.toggle.UpdateStatus(instance == this.graphInstance);
        };
    }

    public void Initiate(string name, GraphInstance associatedInstance)
    {
        this.tabLabel.text = name;
        this.graphInstance = associatedInstance;
    }
    
    // Function called when the tab is pressed
    public void SwitchToInstance()
    {
        // Chance the current graph instance to the one associated with the tab
        Controller.Singleton.ChangeActiveInstance(this.graphInstance, true);
    }
}

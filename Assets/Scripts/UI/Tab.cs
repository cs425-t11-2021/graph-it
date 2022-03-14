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
        Controller.Singleton.ChangeActiveInstance(this.graphInstance);
        
        // Disable the toggle button of all other tabs except the one selected
        foreach (Tab tab in TabBar.Singleton.TabsInScene)
        {
            if (tab == this)
            {
                tab.toggle.Checked = true;
            }
            else
            {
                tab.toggle.Checked = false;
            }
        }
    }
}

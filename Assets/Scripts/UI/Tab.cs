using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Tab : MonoBehaviour //, IPointerClickHandler --might be useful later on for right click menu options
{
    // Graph instance associated with this tab
    private GraphInstance graphInstance;
    // Toggle button associated with the tab, for visual purpose
    private ToggleButton toggle;
    // TMP text component for the tab label
    private TMP_InputField tabLabel;
    
    // Name of the tab
    public string TabName
    {
        get => this.tabLabel.text;
        set => this.tabLabel.text = value;
    }

    private void Awake()
    {
        // Get component references
        this.tabLabel = GetComponentInChildren<TMP_InputField>(true);
        this.toggle = GetComponent<ToggleButton>();
        
        // When the active instance changes, highlight the tab is the tab is associated with the new active instance
        Controller.Singleton.OnInstanceChanged += OnInstanceChanged;
    }

    private void OnInstanceChanged(GraphInstance instance)
    {
        this.toggle.UpdateStatus(instance == this.graphInstance);

        if (instance == this.graphInstance) TabBar.Singleton.ActiveTab = this;
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

    public void Close()
    {
        Controller.Singleton.RemoveGraphInstance(this.graphInstance);
        Destroy(this.gameObject); 
    }

    private void OnDestroy()
    {
        // Unsubscribe to event when the object is destroyed
        Controller.Singleton.OnInstanceChanged -= OnInstanceChanged;
    }

    public void Rename(string newName)
    {
        tabLabel.text = newName;
    }

    // TEMPROARY: CLOSE A TAB WHEN RIGHT CLICKING ON IT
    /*public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Close();
        }
    }*/
}

//All code developed by Team 11

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Base class derived by all menu bar buttons
public abstract class MenuButton : MonoBehaviour
{
    // Refernce to the toggle button component
    private ToggleButton toggleButton;

    // Property for whether or not the dropdown associated with this button is active
    public bool DropdownActive {
        // Gets or sets the activeness of the gameObject of the dropdown (second child of button)
        get {
            return this.transform.GetChild(1).gameObject.activeInHierarchy;
        }
        set {
            if (value) {
                Logger.Log("Opening dropdown.", this, LogType.DEBUG);
                
            }
            // Update the toggle button status and enable/disable the dropdown accordingly
            this.toggleButton.UpdateStatus(value);
            this.transform.GetChild(1).gameObject.SetActive(value);
        }
    }

    // Property for whether or not the toggle button is enabled
    public bool ToggleButtonEnabled {
        get =>  this.toggleButton.enabled; set => this.toggleButton.enabled = value;
    }

    private void Awake() {
        this.toggleButton = this.GetComponent<ToggleButton>();
        // Disable dropdowns by default
        DropdownActive = false;
    }

    // Function called by the menu buttons to enable/disable the dropdown
    public void ToggleDropDown() {
        DropdownActive = !DropdownActive;
    }
}

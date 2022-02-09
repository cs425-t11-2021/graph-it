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
    // Static event for when a menu button dropdown is opened
    // public static event Action<MenuButton> OnMenuButtonDropdownOpen;

    // Refernce to the toggle button component
    private ToggleButton toggleButton;

    // Property for whether or not the dropdown associated with this button is active
    public bool DropdownActive {
        // Gets or sets the activeness of the gameObject of the dropdown (second child of button)
        get {
            return this.transform.GetChild(1).gameObject.activeInHierarchy;
        }
        set {
            // If opening the dropdown menu, invoke the on menu button dropdown open event, and select the button; if closing, deselect the button
            if (value) {
                Logger.Log("Opening dropdown.", this, LogType.DEBUG);
                // MenuButton.OnMenuButtonDropdownOpen?.Invoke(this);
                
            }
            // else if (EventSystem.current.currentSelectedGameObject == this.gameObject)
            //     EventSystem.current.SetSelectedGameObject(null);
            
            this.toggleButton.UpdateStatus(value);
            this.transform.GetChild(1).gameObject.SetActive(value);
        }
    }

    // Property for whether or not the toggle button is enabled
    public bool ToggleButtonEnabled {
        get =>  this.toggleButton.enabled; set => this.toggleButton.enabled = value;
    }

    private void Awake() {
        // On awake, subscribe a method to the on menu button dropdown open event that will close this dropdown of this button
        // if it is not the caller of the event.
        // MenuButton.OnMenuButtonDropdownOpen += caller => {
        //     if (caller != this) 
        //         DropdownActive = false;
        // };
        
        this.toggleButton = this.GetComponent<ToggleButton>();
        DropdownActive = false;
    }

    public void ToggleDropDown() {
        DropdownActive = !DropdownActive;
    }

    // private void OnMenuButtonDropdownOpen(MenuButton caller) {
    //     if (caller != this)
    //         DropdownActive = false;
    // }

    //private GameObject panelObj;
    

//     // Start is called before the first frame update
//     void Start()
//     {
//         //each menu option has a Panel child to activate and de-activate 
//         //panelObj = transform.GetChild(1).gameObject;

//         //want the dropdown to not be shown or accessible when program first starts
//         //panelObj.gameObject.SetActive(false);

        
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         //TODO inplement a listener instead for efficiency
//         /*if (EventSystem.current.currentSelectedGameObject == this.gameObject){
//             //if the file button is clicked, the dropdown should be displayed
//             panelObj.gameObject.SetActive(true);
//         }
//         else{
//             //otherwise, the user clicked elsewhere and the dropdown should disappear
//             panelObj.gameObject.SetActive(false);
//         }*/
//     }

//     //keep the below function commented out, it has errors
//    /* private void DisplayDropDown(){
//         panelObj.gameObject.SetActive(true);
//         if(EventSystem.current.currentSelectedGameObject == menuItem){
//             //if the "ImportToFile" is clicked, show the import to file pop-up
//             importFileButton.gameObject.SetActive(true);
//         }
//         else{
//             //otherwise, the user clicked elsewhere and the dropdown should disappear
//             panelObj.gameObject.SetActive(false);
//         }

//     }*/
}

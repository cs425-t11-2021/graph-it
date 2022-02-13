using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : SingletonBehavior<UIManager>
{
    [Header("UI Components")]
    // References to the different main UI components
    [SerializeField] private GameObject menuBar;
    [SerializeField] private GameObject infoAndAlgorithmsPanel;
    [SerializeField] private GameObject importFileMenu;
    [SerializeField] private GameObject exportFileMenu;
    [SerializeField] private GameObject importErrorDialog;
    [SerializeField] private GameObject toolbar;
    [SerializeField] public GameObject selectionRect;

    // Property for whether or not the cursor is currently over an UI element
    public bool CursorOnUI {
        get => EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null;
    }

    // Property for whether or not the menu bar buttons are enabled
    public bool MenuBarEnabled {
        set => Array.ForEach(this.menuButtons, menuButton => menuButton.ToggleButtonEnabled = value);
    }

    // Property for whether or not the algorithm panel is enabled
    public bool AlgorithmsPanelEnabled {
        set => this.graphInfo.AlgorithmButtonsEnabled = value;
    }

    // Property for whether or not the toolbar is enabled
    public bool ToolBarEnabled {
        set {
            Button[] toolbarButtons = this.toolbar.GetComponentsInChildren<Button>();
            Array.ForEach(toolbarButtons, button => button.enabled = value);
            ToggleButton[] toolbarToggleButtons = this.toolbar.GetComponentsInChildren<ToggleButton>();
            Array.ForEach(toolbarToggleButtons, button => button.enabled = value);
        }
    }

    // Array of all menu bar buttons
    private MenuButton[] menuButtons;

    // Reference to the graph info
    private GraphInfo graphInfo;

    private void Awake() {
        // Get references
        this.menuButtons = this.menuBar.GetComponentsInChildren<MenuButton>();
        this.graphInfo = this.infoAndAlgorithmsPanel.GetComponentInChildren<GraphInfo>();
    }

    private void Update() {
        if (InputManager.Singleton.MouseButtonPressedThisFrame) {
            // If mouse was pressed but not on an UI element, close all dropdowns
            if (!CursorOnUI) {
                CloseAllDropdowns();
            }
            else {
                foreach (MenuButton mb in this.menuButtons) {
                    GameObject gameObjectClicked = EventSystem.current.currentSelectedGameObject;
                    // Go through every menu button
                    if (!gameObjectClicked || mb.gameObject != gameObjectClicked) {
                        // If the element that was clicked is not the UI button, close its dropdown; but if the element is a child of the dropdown 
                        // (one of the buttons in the dropdown), manually invoke the button's onClick before closing the dropdown
                        if (gameObjectClicked && gameObjectClicked.transform.IsChildOf(mb.transform)) {
                            Logger.Log("Calling dropdown button on " + gameObjectClicked.name, this, LogType.DEBUG);
                            gameObjectClicked.GetComponent<Button>()?.onClick.Invoke();
                        }
                        mb.DropdownActive = false;
                    }
                }
            }
        }
    }

    // Close all menu button dropdowns
    public void CloseAllDropdowns() {
        Logger.Log("Closing all menu button dropdowns.", this, LogType.DEBUG);
        Array.ForEach(this.menuButtons, menuButton => menuButton.DropdownActive = false);
    }
}
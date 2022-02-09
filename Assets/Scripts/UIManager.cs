using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // Property for whether or not the cursor is currently over an UI element
    public bool CursorOnUI {
        get => EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null;
    }

    // Delagate for when a menu button dropdown is opened
    // public Action<MenuButton> OnMenuButtonDropdownOpen;

    // Array of all menu bar buttons
    private MenuButton[] menuButtons;

    private void Awake() {
        this.menuButtons = this.menuBar.GetComponentsInChildren<MenuButton>();
    }

    private void Update() {
        if (InputManager.Singleton.MouseButtonPressedThisFrame) {
            // If mouse was pressed but not on an UI element, close all dropdowns
            if (!CursorOnUI) {
                CloseAllDropdowns();
            }

            GameObject UIElementClicked = InputManager.Singleton.GetUIElementCusorIsOn();
            foreach (MenuButton mb in this.menuButtons) {
                if (mb.gameObject != UIElementClicked) {
                    mb.DropdownActive = false;
                }
            }
        }
    }

    // Close all menu button dropdowns
    public void CloseAllDropdowns() {
        Logger.Log("Closing all menu button dropdowns.", this, LogType.DEBUG);
        foreach (MenuButton mb in this.menuButtons) {
            mb.DropdownActive = false;
        }
    }
}

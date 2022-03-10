//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFromPresetMenu : MonoBehaviour
{
    //[SerializeField] private GameObject presetMenu;

    //helper function for creating from preset menu to disable interactions with the program until the user makes a choice or closes the menu
    //NEED TO BE UPDATED WITH CURRENT UI ELEMENTS
        private void OnEnable() {
        UIManager.Singleton.MenuBarEnabled = false;
        UIManager.Singleton.AlgorithmsPanelEnabled = false;
        UIManager.Singleton.ToolBarEnabled = false;

        // Suspend the manipulation state if the import menu is active
        ManipulationStateManager.Singleton.SuspendManipulationState(true);
    }

    //helper function to restore the user's ability to interact with the program once the menu is closed
    private void OnDisable() {
        UIManager.Singleton.MenuBarEnabled = true;
        UIManager.Singleton.AlgorithmsPanelEnabled = true;
        UIManager.Singleton.ToolBarEnabled = true;

        // Unsuspend the manipulation state
        ManipulationStateManager.Singleton.SuspendManipulationState(false);
    }
    public void openPresetMenu(){
        this.gameObject.SetActive(true);
        OnEnable();//not sure if this will work

    }
    public void closePresetMenu(){
        this.gameObject.SetActive(false);
        OnDisable();//also not sure if this will work
    }
}

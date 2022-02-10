//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using TMPro;
public class ImportFile : MonoBehaviour
{
    // Reference to the file name field
    [SerializeField] private TMP_InputField importFilenameInput;
    // Reference to the error message dialog
    [SerializeField] private GameObject errorMessagePopUp;

    // Function called when the object is enabled
    private void OnEnable() {
        UIManager.Singleton.MenuBarEnabled = false;
        UIManager.Singleton.AlgorithmsPanelEnabled = false;
        UIManager.Singleton.ToolBarEnabled = false;

        // Suspend the manipulation state if the import menu is active
        ManipulationStateManager.Singleton.SuspendeManipulationState(true);
    }

    // Function called when the object is disabled
    private void OnDisable() {
        UIManager.Singleton.MenuBarEnabled = true;
        UIManager.Singleton.AlgorithmsPanelEnabled = true;
        UIManager.Singleton.ToolBarEnabled = true;

        // Unsuspend the manipulation state
        ManipulationStateManager.Singleton.SuspendeManipulationState(false);
    }

    // Function called by the confirm button
    public void Confirm() {
        //TODO implement file import
        //needs to check if a filename is provide is valid (exists)
        //if no input is given, display an error
        //InputField obeject.text gets the user input
        if(importFilenameInput.text == ""){
            errorMessagePopUp.SetActive(true);
        }
        else {
            // Clear existing graph
            Controller.Singleton.ClearGraphObjs();

            // TODO: File selector, file always saved on desktop for now
            string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            Debug.Log("Begin import at " + desktop + "/" + importFilenameInput.text + ".csv");
            Controller.Singleton.Graph.Import(desktop + "/" + importFilenameInput.text + ".csv");

            // Recrate graph objects
            Controller.Singleton.CreateGraphObjs();

            // Ondisable does not get called automatically like OnEnable, thus we call it manually
            OnDisable();
            this.gameObject.SetActive(false);
        }
    }

    // Function called by the cancel button
    public void Cancel () {
        //when the user clicks on the cancel button, the pop-up should disappear and the disabled ui elements should be re-enabled
        OnDisable();
        this.gameObject.SetActive(false);
    }
}

//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// Class for handling the behavior of the export file dialog
public class ExportFile : MonoBehaviour
{
    // Reference to the error message dialog
    [SerializeField] private GameObject errorMessagePopUp;
    // Reference to the filename field
    [SerializeField] private TMP_InputField exportFilenameInput;
    
    // Function called when the object is enabled
    private void OnEnable() {
        UIManager.Singleton.MenuBarEnabled = false;
        UIManager.Singleton.AlgorithmsPanelEnabled = false;
        UIManager.Singleton.ToolBarEnabled = false;

        // Suspend the manipulation state if the import menu is active
        ManipulationStateManager.Singleton.SuspendeManipulationState(true);
    }

    // Function called when the object is diabled
    private void OnDisable() {
        UIManager.Singleton.MenuBarEnabled = true;
        UIManager.Singleton.AlgorithmsPanelEnabled = true;
        UIManager.Singleton.ToolBarEnabled = true;

        // Unsuspend the manipulation state
        ManipulationStateManager.Singleton.SuspendeManipulationState(false);
    }

    // Function called by the confirm button
    public void Confirm() {
        if (exportFilenameInput.text == ""){
            errorMessagePopUp.SetActive(true);
        }
        else
        {
            // TODO: File selector, file always saved on desktop for now
            string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            Controller.Singleton.Graph.Export(desktop + "/" + exportFilenameInput.text + ".csv");
            // Ondisable does not get called automatically like OnEnable, thus we call it manually
            OnDisable();
            this.gameObject.SetActive(false);               
        }
    }

    // Function called by the cancel button
    public void Cancel() {
        OnDisable();
        this.gameObject.SetActive(false);
    }
}

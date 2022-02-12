//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Class for handling the behavior of the import/export error message dialog (TEMPOARY)
public class ImportExportErrorMessage : MonoBehaviour
{
    private GameObject acknowledgeButton;

    //need to prevent user from access other elements in the import or export menu
    [SerializeField]
    private Button importButton;
    [SerializeField]
    private Button exportButton;
    [SerializeField]
    private Button importCancelButton;
    [SerializeField]
    private Button exportCancelButton;
    [SerializeField]
    private TMP_InputField exportUserInput;
    [SerializeField]
    private TMP_InputField importUserInput;

    private void OnEnable() {
        this.importButton.enabled = false;
        this.exportButton.enabled = false;
        this.importCancelButton.enabled = false;
        this.exportCancelButton.enabled = false;
        this.importUserInput.enabled = false;
        this.exportUserInput.enabled = false;
    }

    private void OnDisable() {
        this.importButton.enabled = true;
        this.exportButton.enabled = true;
        this.importCancelButton.enabled = true;
        this.exportCancelButton.enabled = true;
        this.importUserInput.enabled = true;
        this.exportUserInput.enabled = true;
    }

    public void Ackowledge() {
        // Ondisable does not get called automatically like OnEnable, thus we call it manually
        OnDisable();
        this.gameObject.SetActive(false);
    }
}

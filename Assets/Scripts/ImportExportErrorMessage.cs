//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
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

    // Start is called before the first frame update
    void Start()
    {
        //do not want to display pop up when program first starts
        this.gameObject.SetActive(false);
        this.acknowledgeButton = transform.GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {   
        //if popup is displayed, disable all other actions in the import or export menu
        if(this.gameObject.activeInHierarchy){
            this.importButton.enabled = false;
            this.exportButton.enabled = false;
            this.importCancelButton.enabled = false;
            this.exportCancelButton.enabled = false;
            this.importUserInput.enabled = false;
            this.exportUserInput.enabled = false;
        }
        else {
            this.importButton.enabled = true;
            this.exportButton.enabled = true;
            this.importCancelButton.enabled = true;
            this.exportCancelButton.enabled = true;
            this.importUserInput.enabled = true;
            this.exportUserInput.enabled = true;
        }

        if(EventSystem.current.currentSelectedGameObject == this.acknowledgeButton){
            this.gameObject.SetActive(false);
            this.importButton.enabled = true;
            this.exportButton.enabled = true;
            this.importCancelButton.enabled = true;
            this.exportCancelButton.enabled = true;
            this.importUserInput.enabled = true;
            this.exportUserInput.enabled = true;
        }
    }
}

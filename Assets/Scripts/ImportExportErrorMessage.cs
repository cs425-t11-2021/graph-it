using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImportExportErrorMessage : MonoBehaviour
{
    private GameObject acknowledgeButton;

    //need to prevent user from access other elements in the import or export menu
    public Button importButton;
    public Button exportButton;
    public Button importCancelButton;
    public Button exportCancelButton;
    //public InputField importUserInput;
   // public InputField exportUserInput;

    // Start is called before the first frame update
    void Start()
    {
        //do not want to display pop up when program first starts
        this.gameObject.SetActive(false);
        acknowledgeButton = transform.GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {   
        //if popup is displayed, disable all other actions in the import or export menu
        if(this.gameObject.activeInHierarchy){
            importButton.enabled = false;
            exportButton.enabled = false;
            importCancelButton.enabled = false;
            exportCancelButton.enabled = false;
           // importUserInput.enabled = false;
           // exportUserInput.enabled = false;
        }

        if(EventSystem.current.currentSelectedGameObject == acknowledgeButton){
            this.gameObject.SetActive(false);
            importButton.enabled = true;
            exportButton.enabled = true;
            importCancelButton.enabled = true;
            exportCancelButton.enabled = true;
           // importUserInput.enabled = true;
            //exportUserInput.enabled = true;
        }
    }
}

//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ExportFile : MonoBehaviour
{
    private GameObject cancelButton;
    private GameObject exportButton;

    //Need to disable the rest of the UI elements when the export to file menu pop-up is displayed
    public Button fileButton;
    public Button editButton;
    public Button viewButton;
    public GameObject fileDropDown;
    public GameObject errorMessagePopUp;
    public TMP_InputField exportFilenameInput; //maybe consider making private
    
    public Button algorithmsPanelPrims;//need to figure out how to deactivate them all at once, maybe put them all on the same panel and deactivate
                                            //or as a dropdown and disable that

    // Start is called before the first frame update
    void Start()
    {
        //when program first starts, user should not be able to access the export from file menu pop-up
        //this.gameObject.SetActive(false);

        //getting references to the cancel and export buttons to perform their corresponding actions
        exportButton = transform.GetChild(2).gameObject;
        cancelButton = transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO need to disable (and later enable) the algorithms panel
        //if the export to file menu pop-up is displayed, the user should not be able to perform any additional functions outside of the pop-up
        if(this.gameObject.activeInHierarchy){
            fileButton.enabled = false;
            editButton.enabled = false;
            viewButton.enabled = false;
            algorithmsPanelPrims.enabled = false;
            fileDropDown.gameObject.SetActive(false); //the file menu dropdown should also no longer be accessable
        }
        else {
            fileButton.enabled = true;
            editButton.enabled = true;
            viewButton.enabled = true;
            algorithmsPanelPrims.enabled = true;
            fileDropDown.gameObject.SetActive(true); //the file menu dropdown should also no longer be accessable
        }

        
        if(EventSystem.current.currentSelectedGameObject == exportButton){
            EventSystem.current.SetSelectedGameObject(null);

            //TODO implement file export
            //needs to check if a filename is provide (for export needs to check if such file already exists)
            //if no input is given, display an error
            //InputField obeject.text gets the user input
            if (exportFilenameInput.text == ""){
                errorMessagePopUp.SetActive(true);
            }
            else
            {
                // TODO: File selector, file always saved on desktop for now
                string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
                Controller.Singleton.Graph.Export(desktop + "/" + exportFilenameInput.text + ".csv");
                this.gameObject.SetActive(false);
                fileButton.enabled = true;
                editButton.enabled = true;
                viewButton.enabled = true;
                algorithmsPanelPrims.enabled = true;                
            }
        }
        else if(EventSystem.current.currentSelectedGameObject == cancelButton){
            EventSystem.current.SetSelectedGameObject(null);
            //when the user clicks on the cancel button, the pop-up should disappear and the disabled ui elements should be re-enabled
            this.gameObject.SetActive(false);
            fileButton.enabled = true;
            editButton.enabled = true;
            viewButton.enabled = true;
            algorithmsPanelPrims.enabled = true;
        }
    }
}

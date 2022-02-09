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
    [SerializeField]
    private GameObject cancelButton;
    [SerializeField]
    private GameObject importButton;
    [SerializeField]
    private TMP_InputField importFilenameInput;

    // //Need to disable the rest of the UI elements when the import from file menu pop-up is displayed
    // [SerializeField]
    // private Button fileButton;
    // [SerializeField]
    // private Button editButton;
    // [SerializeField]
    // private Button viewButton;
    // [SerializeField]
    // private GameObject fileDropDown;
    [SerializeField]
    private GameObject errorMessagePopUp;
    [SerializeField]
    private GameObject toolbar;
    //public InputField importFilenameInput; //maybe consider making private
    [SerializeField]
    private Button algorithmsPanelPrims;//need to figure out how to deactivate them all at once, maybe put them all on the same panel and deactivate
                                            //or as a dropdown and disable that

    // Start is called before the first frame update
    void Start()
    {
        //when program first starts, user should not be able to access the import from file menu pop-up
        //this.gameObject.SetActive(false);

        //getting references to the cancel and import buttons to perform their corresponding actions
        //importButton = transform.GetChild(2).gameObject;
        //cancelButton = transform.GetChild(1).gameObject;
    }

    private void OnEnable() {
        UIManager.Singleton.MenuBarEnabled = false;
        UIManager.Singleton.AlgorithmsPanelEnabled = false;
        UIManager.Singleton.ToolBarEnabled = false;
    }

    private void OnDisable() {
        UIManager.Singleton.MenuBarEnabled = true;
        UIManager.Singleton.AlgorithmsPanelEnabled = true;
        UIManager.Singleton.ToolBarEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO need to disable (and later enable) the algorithms panel
        //if the import from file menu pop-up is displayed, the user should not be able to perform any additional functions outside of the pop-up
        // if(this.gameObject.activeInHierarchy){
            // this.fileButton.enabled = false;
            // this.editButton.enabled = false;
            // this.viewButton.enabled = false;
            // this.algorithmsPanelPrims.enabled = false;
            // this.toolbar.gameObject.SetActive(false);
            // this.fileDropDown.gameObject.SetActive(false); //the file menu dropdown should also no longer be accessable
        // }
    }

        
        /*if(EventSystem.current.currentSelectedGameObject == importButton){
            //TODO implement file import
            //needs to check if a filename is provide is valid (exists)
            //if no input is given, display an error
            //InputField obeject.text gets the user input
            if(importFilenameInput.text == ""){
                errorMessagePopUp.SetActive(true);
            }
            else {
                EventSystem.current.SetSelectedGameObject(null);

                // Clear existing graph
                Controller.singleton.ClearGraphObjs();

                // TODO: File selector, file always saved on desktop for now
                string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
                Debug.Log("Begin import at " + desktop + "/" + importFilenameInput.text + ".csv");
                Controller.singleton.Graph.Import(desktop + "/" + importFilenameInput.text + ".csv");
                this.gameObject.SetActive(false);
                fileButton.enabled = true;
                editButton.enabled = true;
                viewButton.enabled = true;
                algorithmsPanelPrims.enabled = true;

                // Recrate graph objects
                Controller.singleton.CreateGraphObjs();
            }
        }
        else if(EventSystem.current.currentSelectedGameObject == cancelButton){
            EventSystem.current.SetSelectedGameObject(null);

            //when the user clicks on the cancel button, the pop-up should disappear and the disabled ui elements should be re-enabled
            this.gameObject.SetActive(false);
            toolbar.gameObject.SetActive(true);
            fileButton.enabled = true;
            editButton.enabled = true;
            viewButton.enabled = true;
            algorithmsPanelPrims.enabled = true;
        }
    }*/
    public void Confirm() {
        //TODO implement file import
        //needs to check if a filename is provide is valid (exists)
        //if no input is given, display an error
        //InputField obeject.text gets the user input
        if(importFilenameInput.text == ""){
            errorMessagePopUp.SetActive(true);
        }
        else {
            // EventSystem.current.SetSelectedGameObject(null);

            // Clear existing graph
            Controller.Singleton.ClearGraphObjs();

            // TODO: File selector, file always saved on desktop for now
            string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            Debug.Log("Begin import at " + desktop + "/" + importFilenameInput.text + ".csv");
            Controller.Singleton.Graph.Import(desktop + "/" + importFilenameInput.text + ".csv");
            
            // fileButton.enabled = true;
            // editButton.enabled = true;
            // viewButton.enabled = true;
            // algorithmsPanelPrims.enabled = true;
            // toolbar.gameObject.SetActive(true);
            // fileDropDown.gameObject.SetActive(true);

            // Recrate graph objects
            Controller.Singleton.CreateGraphObjs();

            // Ondisable does not get called automatically like OnEnable, thus we call it manually
            OnDisable();
            this.gameObject.SetActive(false);
        }
    }

    public void Cancel () {
        // EventSystem.current.SetSelectedGameObject(null);

        //when the user clicks on the cancel button, the pop-up should disappear and the disabled ui elements should be re-enabled
        OnDisable();
        this.gameObject.SetActive(false);
        // toolbar.gameObject.SetActive(true);
        // fileButton.enabled = true;
        // editButton.enabled = true;
        // viewButton.enabled = true;
        // algorithmsPanelPrims.enabled = true;
        // toolbar.gameObject.SetActive(true);
        // fileDropDown.gameObject.SetActive(true);
    }
}

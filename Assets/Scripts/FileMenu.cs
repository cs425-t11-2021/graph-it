//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FileMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject dropDownMenu;
    [SerializeField]
    private GameObject newGraphButton;
    //[SerializeField]
    //private GameObject importMenuItem;
    //[SerializeField]
   // private GameObject exportMenuItem;

   // private Button fileButton;
    [SerializeField]
    private GameObject importFileMenu;
    [SerializeField]
    private GameObject exportFileMenu;
    //[SerializeField]
    //private GameObject editDropDown;
    //[SerializeField]
    //private GameObject viewDropDown;



    private Button fileButton;

    private void Awake() {
        fileButton = this.gameObject.GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*//getting the "ImportToFile" button object to check if clicked
        importMenuItem = transform.GetChild(1).GetChild(2).gameObject;
        //getting the "ExportToFile" button object to check if clicked
        exportMenuItem = transform.GetChild(1).GetChild(3).gameObject;

        //getting the "file" button to monitor if selected
        //fileButton = this.GetComponent<Button>();
        //fileButton.onClick.AddListener(DisplayDropDown);*/
    }

    // Update is called once per frame
    void LateUpdate()
    {
      /*  if (EventSystem.current.currentSelectedGameObject == importMenuItem)
        {
            //if the "ImportToFile" is clicked, show the import to file pop-up
            importFileButton.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == exportMenuItem)
        {
            exportFileButton.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == newGraphButton)
        {
            Controller.singleton.ClearGraphObjs();
            Controller.singleton.Graph.Clear();
        }*/
        if (!dropDownMenu.activeInHierarchy) return;
        
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
            if( !(Controller.singleton.UIActive())) { //does not close dropdown when another button is pressed
                dropDownMenu.SetActive(false);
            }
            
            if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.transform.IsChildOf(this.transform)) {
                dropDownMenu.SetActive(false);
            }
        }
    }

    //if the user clicks on the "File" button, the dropdown will appear, otherwise the dropdown remains hidden
      public void ToggleDropDown(){
        if(dropDownMenu.activeInHierarchy){
            EventSystem.current.SetSelectedGameObject(null);
            dropDownMenu.SetActive(false);
        }
        else{
            fileButton.Select();
            dropDownMenu.SetActive(true);
            //editDropDown.SetActive(false);
            //viewDropDown.SetActive(false);
        }
    }

    //When the user selects the "New Graph" button; the existing graph is cleared for the user to create a new graph
    public void NewGraphFunc(){
        Controller.singleton.ClearGraphObjs();
        Controller.singleton.Graph.Clear();
        CloseDropDown();
    }

    public void ImportFromFile(){
        importFileMenu.gameObject.SetActive(true);
        CloseDropDown();
    }

    public void ExportToFile(){
        exportFileMenu.gameObject.SetActive(true);
        CloseDropDown();
    }

    public void CloseDropDown(){
        dropDownMenu.SetActive(false);
    }
}

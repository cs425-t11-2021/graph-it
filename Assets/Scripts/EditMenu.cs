//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditMenu : MenuButton
{
    // Reference to child buttons assigned in Unity Inspector
    [SerializeField]
    private GameObject dropDownMenu;
    [SerializeField]
    private GameObject selectAllButton;
    [SerializeField]
    private GameObject deselectAllButton;
    [SerializeField]
    private Button addEdgeButton;
    //[SerializeField]
    //private GameObject fileDropDown;
   // [SerializeField]
    //private GameObject viewDropDown;

    // private Button editButton;

    // private void Awake() {
    //     this.addEdgeButton.interactable = false;
    //     this.editButton = this.gameObject.GetComponent<Button>();
    // }

    // Update is called once per frame
    void Update()
    {
        if (SelectionManager.Singleton.SelectedVertexCount() == 2 && SelectionManager.Singleton.SelectedEdgeCount() == 0) {
            this.addEdgeButton.interactable = true;
        }
        else {
            this.addEdgeButton.interactable = false;
        }

        //If the user clicks close the dropdown panel
        // if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
        //     if(!(Controller.singleton.UIActive())){ //does not close dropdown when another button is pressed
        //         dropDownMenu.SetActive(false);
        //     }
        // }
        /*if (EventSystem.current.currentSelectedGameObject == selectAllButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            SelectionManager.singleton.SelectAll();
        }
        else if (EventSystem.current.currentSelectedGameObject == deselectAllButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            SelectionManager.singleton.DeSelectAll();
        }
        else if (EventSystem.current.currentSelectedGameObject == addEdgeButton.gameObject) {
            SelectionManager.singleton.AddEdge();
        }*/

        // if (!this.dropDownMenu.activeInHierarchy) return;
        
        // if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
        //     if( !(Controller.Singleton.UIActive())) { //does not close dropdown when another button is pressed
        //         this.dropDownMenu.SetActive(false);
        //     }
            
        //     if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.transform.IsChildOf(this.transform)) {
        //         this.dropDownMenu.SetActive(false);
        //     }
        // }
       
    }

    // public void ToggleDropDown(){
    //     if(dropDownMenu.activeInHierarchy){
    //         EventSystem.current.SetSelectedGameObject(null);
    //         this.dropDownMenu.SetActive(false);
    //     }
    //     else{
    //         dropDownMenu.SetActive(true);
    //         this.editButton.Select();
    //         //fileDropDown.SetActive(false);
    //        // viewDropDown.SetActive(false);
    //     }
    // }
    public void ToggleSelectAll(){
        // CloseDropDown();
        //not sure if something else is needed
        SelectionManager.Singleton.SelectAll();
    }

    public void ToggleDeselectAll(){
        // CloseDropDown();
        SelectionManager.Singleton.DeSelectAll();
    }

    public void AddEdgeButtonFromEditMenu(){
        // CloseDropDown();
        SelectionManager.Singleton.AddEdge();
    }

    // public void CloseDropDown(){
    //     this.dropDownMenu.SetActive(false);
    // }
}

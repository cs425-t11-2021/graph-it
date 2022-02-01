//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditMenu : MonoBehaviour
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
    

    private void Awake() {
        addEdgeButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectionManager.singleton.SelectedVertexCount() == 2 && SelectionManager.singleton.SelectedEdgeCount() == 0) {
            addEdgeButton.interactable = true;
        }
        else {
            addEdgeButton.interactable = false;
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

    public void ToggleDropDown(){
        if(dropDownMenu.activeInHierarchy){
            dropDownMenu.SetActive(false);
        }
        else{
            dropDownMenu.SetActive(true);
            //fileDropDown.SetActive(false);
           // viewDropDown.SetActive(false);
        }
    }
    public void ToggleSelectAll(){
        CloseDropDown();
        //not sure if something else is needed
        SelectionManager.singleton.SelectAll();
    }

    public void ToggleDeselectAll(){
        CloseDropDown();
        SelectionManager.singleton.DeSelectAll();
    }

    public void AddEdgeButtonFromEditMenu(){
        CloseDropDown();
        SelectionManager.singleton.AddEdge();
    }

    public void CloseDropDown(){
        dropDownMenu.SetActive(false);
    }
}

//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ViewMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_Text showGrpahLabelsText;
    [SerializeField]
    private GameObject dropDownMenu;
   // [SerializeField]
    //private GameObject fileDropDown;
    //[SerializeField]
    //private GameObject editDropDown;

    private Button viewButton;
    private void Awake() {
        this.viewButton = this.gameObject.GetComponent<Button>();
    }

    private void Update()
    {
        // TODO: Avoid using EventSystem for UI updates
       /* if (EventSystem.current.currentSelectedGameObject == showGrpahLabelsText.transform.parent.gameObject)
        {
            ToggleLabels();
            EventSystem.current.SetSelectedGameObject(null);
        }*/
        //If the user clicks close the dropdown panel
        if (!this.dropDownMenu.activeInHierarchy) return;
        
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
            if( !(Controller.Singleton.UIActive())) { //does not close dropdown when another button is pressed
                this.dropDownMenu.SetActive(false);
            }
            
            if (EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.transform.IsChildOf(this.transform)) {
                this.dropDownMenu.SetActive(false);
            }
        }
    }

    // Method called by the "Show Graph Labels" button in the view dropdown to toggle the display of vertex and edge labels
    public void ToggleLabels()
    {
        CloseDropDown();
        if (Controller.Singleton.DisplayVertexLabels)
        {
            Controller.Singleton.DisplayVertexLabels = false;
            this.showGrpahLabelsText.text = "Show Graph Labels";
        }
        else
        {
            Controller.Singleton.DisplayVertexLabels = true;
            this.showGrpahLabelsText.text = "Hide Graph Labels";
        }
    }

    public void ToggleDropDown(){
        if(dropDownMenu.activeInHierarchy){
            EventSystem.current.SetSelectedGameObject(null);
            this.dropDownMenu.SetActive(false);
        }
        else{
            this.viewButton.Select();
            this.dropDownMenu.SetActive(true);
            //fileDropDown.SetActive(false);
           // editDropDown.SetActive(false);
        }
    }

    public void CloseDropDown(){
        this.dropDownMenu.SetActive(false);
    }
}

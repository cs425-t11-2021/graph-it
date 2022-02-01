//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private void Update()
    {
        // TODO: Avoid using EventSystem for UI updates
       /* if (EventSystem.current.currentSelectedGameObject == showGrpahLabelsText.transform.parent.gameObject)
        {
            ToggleLabels();
            EventSystem.current.SetSelectedGameObject(null);
        }*/
        //If the user clicks close the dropdown panel
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
            if(!(Controller.singleton.UIActive())){ //does not close dropdown when another button is pressed
                dropDownMenu.SetActive(false);
            } 
        }
    }

    // Method called by the "Show Graph Labels" button in the view dropdown to toggle the display of vertex and edge labels
    public void ToggleLabels()
    {
        CloseDropDown();
        if (Controller.singleton.DisplayVertexLabels)
        {
            Controller.singleton.DisplayVertexLabels = false;
            showGrpahLabelsText.text = "Show Graph Labels";
        }
        else
        {
            Controller.singleton.DisplayVertexLabels = true;
            showGrpahLabelsText.text = "Hide Graph Labels";
        }
    }

    public void ToggleDropDown(){
        if(dropDownMenu.activeInHierarchy){
            dropDownMenu.SetActive(false);
        }
        else{
            dropDownMenu.SetActive(true);
            //fileDropDown.SetActive(false);
           // editDropDown.SetActive(false);
        }
    }

    public void CloseDropDown(){
        dropDownMenu.SetActive(false);
    }
}

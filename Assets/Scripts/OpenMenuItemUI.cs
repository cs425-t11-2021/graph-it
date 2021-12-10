//All code developed by Team 11
//This script is a general script for displaying the corresponding menu associated with the option

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenMenuItemUI : MonoBehaviour
{
    private GameObject menuObj;
    private Button menuButton;
    private GameObject dropdownObj;

    // Start is called before the first frame update
    void Start()
    {
        //menu item has an assciated menu to display to the user
        menuObj = transform.GetChild(1).gameObject;
        //do not want the associated menu to be shown or accessible when program first starts
        menuObj.gameObject.SetActive(false);
        
        //dropdown menu must be active for the import menu to appear
        dropdownObj = this.transform.parent.gameObject;
        dropdownObj.gameObject.SetActive(true);

        //When the "Import from file" is clicked called the OpenPopUp function to display the menu pop-up
        menuButton = this.GetComponent<Button>();
        menuButton.onClick.AddListener(OpenPopUp);
        
    }

    // Update is called once per frame
    //void Update()
    //{
        /*//TODO inplement a listener instead for efficiency
        if (EventSystem.current.currentSelectedGameObject == this.gameObject){
            menuObj.gameObject.SetActive(true);
        }*/
    //}

    /*private void OnMouseClick(){
        menuObj.gameObject.SetActive(true);
        
    }*/

    private void OpenPopUp(){
        dropdownObj.gameObject.SetActive(true);
        menuObj.gameObject.SetActive(true);
       // Debug.Log("Import was clicked");
    }
}

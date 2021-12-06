//This script is a general script for displaying the corresponding menu associated with the option

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenMenuItemUI : MonoBehaviour
{
    private GameObject panelObj;
    private Button menuButton;

    // Start is called before the first frame update
    void Start()
    {
        //menu item has an assciated menu to display to the user
        panelObj = transform.GetChild(1).gameObject;
        //do not want the associated menu to be shown or accessible when program first starts
        panelObj.gameObject.SetActive(false);

        //When the "Import from file" is clicked called the OpenPopUp function to display the menu pop-up
        menuButton = panelObj.GetComponent<Button>();
        menuButton.onClick.AddListener(OpenPopUp);
        
    }

    // Update is called once per frame
    void Update()
    {
        /*//TODO inplement a listener instead for efficiency
        if (EventSystem.current.currentSelectedGameObject == this.gameObject){
            menuObj.gameObject.SetActive(true);
        }*/
    }

    /*private void OnMouseClick(){
        menuObj.gameObject.SetActive(true);
        
    }*/

    private void OpenPopUp(){
        panelObj.gameObject.SetActive(true);
        Debug.Log("Import was clicked");
    }
}

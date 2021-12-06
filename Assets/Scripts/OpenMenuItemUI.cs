//This script is a general script for displaying the corresponding menu associated with the option

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenMenuItemUI : MonoBehaviour
{
    private GameObject menuObj;

    // Start is called before the first frame update
    void Start()
    {
        //menu item has an assciated menu to display to the user
        menuObj = transform.GetChild(1).gameObject;
        //do not want the associated menu to be shown or accessible when program first starts
        menuObj.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        //maybe inplement a listener instead for efficiency
        if (EventSystem.current.currentSelectedGameObject == this.gameObject){
            menuObj.gameObject.SetActive(true);
        }
        else{
            menuObj.gameObject.SetActive(false);
        }
    }
}

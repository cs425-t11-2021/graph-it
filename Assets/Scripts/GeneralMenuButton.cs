//This script is for the general functionality of a menu dropdown 
//When the program begins, the dropdown panels should not be shown or accessible
//The dropdown appears when the user clicks on the menu button and remains until the user selects an option or clicks away

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GeneralMenuButton : MonoBehaviour
{
    private GameObject panelObj;
    private GameObject menuItem;

    // Start is called before the first frame update
    void Start()
    {
        //each menu option has a Panel child to activate and de-activate 
        panelObj = transform.GetChild(1).gameObject;
        //want the dropdown to not be shown or accessible when program first starts
        panelObj.gameObject.SetActive(false);

        menuItem = panelObj.transform.GetChild(2).transform.GetChild(1).gameObject;
        menuItem.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //TODO inplement a listener instead for efficiency
        if (EventSystem.current.currentSelectedGameObject == this.gameObject){
            panelObj.gameObject.SetActive(true);
            //Debug.Log("file was clicked");
        }
        /*else if(EventSystem.current.currentSelectedGameObject == menuItem){
            menuItem.gameObject.SetActive(true);
        }*/
        else{
            panelObj.gameObject.SetActive(false);
        }
    }
}

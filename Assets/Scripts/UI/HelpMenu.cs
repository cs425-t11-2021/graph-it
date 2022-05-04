//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenu : MenuButton
{
    [SerializeField] private string websiteHomeURL; //string to hold URL to the Graph-It homepage
    [SerializeField] private string resourcesURL; //string to hold URL to graph algorithm and terminology resources --CURRENTLY JUST THE GRAPH-IT HOMEPAGE

    public void LinktoGraphItWebsite(){ // opens website to Graph-It homepage
        Application.OpenURL(websiteHomeURL);
    }

    public void LinkResourcesWebpage(){ //opens webpage with resources and additional info on graph terminology and resources
        Application.OpenURL(resourcesURL);
    }
}

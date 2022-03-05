//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Class for hosting functions called by buttons of the File dropdown menu, inherits from MenuButton
public class FileMenu : MenuButton
{
    // References to the import and export file dialogs
    [SerializeField]
    private GameObject importFileMenu;
    [SerializeField]
    private GameObject exportFileMenu;

    //When the user selects the "New Graph" button; the existing graph is cleared for the user to create a new graph
    public void NewGraphFunc(){
        Logger.Log("Creating a new graph.", this, LogType.DEBUG);
        Controller.Singleton.CreateGraphInstance();
        
        // TEMPOARY
        ResourceManager.Singleton.LoadVertexSprites();
    }

    // Function called by the import from file button
    public void ImportFromFile(){
        this.importFileMenu.gameObject.SetActive(true);
    }

    // Function called by the export to file button
    public void ExportToFile(){
        this.exportFileMenu.gameObject.SetActive(true);
    }

    // Function called by the save as image button
    public void SaveAsImage() {
        ScreenshotManager.Singleton.SaveScrenshotToDesktop();
    }
}

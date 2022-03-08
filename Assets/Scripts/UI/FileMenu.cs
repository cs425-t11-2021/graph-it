//All code developed by Team 11 -- code that is not is indicated
////The file system (for importing, exporting, and saving)uses the UnityStandAloneFileBrowser Plugin found here: https://github.com/gkngkc/UnityStandaloneFileBrowser
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB; //not developed by Team 11, from UnityStandAloneFileBrowser Plugin

// Class for hosting functions called by buttons of the File dropdown menu, inherits from MenuButton
public class FileMenu : MenuButton
{
    // References to the import and export file dialogs
    [SerializeField]
    private GameObject importFileMenu;
    [SerializeField]
    private GameObject exportFileMenu;

    [SerializeField] private GameObject fileDropDown;

    //When the user selects the "New Graph" button; the existing graph is cleared for the user to create a new graph
    public void NewGraphFunc(){
        Logger.Log("Creating a new graph.", this, LogType.DEBUG);
        Controller.Singleton.CreateGraphInstance();
    }

    // Function called by the import from file button
    //Utilizes UnityStandAloneFileBrowser Plugin
    public void ImportFromFile(){
        fileDropDown.SetActive(false);
        // Clear existing graph
        Controller.Singleton.CreateGraphInstance();
        //string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        
        //declararion modified from UnityStandAloneFileBrowser Plugin example usage
        ExtensionFilter[] extensions = new []{
            new ExtensionFilter ("Comma Seperated Lists","csv") //from UnityStandAloneFileBrowser Plugin
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Import from File", "", extensions, false); //from UnityStandAloneFileBrowser Plugin
        
        Debug.Log("Begin import at " + paths + ".csv");
        //Debug.Log(paths[0]);
        //Controller.Singleton.Graph.Import(desktop + "/" + importFilenameInput.text + ".csv");

        Controller.Singleton.CreateObjsFromGraph();
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

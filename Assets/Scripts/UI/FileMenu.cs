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

    [SerializeField] private GameObject fileDropDown; //TO DO - close immediately after selection of option
    [SerializeField] private GameObject createFromPresetMenu;

    //When the user selects the "New Graph" button; the existing graph is cleared for the user to create a new graph
    public void NewGraphFunc(){
        Logger.Log("Creating a new graph.", this, LogType.DEBUG);
        Controller.Singleton.ClearCurrentInstance();
        
        // TEMPOARY
        ResourceManager.Singleton.LoadVertexSprites();
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    //display pop-up with options users can choose from to create graphs from
    public void CreateFromPreset(){
        createFromPresetMenu.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Function called by the import from file button
    //Utilizes UnityStandAloneFileBrowser Plugin
    public void ImportFromFile(){
        fileDropDown.SetActive(false); //this doesn't work as intended
        
        //string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        
        //declararion modified from UnityStandAloneFileBrowser Plugin example usage
        ExtensionFilter[] importExtension = new []{
            new ExtensionFilter ("Comma Seperated Lists","csv") //from UnityStandAloneFileBrowser Plugin
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Import from File", "", importExtension, false); //from UnityStandAloneFileBrowser Plugin
        
        //if the user does not cancel the file import menu, clear the current graph to import the new one //IMPORT INTO A NEW TAB LATER TO NOT OVERWRITE CURRENT WORK
        if(paths.Length != 0){
            // Clear existing graph
            Controller.Singleton.CreateGraphInstance();
        }
        Controller.Singleton.Graph.Import(paths[0]);
        Controller.Singleton.CreateObjsFromGraph();
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Function called by the export to file button
    public void ExportToFile(){
        fileDropDown.SetActive(false); //this doesn't work as intended
        //this.exportFileMenu.gameObject.SetActive(true);
        //declararion modified from UnityStandAloneFileBrowser Plugin example usage
        ExtensionFilter[] exportExtensions = new [] {
            new ExtensionFilter("Comma Seperated Lists", "csv")//from UnityStandAloneFileBrowser Plugin
        };
        string path = StandaloneFileBrowser.SaveFilePanel("Export to File", "", "Graph1", exportExtensions); //from UnityStandAloneFileBrowser Plugin

        Controller.Singleton.Graph.Export(path);
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Function called by the save as image button
    public void SaveAsImage() {
        fileDropDown.SetActive(false); //this doesn't work as intended
        //declararion modified from UnityStandAloneFileBrowser Plugin example usage
        ExtensionFilter[] imageSaveExtensions = new [] {
            new ExtensionFilter("Image Files", "png")//from UnityStandAloneFileBrowser Plugin
        };
        string path = StandaloneFileBrowser.SaveFilePanel("Export to File", "", "GraphImage1", imageSaveExtensions); //from UnityStandAloneFileBrowser Plugin

        ScreenshotManager.Singleton.TakeScreenshot(path);
        
        EventSystem.current.SetSelectedGameObject(null);
    }
}

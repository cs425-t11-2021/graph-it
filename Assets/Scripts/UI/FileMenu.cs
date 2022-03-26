//All code developed by Team 11 -- code that is not is indicated
////The file system (for importing, exporting, and saving)uses the UnityStandAloneFileBrowser Plugin found here: https://github.com/gkngkc/UnityStandaloneFileBrowser
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    [SerializeField] private CreateFromPresetMenu presetMenu; //no clue if this will work

    //When the user selects the "New Graph" button; the existing graph is cleared for the user to create a new graph
    public void NewGraphFunc(){
        Logger.Log("Creating a new graph.", this, LogType.DEBUG);
        Controller.Singleton.CreateGraphInstance();
        
        // TEMPOARY
        ResourceManager.Singleton.LoadVertexSprites();
        
        NotificationManager.Singleton.CreateNotification("Creating a new graph.", 3);
        EventSystem.current.SetSelectedGameObject(null);
    }

    
    //display pop-up with options users can choose from to create graphs from
    public void CreateFromPreset(){
        presetMenu.openPresetMenu();
        
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
        if(paths.Length != 0 && !string.IsNullOrEmpty(paths[0])){
            // Clear existing graph
            GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true);
            
            Controller.Singleton.Graph.Import(paths[0]);
            Controller.Singleton.CreateObjsFromGraph(newInstance);
            
            // Change tab name to exported file name
            TabBar.Singleton.ActiveTab.TabName = Path.GetFileNameWithoutExtension(paths[0]);
            
            NotificationManager.Singleton.CreateNotification(string.Format("Imported <#0000FF>{0}</color>", Path.GetFileName(paths[0])), 3);
        }

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
        string path = StandaloneFileBrowser.SaveFilePanel("Export to File", "", TabBar.Singleton.ActiveTab.TabName, exportExtensions); //from UnityStandAloneFileBrowser Plugin

        if (!string.IsNullOrEmpty(path))
        {
            Controller.Singleton.Graph.Export(path);

            // Change tab name to exported file name
            TabBar.Singleton.ActiveTab.TabName = Path.GetFileNameWithoutExtension(path);
            
            NotificationManager.Singleton.CreateNotification(string.Format("Exported to <#0000FF>{0}</color>", Path.GetFileName(path)), 3);
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    // Function called by the save as image button
    public void SaveAsImage() {
        fileDropDown.SetActive(false); //this doesn't work as intended
        //declararion modified from UnityStandAloneFileBrowser Plugin example usage
        ExtensionFilter[] imageSaveExtensions = new [] {
            new ExtensionFilter("Image Files", "png")//from UnityStandAloneFileBrowser Plugin
        };
        string path = StandaloneFileBrowser.SaveFilePanel("Export to File", "", TabBar.Singleton.ActiveTab.TabName, imageSaveExtensions); //from UnityStandAloneFileBrowser Plugin

        if (!string.IsNullOrEmpty(path))
        {
            ScreenshotManager.Singleton.TakeScreenshot(path);
            
            NotificationManager.Singleton.CreateNotification(string.Format("Exported to <#0000FF>{0}</color>", Path.GetFileName(path)), 3);
        }

        EventSystem.current.SetSelectedGameObject(null);
    }
}

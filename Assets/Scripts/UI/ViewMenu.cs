//All code developed by Team 11

using TMPro;
using UnityEngine;

// Class for hosting functions called by buttons of the View dropdown menu, inherits from MenuButton
public class ViewMenu : MenuButton
{
    [SerializeField]
    private TMP_Text showGrpahLabelsText;

    [SerializeField] private TMP_Text showToolbarText;

    [SerializeField] private GameObject toolbar;

    
    public void ToggleToolbar() {
        //if active, set inactive
        if(toolbar.activeSelf){
            toolbar.SetActive(false);
            this.showToolbarText.text = "Show Toolbar";
        }
        //otherwise inactive and needs to be set active
        else{
            toolbar.SetActive(true);
            this.showToolbarText.text = "Hide Toolbar";
        }
    }

    // Method called by the "Show Graph Labels" button in the view dropdown to toggle the display of vertex and edge labels
    public void ToggleLabels()
    {
        if (SettingsManager.Singleton.DisplayVertexLabels)
        {
            SettingsManager.Singleton.DisplayVertexLabels = false;
            this.showGrpahLabelsText.text = "Show Graph Labels";
        }
        else
        {
            SettingsManager.Singleton.DisplayVertexLabels = true;
            this.showGrpahLabelsText.text = "Hide Graph Labels";
        }
    }
}

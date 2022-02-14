//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// Class for hosting functions called by buttons of the View dropdown menu, inherits from MenuButton
public class ViewMenu : MenuButton
{
    [SerializeField]
    private TMP_Text showGrpahLabelsText;

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

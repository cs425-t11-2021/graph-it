//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ViewMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_Text showGrpahLabelsText;

    private void Update()
    {
        // TODO: Avoid using EventSystem for UI updates
        if (EventSystem.current.currentSelectedGameObject == showGrpahLabelsText.transform.parent.gameObject)
        {
            ToggleLabels();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Method called by the "Show Graph Labels" button in the view dropdown to toggle the display of vertex and edge labels
    public void ToggleLabels()
    {
        if (Controller.singleton.DisplayVertexLabels)
        {
            Controller.singleton.DisplayVertexLabels = false;
            showGrpahLabelsText.text = "Show Graph Labels";
        }
        else
        {
            Controller.singleton.DisplayVertexLabels = true;
            showGrpahLabelsText.text = "Hide Graph Labels";
        }
    }
}

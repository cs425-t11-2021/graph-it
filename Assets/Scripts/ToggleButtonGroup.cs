// Modification of code found on https://answers.unity.com/questions/314926/make-a-button-behave-like-a-toggle.html 

using System.Collections.Generic;
using UnityEngine;

public class ToggleButtonGroup : MonoBehaviour
{
    List<ToggleButton> _toggles = new List<ToggleButton>();

    public void RegisterToggle(ToggleButton toggle)
    {
        _toggles.Add(toggle);
        toggle.CheckedChanged.AddListener(HandleCheckedChanged);
    }

    void HandleCheckedChanged(ToggleButton toggle)
    {
        if (toggle.Checked)
        {
            foreach (var item in _toggles)
            {
                if (item.GetInstanceID() != toggle.GetInstanceID())
                {
                    item.Checked = false;
                }
            }
        }
    }
}
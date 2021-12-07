using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditMenu : MonoBehaviour
{
    // Reference to child buttons assigned in Unity Inspector
    [SerializeField]
    private GameObject selectAllButton;
    [SerializeField]
    private GameObject deselectAllButton;

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == selectAllButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            SelectionManager.singleton.SelectAll();
        }
        else if (EventSystem.current.currentSelectedGameObject == deselectAllButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            SelectionManager.singleton.DeSelectAll();
        }
    }
}

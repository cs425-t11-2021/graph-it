// Created with help from https://forum.unity.com/threads/detect-double-click-on-something-what-is-the-best-way.476759/

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Extension to the Unity InputField class which requires a double click to start editing, rather than a single click.
public class DoubleClickInputField : TMP_InputField
{
    [SerializeField] private int requiredClicks = 2;

    public override void OnPointerClick(PointerEventData eventData) {
 
        if (eventData.clickCount == 2) {
            ActivateInputField();
        }
    }

    public override void OnSelect(BaseEventData eventData) {
        this.onSelect?.Invoke(this.text);
    }
}

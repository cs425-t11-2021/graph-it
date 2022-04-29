using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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

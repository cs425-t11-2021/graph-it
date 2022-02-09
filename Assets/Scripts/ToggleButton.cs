// Modification of code found on https://answers.unity.com/questions/314926/make-a-button-behave-like-a-toggle.html 

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ToggleButton : MonoBehaviour, IPointerClickHandler
{
    public ToggleEvent checkedChanged = new ToggleEvent();

    private Image image;
    private Color originalColor;

    private bool @checked;
    [SerializeField] private Color checkedColor;
    [SerializeField] private ToggleButtonGroup group;


    [SerializeField]
    public bool Checked
    {
        get
        {
            return this.@checked;
        }
        set
        {
            if (this.@checked != value)
            {
                this.@checked = value;
                UpdateVisual();
                this.checkedChanged.Invoke(this);
            }
        }
    }

    public void UpdateStatus(bool value) {
        this.@checked = value;
        UpdateVisual();
    }

    void Awake()
    {
        this.image = GetComponent<Image>();
        this.originalColor = this.image.color;

        if (this.group != null)
            this.group.RegisterToggle(this);
    }

    private void UpdateVisual()
    {
        this.image.color = Checked ? this.checkedColor : this.originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Checked = !Checked;
    }

    [Serializable]
    public class ToggleEvent : UnityEvent<ToggleButton>
    {
    }
}
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
    public Color originalColor;

    private bool @checked;
    [SerializeField] public Color checkedColor;
    [SerializeField] public Color highlightColor;
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

   /* //doesn't work
    //If mouse hovers over the button, highlight the button with a different color
    //referenced for onMouseHover and onMouseExit https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseOver.html
    private void OnMouseOver(){
        this.image.color = this.highlightColor;
    }

    private void onMouseExit(){
        this.image.color = this.originalColor;
    }*/
}
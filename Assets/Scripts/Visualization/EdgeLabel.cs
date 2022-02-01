using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EdgeLabel : MonoBehaviour
{
    public double weight;
    // UI Rect of the label object
    private Rect rect;
    // Store previous global position of the labelObj
    private Vector3 previousPosition;

    // Reference to the text mesh object
    TMP_InputField inputField;

    private bool displayEnabled;

    public void Initiate(double weight)
    {
        this.weight = weight;

        if (weight != 9)
        {
            inputField.text = weight.ToString();
        }

        OnToggleVertexLabels(Controller.singleton.DisplayVertexLabels);
    }

    private void Awake() {
        RectTransform rectTransform = GetComponent<RectTransform>();
        this.rect = rectTransform.rect;

        inputField = GetComponentInChildren<TMP_InputField>();

        Controller.singleton.OnToggleVertexLabels += OnToggleVertexLabels;
    }

    private void OnToggleVertexLabels(bool enabled)
    {
        this.displayEnabled = enabled;

        if (enabled)
        {
            MakeUneditable();
        }
        else
        {
            inputField.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate() {
        if (!enabled) {
            return;
        }

        // Only check to see if the label needs to move if the edge moved
        if (transform.position != previousPosition)
        {
            previousPosition = transform.position;
            transform.localPosition = FindSuitablePosition();
        }
        transform.localScale = new Vector3(0.01f / transform.parent.lossyScale.x, 0.01f / transform.parent.lossyScale.y, 1);
    }

    Vector3 FindSuitablePosition()
    {
        Vector2 vertexPos = transform.parent.position;
        Vector3 localPos = new Vector3(0.25f, 0.6f, 0);
        Collider2D col = Physics2D.OverlapArea(vertexPos + new Vector2(localPos.x - this.rect.width / 200, localPos.y + this.rect.height / 200), vertexPos + new Vector2(localPos.x + this.rect.width / 200, localPos.y - this.rect.height / 200), LayerMask.GetMask("Edge", "Vertex"));
        if (!col)
        {
            return localPos;
        }
        return new Vector3(0.25f, -0.6f, 0);
    }

    // Make the label editable
    public void MakeEditable()
    {
        if (displayEnabled)
        {
            inputField.gameObject.SetActive(true);
            inputField.interactable = true;
        }
    }

    // Make the label uneditable
    public void MakeUneditable()
    {
        if (displayEnabled)
        {
            inputField.interactable = false;
            if (string.IsNullOrEmpty(inputField.text))
            {
                inputField.gameObject.SetActive(false);
            }
            else
            {
                inputField.gameObject.SetActive(true);
            }
            transform.localPosition = FindSuitablePosition();
        }
    }

    // Update the content field with a new label
    public void UpdateLabel(string newLabel)
    {
        if (double.TryParse(newLabel, out this.weight)) {
            this.transform.parent.GetComponent<EdgeObj>().UpdateWeight(this.weight);
        }
        else {
            inputField.text = this.weight.ToString();
        }
        
    }
}

//All code developed by Team 11
using UnityEngine;
using System;
using System.Linq;
using TMPro;


public class LabelObj : MonoBehaviour
{
    //TODO: ADD COMMENTS

    public string content;

    // UI Rect of the label object
    private Rect rect;
    // Store previous global position of the labelObj
    private Vector3 previousPosition;

    // Reference to the text mesh object
    TMP_InputField inputField;

    public void Initiate(string content)
    {
        this.content = content;

        if (content != "")
        {
            inputField.text = content;
        }

        UpdatePosition();
        MakeUneditable();
    }

    private void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        this.rect = rectTransform.rect;

        inputField = GetComponentInChildren<TMP_InputField>();
        inputField.enabled = Controller.singleton.displayVertexLabels;
    }

    private void FixedUpdate()
    {
        if (!Controller.singleton.displayVertexLabels) {
            inputField.enabled = false;
            previousPosition = Vector3.negativeInfinity;
            return;
        }

        // Only check to see if the label needs to move if the vertex moved
        if (transform.position != previousPosition)
        {
            previousPosition = transform.position;
            UpdatePosition();
        }
    }

    // Updates the position of the label, moving it if needed
    public void UpdatePosition()
    {
        Nullable<Vector3> position = FindSuitablePosition();
        if (position == null)
        {
            inputField.enabled = false;
        }
        else
        {
            inputField.enabled = true;
            transform.localPosition = (Vector3) position;
        }
    }

    // This code is slow as fuck, someone try to speed it up
    Nullable<Vector3> FindSuitablePosition()
    {
        for (float radius = 0.3f; radius < 0.8f; radius += 0.1f)
        {
            for (float angle = 0f; angle <= 360f; angle += 30f)
            {
                Vector2 vertexPos = transform.parent.position;
                Vector3 localPos = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 1);
                Collider2D col = Physics2D.OverlapArea(vertexPos + new Vector2(localPos.x - this.rect.width / 200, localPos.y + this.rect.height / 200), vertexPos + new Vector2(localPos.x + this.rect.width / 200, localPos.y - this.rect.height / 200), LayerMask.GetMask("Edge", "Vertex"));
                if (!col)
                {
                    return localPos;
                }
            }
        }
        return null;
    }

    // Method called by another class to turn on or off the text label
    public void SetEnabled(bool enabled)
    {
        inputField.enabled = enabled;
    }

    // Make the label editable
    public void MakeEditable()
    {
        inputField.gameObject.SetActive(true);
        inputField.enabled = true;
        inputField.interactable = true;
    }

    // Make the label uneditable
    public void MakeUneditable()
    {
        inputField.interactable = false;
        if (string.IsNullOrEmpty(inputField.text))
        {
            inputField.gameObject.SetActive(false);
        }
        UpdatePosition();
    }

    // Update the content field with a new label
    public void UpdateLabel(string newLabel)
    {
        this.content = newLabel;
    }
}

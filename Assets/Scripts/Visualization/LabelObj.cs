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

    private bool displayEnabled;

    public void Initiate(string content)
    {
        this.content = content;

        if (content != "")
        {
            inputField.text = content;
        }

        OnToggleVertexLabels(Controller.singleton.DisplayVertexLabels);
    }

    private void Awake()
    {
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

    private void FixedUpdate()
    {
        if (!enabled) {
            return;
        }

        // Only check to see if the label needs to move if the vertex  moved
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
            inputField.gameObject.SetActive(false);
        }
        else
        {
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
            UpdatePosition();
        }
    }

    // Update the content field with a new label
    public void UpdateLabel(string newLabel)
    {
        this.content = newLabel;
    }
}

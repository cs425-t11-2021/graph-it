using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EdgeLabel : MonoBehaviour
{
    private double weight;
    // UI Rect of the label object
    private Rect rect;
    // Store previous global position of the labelObj
    private Vector3 previousPosition;

    // Reference to the text mesh object
    private TMP_InputField inputField;

    private bool displayEnabled;

    public void Initiate(double weight)
    {
        this.weight = weight;
        inputField.text = weight.ToString();

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
        // if (transform.position != previousPosition)
        // {
        //     previousPosition = transform.position;
        //     transform.localPosition = FindSuitablePosition();
        // }
        transform.position = FindSuitablePosition();
        transform.localScale = new Vector3(0.01f, 0.01f, 1);
    }

    Vector3 FindSuitablePosition()
    {
        Vector3 toPos = this.transform.parent.GetComponentInChildren<EdgeObj>().TargetVertexObj.transform.position;
        Vector3 fromPos = this.transform.parent.parent.position;

        Vector3 center = (toPos + fromPos) / 2f;
        // Vector3 pos = center + new Vector3(0f, 0.4f, 0f);
        // Collider2D col = Physics2D.OverlapArea(new Vector2(pos.x - this.rect.width / 2, pos.y + this.rect.height / 2), new Vector2(pos.x + this.rect.width / 2, pos.y - this.rect.height / 2), LayerMask.GetMask("Edge", "Vertex"));
        // if (!col)
        // {
        //     return pos;
        // }
        float angle = Mathf.Atan2(toPos.y - fromPos.y, toPos.x - fromPos.x) * Mathf.Rad2Deg;
        // Debug.Log(angle);
        // return center + new Vector3(0f, -0.4f, 0f);
        if (Mathf.Abs(angle) > 45f && Mathf.Abs(angle) < 135f) {
            return center + new Vector3(0.4f, 0f, 0f);
        }
        return center + new Vector3(0f, 0.4f, 0f);
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
        if (double.TryParse(inputField.text, out double newWeight)) {
            this.weight = newWeight;
            this.transform.parent.GetComponentInChildren<EdgeObj>().UpdateWeight(this.weight);
            inputField.text = this.weight.ToString();
        }
        else {
            inputField.text = this.weight.ToString();
        }
        
    }
}

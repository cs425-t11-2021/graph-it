using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EdgeLabelObj : MonoBehaviour
{
    // private double weight;
    // UI Rect of the label object
    private Rect rect;
    // Store previous global position of the labelObj
    private Vector3 previousPosition;

    // Reference to the text mesh object
    private TMP_InputField inputField;

    private GraphicRaycaster raycaster;

    private bool displayEnabled;

    [SerializeField] private EdgeObj edgeObject;

    public void Initiate(EdgeObj edgeObject)
    {
        this.edgeObject = edgeObject;
        UpdateEdgeLabel(edgeObject.Edge.Label, false);
        
        this.gameObject.SetActive(true);

        OnToggleVertexLabels(SettingsManager.Singleton.DisplayVertexLabels);
    }

    private void Awake() {
        RectTransform rectTransform = GetComponent<RectTransform>();
        this.rect = rectTransform.rect;
        this.inputField = GetComponentInChildren<TMP_InputField>();
        this.raycaster = GetComponent<GraphicRaycaster>();
        this.raycaster.enabled = false;

        SettingsManager.Singleton.OnToggleVertexLabels += OnToggleVertexLabels;
        this.gameObject.SetActive(false);

    }

    private void Start() {
        UpdatePosition();
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
            this.inputField.gameObject.SetActive(false);
        }
    }

    public void UpdatePosition() {
        if (!this.enabled) {
            return;
        }

        transform.position = FindSuitablePosition();
        transform.localScale = new Vector3(0.01f, 0.01f, 1);
    }

    Vector3 FindSuitablePosition()
    {
        Vector3 pos1 = this.edgeObject.Vertex1.transform.position;
        Vector3 pos2 = this.edgeObject.Vertex2.transform.position;

        Vector3 center = (pos1 + pos2) / 2f;
        float angle = Mathf.Atan2(pos1.y - pos2.y, pos1.x - pos2.x) * Mathf.Rad2Deg;
        if (Mathf.Abs(angle) > 45f && Mathf.Abs(angle) < 135f) {
            return center + new Vector3(0.4f, 0f, 1f);
        }
        return center + new Vector3(0f, 0.4f, 1f);
    }

    // Make the label editable
    public void MakeEditable()
    {
        if (displayEnabled)
        {
            inputField.gameObject.SetActive(true);
            this.raycaster.enabled = true;
        }
    }

    // Make the label uneditable
    public void MakeUneditable()
    {
        if (displayEnabled)
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                inputField.gameObject.SetActive(false);
            }
            else
            {
                inputField.gameObject.SetActive(true);
            }

            this.raycaster.enabled = false;
            UpdatePosition();
        }
    }

    // Update the content field with a new label
    public void UpdateLabel(string newLabel)
    {
        UpdateEdgeLabel(newLabel);
    }

    public void UpdateEdgeLabel(string newLabel, bool updateDS = true) {
        if (updateDS)
        {
            this.edgeObject.Edge.Label = newLabel;
            Controller.Singleton.ForceInvokeModificationEvent();
            GraphInfo.Singleton.UpdateGraphInfo();
        }

        if (this.edgeObject.Edge.Weighted)
        {
            inputField.text = this.edgeObject.Edge.Weight.ToString();
        }
        else
        {
            inputField.text = this.edgeObject.Edge.Label;
        }
        
        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(null);
        MakeUneditable();
        Logger.Log(string.Format("Edge {0} set to {1}.", this.edgeObject.Edge.Weighted ? "weight" : "label", inputField.text), this, LogType.INFO);
    }
}

//All code developed by Team 11
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

public class VertexLabelObj : MonoBehaviour
{
    //TODO: ADD COMMENTS

    private string content;

    // UI Rect of the label object
    private Rect rect;

    // Reference to the text mesh object
    TMP_InputField inputField;

    private bool displayEnabled;

    private RawImage image;
    private Texture latexTexture;
    private bool waitingForLatex = false;
    private bool latexMode = false;
    private string latexFormula = "";
    [SerializeField] private VertexObj vertexObj;
    
    public bool CenteredLabel { get; set; }

    public void Initiate(string content)
    {
        this.content = content;

        if (content != "")
        {
            UpdateLabel(content);
        }

        OnToggleVertexLabels(SettingsManager.Singleton.DisplayVertexLabels);
    }

    private void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        this.rect = rectTransform.rect;

        inputField = GetComponentInChildren<TMP_InputField>();

        SettingsManager.Singleton.OnToggleVertexLabels += OnToggleVertexLabels;

        image = this.gameObject.GetComponentInChildren<RawImage>();
        image.enabled = false;

        this.vertexObj.OnVertexObjMove += UpdatePosition;
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

    private void Update() {
        if (waitingForLatex) {
            if (this.latexTexture != null) {
                waitingForLatex = false;
                latexMode = true;
                image.rectTransform.sizeDelta = new Vector2(this.latexTexture.width, this.latexTexture.height);
                image.texture = this.latexTexture;
                if (!inputField.interactable)
                    image.enabled = true;
            }
        }
    }

    // Updates the position of the label, moving it if needed
    public void UpdatePosition()
    {
        Vector3? position = FindSuitablePosition();
        if (position == null)
        {
            inputField.gameObject.SetActive(false);
        }
        else
        {
            transform.localPosition = (Vector3) position + Vector3.forward;
        }
    }

    // This code is slow as fuck, someone try to speed it up
    Vector3? FindSuitablePosition()
    {
        if (this.CenteredLabel)
        {
            return Vector3.zero;
        }
        
        for (float radius = 0.4f; radius < 0.9f; radius += 0.1f)
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

        if (latexMode) {
            image.enabled = false;
            inputField.text = "$" + this.latexFormula + "$";
        }
    }

    // Make the label uneditable
    public void MakeUneditable()
    {
        if (displayEnabled)
        {
            if (!EventSystem.current.alreadySelecting) inputField.interactable = false;
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

        if (waitingForLatex) {
            inputField.text = " ";
        }

        if (latexMode) {
            image.enabled = true;
            inputField.text = " ";
        }
    }

    // Update the content field with a new label
    public void UpdateLabel(string newLabel)
    {
        UpdateVertexLabel(newLabel);
    }

    public void UpdateVertexLabel(string newLabel, bool updateDS = true)
    {
        if (newLabel.StartsWith("$") && newLabel.EndsWith("$")) {
            if (latexMode) {
                latexMode = false;
                this.latexTexture = null;
                image.texture = null;
            }

            string formula = newLabel.Substring(1, newLabel.Length - 2);
            this.content = " ";
            this.inputField.text = newLabel;
            this.latexFormula = formula;

            waitingForLatex = true;
            LatexRenderer.Singleton.AddToLatexQueue(formula, result => this.latexTexture = result);

            Logger.Log("Vertex label changed to " + formula, this, LogType.INFO);
            
            if (updateDS)
                this.vertexObj.Vertex.Label = newLabel;
        }
        else {
            latexMode = false;
            waitingForLatex = false;

            image.texture = null;
            this.latexTexture = null;
            image.enabled = false;
            this.content = newLabel;
            this.inputField.text = newLabel;

            Logger.Log("Vertex label changed to " + newLabel, this, LogType.INFO);
            
            if (updateDS)
                this.vertexObj.Vertex.Label = newLabel;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabelObj : MonoBehaviour
{
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
    
    public void Initiate(string content)
    {
        this.content = content;

        if (content != "")
        {
            inputField.text = content;
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
            inputField.text = "$$" + this.latexFormula + "$$";
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
        if (newLabel.StartsWith("$") && newLabel.EndsWith("$")) {
            if (latexMode) {
                latexMode = false;
                this.latexTexture = null;
                image.texture = null;
            }

            string formula = newLabel.Substring(1, newLabel.Length - 2);
            this.content = " ";
            inputField.text = newLabel;
            this.latexFormula = formula;

            waitingForLatex = true;
            // StartCoroutine(LatexRenderer.Singleton.GetLatexTexture(formula, result => this.latexTexture = result));

            Logger.Log("Label changed to " + formula, this, LogType.INFO);
        }
        else {
            latexMode = false;
            waitingForLatex = false;

            image.texture = null;
            this.latexTexture = null;
            image.enabled = false;
            this.content = newLabel;

            Logger.Log("Label changed to " + newLabel, this, LogType.INFO);
        }
    }
}

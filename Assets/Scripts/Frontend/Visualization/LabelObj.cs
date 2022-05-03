using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Function for implementing graph labels that are independent of both edges and vertices. This feature is currently not finished
// and graph labels are not currently enabled.
public class LabelObj : MonoBehaviour
{
    private string content;

    // UI Rect of the label object
    private Rect rect;

    // Reference to the text mesh object
    TMP_InputField inputField;
    private SpriteRenderer spriteRenderer;

    private bool displayEnabled;

    private RawImage image;
    private Texture latexTexture;
    private bool waitingForLatex = false;
    private bool latexMode = false;
    private string latexFormula = "";
    
    // The distance between the starting position of the vertex and the cursor world position
    private Vector3 cursorOffset;
    
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
        RectTransform rectTransform = GetComponentInChildren<RectTransform>();
        this.rect = rectTransform.rect;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        inputField = GetComponentInChildren<TMP_InputField>();

        SettingsManager.Singleton.OnToggleVertexLabels += OnToggleVertexLabels;

        image = this.gameObject.GetComponentInChildren<RawImage>(true);
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
            spriteRenderer.enabled = true;
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

            spriteRenderer.enabled = false;
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

    private void OnMouseDown()
    {
        Debug.Log("test");
        this.cursorOffset = this.transform.position - InputManager.Singleton.CursorWorldPosition;
    }

    private void OnMouseDrag()
    {
        this.transform.position = InputManager.Singleton.CursorWorldPosition + this.cursorOffset;
    }

    public void ShowMover()
    {
        this.GetComponent<CircleCollider2D>().enabled = true;
        this.spriteRenderer.enabled = true;
    }

    public void HideMover()
    {
        this.GetComponent<CircleCollider2D>().enabled = false;
        this.spriteRenderer.enabled = false;
    }
}

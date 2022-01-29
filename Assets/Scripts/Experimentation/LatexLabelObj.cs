using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LatexLabelObj : MonoBehaviour
{
    private RawImage image;

    public string formula;
    private Texture latexTexture;

    private void Awake() {
        image = this.gameObject.GetComponentInChildren<RawImage>();
        image.enabled = false;
    }

    private void Start() {
        if (this.formula != "") {
            StartCoroutine(LatexRenderer.singleton.GetLatexTexture(this.formula, result => this.latexTexture = result));
        }
    }

    private void Update() {
        Debug.Log(this.latexTexture);
        if (this.latexTexture != null) {
            Debug.Log("Setting new texture");
            image.rectTransform.sizeDelta = new Vector2(this.latexTexture.width, this.latexTexture.height);
            image.texture = this.latexTexture;
            image.enabled = true;
        }
    }
}

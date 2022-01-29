using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LatexRenderer : MonoBehaviour
{
    private Texture latexTexture = null;
    private string formula = "y=\\frac{a}{b}";

    private void Start() {
        StartCoroutine(GetLatexTexture());
    }

    IEnumerator GetLatexTexture() {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://latex.codecogs.com/png.image?" + formula);
        yield return www.SendWebRequest();
        latexTexture = DownloadHandlerTexture.GetContent(www);
        Debug.Log(latexTexture);

    }

    void OnGUI() {
        if(latexTexture != null)
            GUI.DrawTexture(new Rect(5,5,latexTexture.width,latexTexture.height), latexTexture);
    }
}

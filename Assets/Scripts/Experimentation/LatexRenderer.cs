using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class LatexRenderer : SingletonBehavior<LatexRenderer>
{
    private Texture latexTexture = null;

    public static LatexRenderer singleton;

    private void Start() {
        // StartCoroutine(GetLatexTexture());
    }

    // IEnumerator GetLatexTexture() {
    //     UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://latex.codecogs.com/png.image?" + this.formula);
    //     yield return www.SendWebRequest();
    //     latexTexture = DownloadHandlerTexture.GetContent(www);
    //     Debug.Log(latexTexture);

    // }

    // void OnGUI() {
    //     if(latexTexture != null)
    //         GUI.DrawTexture(new Rect(5,5,latexTexture.width,latexTexture.height), latexTexture);
    // }

    public IEnumerator GetLatexTexture(string formula, Action<Texture> textureAction) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://latex.codecogs.com/png.image?" + formula);
        yield return www.SendWebRequest();
        textureAction(DownloadHandlerTexture.GetContent(www));
        // Debug.Log(texture);
        Debug.Log("[LatexRenderer] Latex texture for " + formula + " received");
    }
}
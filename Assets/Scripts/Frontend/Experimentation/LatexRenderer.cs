using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class LatexRenderer : SingletonBehavior<LatexRenderer>
{
    private Texture latexTexture = null;

    public IEnumerator GetLatexTexture(string formula, Action<Texture> textureAction) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://latex.codecogs.com/png.image?" + formula);
        yield return www.SendWebRequest();
        textureAction(DownloadHandlerTexture.GetContent(www));
        Logger.Log("Latex texture for " + formula + " received", this, LogType.INFO);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class LatexRenderer : SingletonBehavior<LatexRenderer>
{
    private Texture latexTexture = null;

    private Queue<(string formula, Action<Texture> textureAction)> latexRequestQueue;

    private void Awake()
    {
        latexRequestQueue = new Queue<(string formula, Action<Texture> textureAction)>();
    }

    private void Start()
    {
        StartCoroutine(GetLatexTexture());
    }

    public void AddToLatexQueue(string formula, Action<Texture> textureAction)
    {
        latexRequestQueue.Enqueue((formula, textureAction));
    }

    private IEnumerator GetLatexTexture() {
        while (true) {
            if (latexRequestQueue.Count > 0)
            {
                (string formula, Action<Texture> textureAction) = latexRequestQueue.Dequeue();
                UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://latex.codecogs.com/png.image?" + formula);
                yield return www.SendWebRequest();
                textureAction(DownloadHandlerTexture.GetContent(www));
                Logger.Log("Latex texture for " + formula + " received", this, LogType.INFO);
            }

            yield return null;
        }
    }
}

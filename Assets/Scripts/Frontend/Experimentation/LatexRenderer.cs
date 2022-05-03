// Using API from latex.codecogs.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Experimental class which handles the retreival of Latex images given a Latex formula using the CodeCogs API. Requires an
// internet connection to function. 
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
    
    // Add latex formula to a queue to be retreived
    public void AddToLatexQueue(string formula, Action<Texture> textureAction)
    {
        latexRequestQueue.Enqueue((formula, textureAction));
    }
    
    // Async method to download a latex image from CodeCogs and render it as a texture.
    private IEnumerator GetLatexTexture() {
        while (true) {
            if (latexRequestQueue.Count > 0)
            {
                (string formula, Action<Texture> textureAction) = latexRequestQueue.Dequeue();
                UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://latex.codecogs.com/png.image?" + formula);
                www.timeout = 5;
                yield return www.SendWebRequest();
                
                while(!www.isDone) {
                    yield return null;
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Logger.Log(www.error, this, LogType.ERROR);
                }

                textureAction(DownloadHandlerTexture.GetContent(www));
                Logger.Log("Latex texture for " + formula + " received", this, LogType.INFO);
            }

            yield return null;
        }
    }
}

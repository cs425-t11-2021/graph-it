// Modification of CodeMonkey Tutorial on Youtube
// https://youtu.be/lT-SRLKUe5k

using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
     // Singleton
    public static ScreenshotManager singleton;

    // Component references
    private Camera cam;

    // If true, take screenshot next frame
    private bool takeScreenshotNextFrame;
    // Filepath for the image
    private string filepath;

    private void Awake()
    {
        // Singleton pattern setup
        if (singleton == null) {
            singleton = this;
        }
        else {
            Debug.LogError("[ScreenshotManager] Singleton pattern violation");
            Destroy(this);
            return;
        }

        this.cam = GetComponent<Camera>();
    }

    private void Update() {
        // Tempoary testing code to take a screenshot
        if (Input.GetKeyDown(KeyCode.Minus)) {
            TakeScreenshot(Screen.width, Screen.height, System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/GraphImgs/test.png");
        }
    }

    private void OnPostRender() {
        if (takeScreenshotNextFrame) {
            RenderTexture renderTex = this.cam.targetTexture;
            Texture2D renderResult = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTex.width, renderTex.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            File.WriteAllBytes(filepath, byteArray);

            RenderTexture.ReleaseTemporary(renderTex);
            this.cam.targetTexture = null;

            this.takeScreenshotNextFrame = false;
            this.filepath = null;
        }
    }

    private void TakeScreenshot(int width, int height, string filepath) {
        this.takeScreenshotNextFrame = true;
        this.cam.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        this.filepath = filepath;
    }
}

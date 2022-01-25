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
    // Current screen bounds of graph objects
    private Bounds screenBounds;

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
            TakeScreenshot(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/GraphImgs/test.png");
        }
    }

    private void OnPostRender() {
        if (this.takeScreenshotNextFrame) {
            RenderTexture renderTex = this.cam.targetTexture;
            Texture2D renderResult = new Texture2D((int) (this.screenBounds.size.x + 1), (int) (this.screenBounds.size.y + 1), TextureFormat.ARGB32, false);

            Rect rect = new Rect(this.screenBounds.min.x, this.screenBounds.min.y, this.screenBounds.size.x, this.screenBounds.size.y);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            File.WriteAllBytes(filepath, byteArray);

            RenderTexture.ReleaseTemporary(renderTex);
            this.cam.targetTexture = null;

            this.takeScreenshotNextFrame = false;
            this.filepath = null;
        }
    }

    private void TakeScreenshot(string filepath) {
        this.screenBounds = GetBoundsOfGraphObjects();
        this.takeScreenshotNextFrame = true;
        this.cam.targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
        this.filepath = filepath;
    }

    private Bounds GetBoundsOfGraphObjects() {
        Bounds bounds = new Bounds(Vector2.zero, Vector2.zero);

        Transform ObjContainer = Controller.singleton.graphObj;
        float xMin = 0;
        float xMax = 0;
        float yMin = 0;
        float yMax = 0;

        for (int i = 0; i < ObjContainer.childCount; i++) {
            Transform vertex = ObjContainer.GetChild(i);

            if (vertex.position.x < xMin) {
                xMin = vertex.position.x;
            }
            if (vertex.position.x > xMax) {
                xMax = vertex.position.x;
            }
            if (vertex.position.y < yMin) {
                yMin = vertex.position.y;
            }
            if (vertex.position.y > yMax) {
                yMax = vertex.position.y;
            }
        }

        bounds.SetMinMax(cam.WorldToScreenPoint(new Vector3(xMin - 1, yMin - 1, 0)), cam.WorldToScreenPoint(new Vector3(xMax + 1, yMax + 1, 0)));

        return bounds;
    }
}

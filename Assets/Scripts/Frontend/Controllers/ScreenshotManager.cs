// Modification of CodeMonkey Tutorial on Youtube
// https://youtu.be/lT-SRLKUe5k

using System.IO;
using UnityEngine;

// Class responsible for creating savable images from the current graph
public class ScreenshotManager : SingletonBehavior<ScreenshotManager>
{
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
        this.cam = GetComponent<Camera>();
    }

    // Screenshot code needs to be run in postrender
    private void OnPostRender() {
        if (this.takeScreenshotNextFrame) {
            // Render the camera into a texture2d
            RenderTexture renderTex = this.cam.targetTexture;
            Texture2D renderResult = new Texture2D((int) (this.screenBounds.size.x), (int) (this.screenBounds.size.y), TextureFormat.ARGB32, false);
            Rect rect = new Rect(this.screenBounds.min.x, this.screenBounds.min.y, this.screenBounds.size.x, this.screenBounds.size.y);
            renderResult.ReadPixels(rect, 0, 0);

            // Write the texture data as a png
            byte[] byteArray = renderResult.EncodeToPNG();
            File.WriteAllBytes(filepath, byteArray);

            RenderTexture.ReleaseTemporary(renderTex);
            this.cam.targetTexture = null;

            this.takeScreenshotNextFrame = false;
            this.filepath = null;
        }
    }

    // Tempoarty function for saving a screenshot to desktop
    public void SaveScrenshotToDesktop() {
        TakeScreenshot(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/graph_img.png");
    }

    private void TakeScreenshot(string filepath) {
        this.screenBounds = GetBoundsOfGraphObjects();
        this.takeScreenshotNextFrame = true;
        this.cam.targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
        this.filepath = filepath;
    }

    // Helper function for getting a bound around all graph objects active in the scene
    private Bounds GetBoundsOfGraphObjects() {
        Bounds bounds = new Bounds(Vector2.zero, Vector2.zero);

        Transform objContainer = Controller.Singleton.GraphObjContainer;
        float xMin = float.PositiveInfinity;
        float xMax = float.NegativeInfinity;
        float yMin = float.PositiveInfinity;
        float yMax = float.NegativeInfinity;

        for (int i = 0; i < objContainer.childCount; i++) {
            Transform vertex = objContainer.GetChild(i);

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

        bounds.SetMinMax(cam.WorldToScreenPoint(new Vector3(xMin - 2, yMin - 2, cam.transform.position.z)), cam.WorldToScreenPoint(new Vector3(xMax + 2, yMax + 2, cam.transform.position.z)));

        return bounds;
    }
}

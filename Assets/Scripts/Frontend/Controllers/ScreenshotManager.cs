// Received help from the following tutorials:
// https://youtu.be/lT-SRLKUe5k
// https://forum.unity.com/threads/rendering-screenshot-larger-than-screen-resolution.254760/

using System.Collections;
using System.IO;
using UnityEngine;

// Class responsible for creating savable images from the current graph.
public class ScreenshotManager : SingletonBehavior<ScreenshotManager>
{
    // Reference to the screenshot camera
    private Camera cam;

    private void Awake()
    {
        // Get reference to the SC camera
        this.cam = GetComponent<Camera>();
        // Disable it as it is only used for taking screensthos
        this.cam.enabled = false;
    }

    public void TakeScreenshot(string filepath) {
        Bounds worldBounds = GetBoundsOfGraphObjects();
        Bounds screenBounds = new Bounds();
        screenBounds.SetMinMax(Camera.main.WorldToScreenPoint(worldBounds.min), Camera.main.WorldToScreenPoint(worldBounds.max));

        StartCoroutine(RenderCameraView(worldBounds, screenBounds, filepath));
    }

    private IEnumerator RenderCameraView(Bounds worldBounds, Bounds screenBounds, string filepath)
    {
        // Wait until everything is rendered
        yield return new WaitForEndOfFrame();

        // Render the camera into a texture2d
        Logger.Log("Taking screenshot of scene.", this, LogType.INFO);

        this.cam.targetTexture = RenderTexture.GetTemporary((int) (screenBounds.size.x + .5f), (int) (screenBounds.size.y + .5f), 24);
        Texture2D renderResult = new Texture2D((int) (screenBounds.size.x + .5f), (int) (screenBounds.size.y + .5f), TextureFormat.ARGB32, false);

        this.cam.orthographicSize = worldBounds.size.y / 2f;
        this.cam.transform.position = worldBounds.center;
        this.cam.aspect = screenBounds.size.x / screenBounds.size.y;
        this.cam.Render();
        RenderTexture.active = this.cam.targetTexture;

        renderResult.ReadPixels(new Rect(0, 0, screenBounds.size.x, screenBounds.size.y), 0, 0);

        // Write the texture data as a png
        byte[] byteArray = renderResult.EncodeToPNG();
        File.WriteAllBytes(filepath, byteArray);

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(this.cam.targetTexture);
        this.cam.targetTexture = null;

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

        bounds.SetMinMax(new Vector3(xMin - 3, yMin - 3 , this.cam.transform.position.z), new Vector3(xMax + 3 , yMax + 3, this.cam.transform.position.z));
        return bounds;
    }
}

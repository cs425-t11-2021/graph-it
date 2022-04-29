using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : SingletonBehavior<ResourceManager>
{

    public Sprite[] vertexSprites;

    private void Awake()
    {
        // LoadVertexSprites();
    }

    public void LoadVertexSprites()
    {
        List<Sprite> vertexSprites = new List<Sprite>();

        foreach (Sprite builtIn in SettingsManager.Singleton.vertexSprites)
        {
            vertexSprites.Add(builtIn);
        }
        
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "Vertex/"));
        FileInfo[] vertexFiles = directoryInfo.GetFiles();

        foreach (FileInfo file in vertexFiles)
        {
            if (file.Extension == ".png")
            {
                //Converts desired path into byte array
                byte[] pngBytes = File.ReadAllBytes(file.FullName);
 
                //Creates texture and loads byte array data to create image
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(pngBytes);
 
                //Creates a new Sprite based on the Texture2D
                Sprite fromTex = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                vertexSprites.Add(fromTex);
            }
        }

        this.vertexSprites = vertexSprites.ToArray();
    }
}

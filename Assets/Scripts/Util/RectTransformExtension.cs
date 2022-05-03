// from https://answers.unity.com/questions/1013011/convert-recttransform-rect-to-screen-space.html

using UnityEngine;

// Extension to the Unity RectTransform class that adds a function to return a rect in screen coordinates.
public static class RectTransformExtension
{
    public static Rect GetScreenRect(this RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * transform.pivot), size);
    }
}

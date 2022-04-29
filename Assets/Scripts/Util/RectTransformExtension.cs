using UnityEngine;

// https://answers.unity.com/questions/1013011/convert-recttransform-rect-to-screen-space.html

public static class RectTransformExtension
{
    public static Rect GetScreenRect(this RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * transform.pivot), size);
    }
}

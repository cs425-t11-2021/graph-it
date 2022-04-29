using UnityEngine;

public static class Collider2DExtension
{
    public static CircleCollider2D CopyFromCollider(this CircleCollider2D self, CircleCollider2D other)
    {
        self.radius = other.radius;
        self.offset = other.offset;
        return self;
    }
    
    public static PolygonCollider2D CopyFromCollider(this PolygonCollider2D self, PolygonCollider2D other)
    {
        Vector2[] points = new Vector2[other.points.Length];
        other.points.CopyTo(points, 0);
        self.points = points;
        self.offset = other.offset;

        self.pathCount = other.pathCount;
        for (int i = 0; i < other.pathCount; i++)
        {
            self.SetPath(i, other.GetPath(i));
        }
        
        return self;
    }
}

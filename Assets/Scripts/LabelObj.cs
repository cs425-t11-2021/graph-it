using UnityEngine;
using System;
using System.Linq;
using TMPro;


public class LabelObj : MonoBehaviour
{
    //TODO: ADD COMMENTS

    public string content;

    // UI Rect of the label object
    private Rect rect;
    // Store previous global position of the labelObj
    private Vector3 previousPosition;

    // Reference to the text mesh object
    TMP_Text textMesh;

    public void Initiate(string content)
    {
        this.content = content;
    }

    private void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        this.rect = rectTransform.rect;

        textMesh = GetComponent<TMP_Text>();
        textMesh.text = content;

        // TODO: REMOVE TEST CODE!!!
        // TEST CODE: assign random labels for now
        textMesh.text = string.Join("", Enumerable.Repeat(0, 10).Select(n => (char) UnityEngine.Random.Range(48, 123)));

        textMesh.enabled = Controller.singleton.displayVertexLabels;
    }

    private void FixedUpdate()
    {
        if (!Controller.singleton.displayVertexLabels) {
            textMesh.enabled = false;
            previousPosition = Vector3.negativeInfinity;
            return;
        }

        // Only check to see if the label needs to move if the vertex moved
        if (transform.position != previousPosition)
        {
            previousPosition = transform.position;
            Nullable<Vector3> position = FindSuitablePosition();
            if (position == null)
            {
                textMesh.enabled = false;
            }
            else
            {
                textMesh.enabled = true;
                transform.localPosition = (Vector3) position;
            }
        }
    }

    // This code is slow as fuck, someone try to speed it up
    Nullable<Vector3> FindSuitablePosition()
    {
        for (float radius = 0.3f; radius < 0.8f; radius += 0.1f)
        {
            for (float angle = 0f; angle <= 360f; angle += 30f)
            {
                Vector2 vertexPos = transform.parent.position;
                Vector3 localPos = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 1);
                Collider2D col = Physics2D.OverlapArea(vertexPos + new Vector2(localPos.x - this.rect.width / 2, localPos.y + this.rect.height / 2), vertexPos + new Vector2(localPos.x + this.rect.width / 2, localPos.y - this.rect.height / 2), LayerMask.GetMask("Edge"));
                if (!col)
                {
                    return localPos;
                }
            }
        }
        return null;
    }

    // Method called by another class to turn on or off the text label
    public void SetEnabled(bool enabled)
    {
        textMesh.enabled = enabled;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        DrawRect(this.rect);

    }
    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(transform.parent.position.x + rect.center.x, transform.parent.position.y + rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }
}

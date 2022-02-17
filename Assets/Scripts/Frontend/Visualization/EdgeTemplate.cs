using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeTemplate : MonoBehaviour
{
    private Vector2 startingPoint;
    private Transform arrow;
    private SpriteRenderer arrowSpriteRenderer;
    public bool Directed {get; set;}

    public void Initiate(Vector2 startingPoint) {
        this.startingPoint = startingPoint;
    }

    private void Awake() {
        this.arrow = this.transform.GetChild(0);
        this.arrowSpriteRenderer = this.arrow.GetComponent<SpriteRenderer>();
    }

    private void Update() {
        StretchBetweenPoints(startingPoint, InputManager.Singleton.CursorWorldPosition);
    }

    // Method to stretch the edge so it extends from one point to another 
    private void StretchBetweenPoints(Vector2 point1, Vector2 point2)
    {
        this.transform.position = point1;
        Vector2 dir = point2 - point1;
        this.transform.localScale = new Vector3(dir.magnitude * 2, transform.localScale.y, 1);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (this.Directed) {
            this.arrow.localPosition = new Vector3((this.Directed ? 0.5f : 0f) - (this.arrowSpriteRenderer.size.x / this.transform.localScale.x), 0f, 0f);
            this.arrow.localScale = new Vector3(1f / this.transform.lossyScale.x, 1f / (this.transform.lossyScale.y - 0.1f), 1);
            this.arrow.localRotation = Quaternion.AngleAxis(this.Directed ? 0f : 180f, Vector3.forward);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }
    }
}

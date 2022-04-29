using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeTemplate : MonoBehaviour
{
    [SerializeField] private Transform arrow;
    private Vector2 startingPoint;
    private SpriteRenderer arrowSpriteRenderer;
    public bool Directed {get; set;}

    public void Initiate(Vector2 startingPoint) {
        this.startingPoint = startingPoint;
    }

    private void Awake() {
        this.arrowSpriteRenderer = this.arrow.GetComponent<SpriteRenderer>();
    }

    private void Update() {
        StretchBetweenPoints(startingPoint, InputManager.Singleton.CursorWorldPosition);
    }

    // Method to stretch the edge so it extends from one point to another 
    private void StretchBetweenPoints(Vector2 point1, Vector2 point2)
    {
        Vector2 distance = point2 - point1;
        this.transform.localScale = new Vector3(distance.magnitude * 2 - 2 * (this.Directed ? this.arrowSpriteRenderer.size.x * .95f : 0f), (1 + 1) * 0.05f * 2f, 1);
        this.transform.parent.position = point1;
        // this.transform.position = point1;
        
        float angle = Mathf.Atan2(distance.normalized.y, distance.normalized.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (this.Directed) {
            this.arrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            this.arrow.localPosition = distance * (1f - (this.arrowSpriteRenderer.size.x) / distance.magnitude) ;
            this.arrow.localScale = new Vector3(1f, (.5f + (1f + 1f) * (.25f)), 1f);
            this.arrow.gameObject.SetActive(true);
        }
        else {
            this.arrow.gameObject.SetActive(false);
        }
    }
}

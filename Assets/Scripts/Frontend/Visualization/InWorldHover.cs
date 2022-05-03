using UnityEngine;

// In-world version of the OnHover UI label class.
public class InWorldHover : MonoBehaviour
{
    [SerializeField] private GameObject labelPrefab;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 size;
    public string Label { get; set; }

    private OnHoverLabel newLabel;
    
    private void OnMouseEnter()
    {
        if (!this.enabled) return;
       
        this.newLabel = Instantiate(labelPrefab, UIManager.Singleton.worldCanvas.transform).GetComponent<OnHoverLabel>();
        this.newLabel.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
        this.newLabel.CreateLabel(this.Label, this.size);
        this.newLabel.transform.SetAsLastSibling();
        this.newLabel.transform.position = new Vector3(InputManager.Singleton.CursorWorldPosition.x, InputManager.Singleton.CursorWorldPosition.y, -5f) + this.offset;
    }

    private void OnMouseExit()
    {
        if (this.newLabel != null) {
            Destroy(this.newLabel.gameObject);
        }
    }

    private void OnDisable() {
        if (this.newLabel != null) {
            Destroy(this.newLabel.gameObject);
        }
    }
}

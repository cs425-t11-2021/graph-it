using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    private TMP_Text textMesh;
    private float durationRemaining = 999f;
    private bool alwaysOnBottom = false;

    private void Awake() {
        this.textMesh = GetComponentInChildren<TMP_Text>();
    }

    public Notification Initiate(string content, float duration) {
        this.textMesh.text = content;
        this.durationRemaining = duration;
        return this;
    }

    public Notification Initiate(string content) {
         this.textMesh.text = content;
         this.durationRemaining = float.PositiveInfinity;
         this.alwaysOnBottom = true;
         Animator anim = GetComponent<Animator>();
         anim.speed = 0f;
         anim.Play("NotificationEnter", 0, 1f);
         return this;
    }

    private void Update() {
        if (durationRemaining > 0f) {
            this.durationRemaining -= Time.deltaTime;

            if (this.alwaysOnBottom)
            {
                this.transform.SetAsLastSibling();
            }
        }
        else {
            Destroy(this.gameObject);
            return;
        }
        
    }
}

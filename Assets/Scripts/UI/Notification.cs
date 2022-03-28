using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Notification : MonoBehaviour
{
    private TMP_Text textMesh;
    private float durationRemaining = 999f;
    private Func<bool> predicate = null;

    private void Awake() {
        this.textMesh = GetComponentInChildren<TMP_Text>();
    }

    public void Initiate(string content, float duration) {
        this.textMesh.text = content;
        this.durationRemaining = duration;
    }

    public void Initiate(string content, Func<bool> predicate) {
         this.textMesh.text = content;
         this.predicate = predicate;
    }

    private void Update() {
        if (predicate == null) {
            if (durationRemaining > 0f) {
                this.durationRemaining -= Time.deltaTime;
            }
            else {
                Destroy(this.gameObject);
                return;
            }
        }
        else {
            if (predicate()) {
                Destroy(this.gameObject);
                return;
            }
        }
    }
}

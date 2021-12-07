using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphInfo : MonoBehaviour
{
    // Singleton
    public static GraphInfo singleton;

    [SerializeField]
    private TMP_Text chromatic_text;
    [SerializeField]
    private TMP_Text bipartite_text;

    [SerializeField]
    private Button prim_button;

    private void Awake() {
        // Singleton pattern setup
        if (singleton == null) {
            singleton = this;
        }
        else {
            Debug.LogError("[GraphInfo] Singleton pattern violation");
            Destroy(this);
            return;
        }

        prim_button.interactable = false;
    }

    private void FixedUpdate() {
        // Only allow the prim button to be pressed if there is exactly one vertex selected
        if (SelectionManager.singleton.SelectedVertexCount() == 1 && SelectionManager.singleton.SelectedEdgeCount() == 0) {
            prim_button.interactable = true;
        }   
        else {
            prim_button.interactable = false;
        }
    }

    public void UpateGraphInfo() {
        if (Controller.singleton.graph.vertices.Count > 6) {
            chromatic_text.text = "Chromatic Number: TMV";
            bipartite_text.text = "Bipartite: TMV";
        }
        else {
            int chromatic_num = Controller.singleton.graph.GetChromaticNumber();
            chromatic_text.text = "Chromatic Number: " + chromatic_num;
            bipartite_text.text = "Bipartite: " + (chromatic_num == 2 ? "Yes" : "No");
        }
    }
}

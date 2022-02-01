//All code developed by Team 11
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

    private ChromaticAlgorithm chromaticAlgorithm;

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

        chromaticAlgorithm = new ChromaticAlgorithm( Controller.singleton.Graph );
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
        this.chromaticAlgorithm.Run();
    }

    public void updateChromaticNumber(int chromatic_number) {
        chromatic_text.text = "Chromatic Number: " + chromatic_number;
    
    }

    public void test(int a) {
        Debug.Log(a);
    }
}

//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = System.Object;

public enum ResultType { EdgeList, VertexList }

[Serializable]
public struct DisplayAlgorithmExtraInfo
{
    public string lead;
    public string getInfoMethod;
}

[Serializable]
public class GraphDisplayAlgorithmAssociation
{
    public string algorithmClass = "";
    public string displayName = "";
    public bool enabled = false;
    public int requiredVertices = 0;
    public string activationMethod = "";
    public string completedMethod = "";
    public ResultType resultType;
    public DisplayAlgorithmExtraInfo[] extraInfo;
    
    [HideInInspector] public ToggleButton activationButton;

    public Action<Vertex[]> OnCompleteUpdateDisplay
    {
        get
        {
            return (vertexParms) =>
            {
                object result = Type.GetType("AlgorithmManager").GetMethod(completedMethod)
                    .Invoke(Controller.Singleton.AlgorithmManager, (Object[]) vertexParms);
                
                if (result == null)
                {
                    Logger.Log("Graph display algorithm " + algorithmClass + " returned a null result.", this, LogType.ERROR);
                    NotificationManager.Singleton.CreateNotification(string.Format("<color=red>{0} returned a null result.</color>", algorithmClass), 3);
                }
                else
                {                    
                    AlgorithmsPanel.Singleton.StoreAlgorithmResult(this.algorithmClass, result);
                    if (AlgorithmsPanel.Singleton.CurrentlySelectedAlgorithm == this) {
                        AlgorithmsPanel.Singleton.AlgorithmResult = result;
                    }

                    NotificationManager.Singleton.CreateNotification("<#0000FF>" + this.algorithmClass + "</color> finished.", 3);
                }
            };
        }
    }
}

public class AlgorithmsPanel : SingletonBehavior<AlgorithmsPanel>
{
    [SerializeField] private GraphDisplayAlgorithmAssociation[] associations;
    
    [SerializeField] private Button algOpenPanel;
    //Reference to the button to open the algorithm info panels

    [SerializeField] public ToggleButton resultButton;
    [SerializeField] public ToggleButton runButton;
    [SerializeField] private Color deafultColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color defaultFinishedColor;
    [SerializeField] private Color selectedFinishedColor;
    [SerializeField] private Transform algorithmButtonHolder;
    [SerializeField] private GameObject algorithmToggleButtonPrefab;
    
    // TODO: get rid of Eventaully
    [SerializeField] private GameObject deprecationWarning;

    public bool StepByStep { get; set; } = true;

    public GraphDisplayAlgorithmAssociation CurrentlySelectedAlgorithm {get; private set;}

    public object AlgorithmResult {get; set;}

    public object[] algorithmResults;


    // Property for whether or not the algorithm buttons are enabled
    public bool AlgorithmButtonsEnabled
    {
        get; set;
    }

    private void Awake() {
        this.deprecationWarning?.SetActive(false);
        
        Logger.Log("Loading currently enabled graph display algorithms.", this, LogType.INFO);
        NotificationManager.Singleton.CreateNotification("Loading currently enabled algorithms.", 3);
        Array.ForEach(this.associations, a =>
        {
            // Create a new toggle button for each association
            ToggleButton newAlgoButton = Instantiate(algorithmToggleButtonPrefab, algorithmButtonHolder).GetComponent<ToggleButton>();
            newAlgoButton.checkedColor = this.selectedColor;
            newAlgoButton.originalColor = this.deafultColor;
            newAlgoButton.GetComponent<Image>().color = this.deafultColor;
            newAlgoButton.checkedChanged.AddListener(delegate { SelectAlgorithm(a.algorithmClass); });
            newAlgoButton.GetComponentInChildren<TMP_Text>(true).text = a.displayName;
            
            a.activationButton = newAlgoButton;
            a.activationButton.UpdateStatus(false);
            a.activationButton.gameObject.SetActive(a.enabled);
        });
        this.resultButton.gameObject.SetActive(false);

        algorithmResults = new object[this.associations.Length];

        Controller.Singleton.OnGraphModified += ClearAlgorithmResults;
        Controller.Singleton.OnInstanceChanged += (newInstance) => ClearAlgorithmResults();

        this.resultButton.gameObject.SetActive(false);
    }

    public void UpdateGraphDisplayResults(Algorithm algorithm, Vertex[] vertexParms, AlgorithmManager algoMan)
    {
        // Fix for #126
        if (algoMan != Controller.Singleton.AlgorithmManager)
        {
            return;
        }
        
        string algorithmName = algorithm.GetType().ToString();

        foreach (GraphDisplayAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                association.OnCompleteUpdateDisplay(vertexParms);
                return;
            }
        }        
    }

    public void RunGraphDisplayAlgorithm(GraphDisplayAlgorithmAssociation association)
    {
        if (association.requiredVertices > 0)
        {
            Object[] vertices = new Object[association.requiredVertices + 1];
            vertices[association.requiredVertices] = (Object) true;
            for (int i = 0; i < association.requiredVertices; i++)
            {
                vertices[i] = SelectionManager.Singleton.SelectedVertices[i].Vertex;
            }
            Type.GetType("AlgorithmManager").GetMethod(association.activationMethod).Invoke(Controller.Singleton.AlgorithmManager, vertices);
        }
        else
        {
            Type.GetType("AlgorithmManager").GetMethod(association.activationMethod).Invoke(Controller.Singleton.AlgorithmManager, new Object[] {true});
        }
    }

    public void SelectAlgorithm(string algorithmName) {
        foreach (GraphDisplayAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                if (this.CurrentlySelectedAlgorithm != association) {
                    this.CurrentlySelectedAlgorithm = association;
                    association.activationButton.UpdateStatus(true);
                }
                else {
                    this.CurrentlySelectedAlgorithm = null;
                    association.activationButton.UpdateStatus(false);
                }

                int index = Array.IndexOf(this.associations, association);
                if (this.algorithmResults[index] != null) {
                    this.resultButton.gameObject.SetActive(true);
                    this.AlgorithmResult = this.algorithmResults[index];
                }
                else {
                    this.resultButton.gameObject.SetActive(false);
                    this.AlgorithmResult = null;
                }
            }
            else {
                association.activationButton.UpdateStatus(false);
            }
        }

        this.resultButton.Checked = false;
    }

    public void StoreAlgorithmResult(string algorithmName, object result) {
        foreach (GraphDisplayAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass == algorithmName)
            {
                int index = Array.IndexOf(this.associations, association);
                this.algorithmResults[index] = result;
                association.activationButton.checkedColor = this.selectedFinishedColor;
                association.activationButton.originalColor = this.defaultFinishedColor;
                association.activationButton.GetComponent<Image>().color = this.defaultFinishedColor;
                association.activationButton.UpdateStatus(association.activationButton.Checked);

                if (CurrentlySelectedAlgorithm == association) {
                    this.resultButton.gameObject.SetActive(true);
                }

                return;
            }
        }
    }

    public void DeselectAllAlgorithms() {
        foreach (GraphDisplayAlgorithmAssociation association in this.associations) {
            association.activationButton.UpdateStatus(false);
        }
        this.CurrentlySelectedAlgorithm = null;
    }

    public void StartAlgorithmInitiation() {
        if (this.CurrentlySelectedAlgorithm != null) {
            if (this.runButton.Checked)
            {
                ManipulationStateManager.Singleton.ActiveState = ManipulationState.algorithmInitiationState;
            }
            else
            {
                ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
            }
        }
        else
        {
            this.runButton.UpdateStatus(false);
        }
    }

    public void DisplayAlgorithmResult() {
        ManipulationStateManager.Singleton.ActiveState = ManipulationState.algorithmDisplayState;
    }

    public void ClearAlgorithmResults() {
        this.algorithmResults = new object[this.associations.Length];
        this.AlgorithmResult = null;
        this.resultButton.gameObject.SetActive(false);

        foreach (GraphDisplayAlgorithmAssociation association in this.associations) {
            association.activationButton.checkedColor = this.selectedColor;
            association.activationButton.originalColor = this.deafultColor;
            association.activationButton.GetComponent<Image>().color = this.deafultColor;
            association.activationButton.UpdateStatus(association.activationButton.Checked);
        }

        if (ManipulationStateManager.Singleton.ActiveState == ManipulationState.algorithmDisplayState) {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
        }
    }

    public void ToggleAlgorithmResultDisplay() {
        if (this.resultButton.Checked) {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.algorithmDisplayState;
        }
        else {
            ManipulationStateManager.Singleton.ActiveState = ManipulationState.viewState;
        }
    }

    public void SetStepByStep(bool enabled)
    {
        this.StepByStep = enabled;
    }

    public void Search(string term)
    {
        foreach (GraphDisplayAlgorithmAssociation association in this.associations)
        {
            if (association.algorithmClass.ToLower().Contains(term.ToLower()))
            {
                association.activationButton.gameObject.SetActive(association.enabled);
            }
            else
            {
                association.activationButton.gameObject.SetActive(false);
            }
        }
    }

    //deactivate the graphInfo panel and display the open panel button for the user to access
    public void CloseAlgorithmInfoPanel(){
        this.gameObject.SetActive(false); 
        algOpenPanel.gameObject.SetActive(true);
    }

    //activate the graphInfo panel and prevent access to the open panel button
    public void OpenAlgorithmInfoPanel(){
        this.gameObject.SetActive(true);
        algOpenPanel.gameObject.SetActive(false);
    }

}

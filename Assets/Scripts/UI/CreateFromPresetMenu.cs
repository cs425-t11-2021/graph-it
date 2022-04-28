//All code developed by Team 11

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateFromPresetMenu : MonoBehaviour
{
    //[SerializeField] private GameObject presetMenu;
    private int parameter = 5;
    [SerializeField] private TMP_InputField parameterInput;

    //helper function for creating from preset menu to disable interactions with the program until the user makes a choice or closes the menu
    //NEED TO BE UPDATED WITH CURRENT UI ELEMENTS
    private void OnEnable() {
        UIManager.Singleton.MenuBarEnabled = false;
        UIManager.Singleton.GraphInfoEnabled = false;
        UIManager.Singleton.OpenGraphInfoPanelEnabled = false;
        UIManager.Singleton.AlgorithmsPanelEnabled = false;
        UIManager.Singleton.OpenAlgInfoPanelEnabled = false;
        UIManager.Singleton.TabsBarEnabled = false;
        UIManager.Singleton.ToolBarEnabled = false;

        // Suspend the manipulation state if the import menu is active
        ManipulationStateManager.Singleton.SuspendManipulationState(true);
        
        parameterInput.text = this.parameter.ToString();
    }

    //helper function to restore the user's ability to interact with the program once the menu is closed
    private void OnDisable() {
        UIManager.Singleton.MenuBarEnabled = true;
        UIManager.Singleton.GraphInfoEnabled = true;
        UIManager.Singleton.OpenGraphInfoPanelEnabled = true;
        UIManager.Singleton.AlgorithmsPanelEnabled = true;
        UIManager.Singleton.OpenAlgInfoPanelEnabled = true;
        UIManager.Singleton.TabsBarEnabled = true;
        UIManager.Singleton.ToolBarEnabled = true;

        // Unsuspend the manipulation state
        ManipulationStateManager.Singleton.SuspendManipulationState(false);
    }
    public void openPresetMenu(){
        OnEnable();//not sure if this will work
        this.gameObject.SetActive(true);
    }
    public void closePresetMenu(){
        OnDisable();//also not sure if this will work
        this.gameObject.SetActive(false);
    }

    public void SetParameter(string paramter)
    {
        if (int.TryParse(paramter, out int n))
        {
            this.parameter = n;
        }
        else
        {
            this.parameter = 5;
            parameterInput.text = this.parameter.ToString();
        }
    }

    public void CreatePresetGraph(Func<int, Graph> generator, string name)
    {
        GraphInstance instance;
        if (Controller.Singleton.VertexObjs.Count != 0 || Controller.Singleton.Graph.Changes.Count != 0)
        {
            instance = Controller.Singleton.CreateGraphInstance(true, generator(this.parameter));
        }
        else
        {
            Controller.Singleton.ReplaceGraph(generator(this.parameter));
            instance = Controller.Singleton.ActiveGraphInstance;
        }
        Controller.Singleton.CreateObjsFromGraph(instance);
        
        closePresetMenu();
        NotificationManager.Singleton.CreateNotification(string.Format("Creating a {1} graph with n = <#0000FF>{0}</color>.", this.parameter, name), 3);
    }

    public void CreateComplete()
    {
        CreatePresetGraph(PresetGraph.Complete, "Complete");
    }

    public void CreateCompleteBipartite()
    {
        CreatePresetGraph(PresetGraph.CompleteBipartite, "Complete Bipartite");
    }

    public void CreateCycle()
    {
        CreatePresetGraph(PresetGraph.Cycle, "Cycle");
    }

    public void CreatePath()
    {
        CreatePresetGraph(PresetGraph.Path, "Path");
    }

    public void CreateStar()
    {
        CreatePresetGraph(PresetGraph.Star, "Star");
    }
}

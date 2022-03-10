//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFromPresetMenu : MonoBehaviour
{
    //[SerializeField] private GameObject presetMenu;

    //helper function for creating from preset menu to disable interactions with the program until the user makes a choice or closes the menu
    //NEED TO BE UPDATED WITH CURRENT UI ELEMENTS
        private void OnEnable() {
        UIManager.Singleton.MenuBarEnabled = false;
        UIManager.Singleton.AlgorithmsPanelEnabled = false;
        UIManager.Singleton.ToolBarEnabled = false;

        // Suspend the manipulation state if the import menu is active
        ManipulationStateManager.Singleton.SuspendManipulationState(true);
    }

    //helper function to restore the user's ability to interact with the program once the menu is closed
    private void OnDisable() {
        UIManager.Singleton.MenuBarEnabled = true;
        UIManager.Singleton.AlgorithmsPanelEnabled = true;
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

    public void CreateComplete()
    {
        Controller.Singleton.Graph = PresetGraph.Complete(5);
        Controller.Singleton.CreateObjsFromGraph();
        closePresetMenu();
    }

    public void CreateCompleteBipartite()
    {
        Controller.Singleton.Graph = PresetGraph.CompleteBipartite(5);
        Controller.Singleton.CreateObjsFromGraph();
        closePresetMenu();
    }

    public void CreateCycle()
    {
        Controller.Singleton.Graph = PresetGraph.Cycle(5);
        Controller.Singleton.CreateObjsFromGraph();
        closePresetMenu();
    }

    public void CreatePath()
    {
        Controller.Singleton.Graph = PresetGraph.Path(5);
        Controller.Singleton.CreateObjsFromGraph();
        closePresetMenu();
    }

    public void CreateStar()
    {
        Controller.Singleton.Graph = PresetGraph.Star(5);
        Controller.Singleton.CreateObjsFromGraph();
        closePresetMenu();
    }
}

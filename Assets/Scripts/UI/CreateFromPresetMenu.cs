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
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.Complete(5));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
    }

    public void CreateCompleteBipartite()
    {
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.CompleteBipartite(5));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
    }

    public void CreateCycle()
    {
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.Cycle(5));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
    }

    public void CreatePath()
    {
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.Path(5));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
    }

    public void CreateStar()
    {
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.Star(5));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
    }
}

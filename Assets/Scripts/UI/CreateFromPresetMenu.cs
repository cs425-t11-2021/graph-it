//All code developed by Team 11
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
        UIManager.Singleton.AlgorithmsPanelEnabled = false;
        UIManager.Singleton.ToolBarEnabled = false;

        // Suspend the manipulation state if the import menu is active
        ManipulationStateManager.Singleton.SuspendManipulationState(true);
        
        parameterInput.text = this.parameter.ToString();
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

    public void CreateComplete()
    {
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.Complete(this.parameter));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
        
        NotificationManager.Singleton.CreateNotification(string.Format("Creating a complete graph with n = <#0000FF>{0}</color>.", this.parameter), 3);
    }

    public void CreateCompleteBipartite()
    {
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.CompleteBipartite(this.parameter));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
        
        NotificationManager.Singleton.CreateNotification(string.Format("Creating a complete bipartite graph with n = <#0000FF>{0}</color>.", this.parameter), 3);
    }

    public void CreateCycle()
    {
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.Cycle(this.parameter));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
        
        NotificationManager.Singleton.CreateNotification(string.Format("Creating a cycle graph with n = <#0000FF>{0}</color>.", this.parameter), 3);
    }

    public void CreatePath()
    {
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.Path(this.parameter));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
        
        NotificationManager.Singleton.CreateNotification(string.Format("Creating a path graph with n = <#0000FF>{0}</color>.", this.parameter), 3);
    }

    public void CreateStar()
    {
        GraphInstance newInstance = Controller.Singleton.CreateGraphInstance(true, PresetGraph.Star(this.parameter));
        Controller.Singleton.CreateObjsFromGraph(newInstance);
        closePresetMenu();
        
        NotificationManager.Singleton.CreateNotification(string.Format("Creating a star graph with n = <#0000FF>{0}</color>.", this.parameter), 3);
    }
}

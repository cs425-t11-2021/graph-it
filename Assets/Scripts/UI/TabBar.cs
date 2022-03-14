using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabBar : SingletonBehavior<TabBar>
{
    // Prefab of the tab UI
    [SerializeField] private GameObject tabPrefab;
    
    // List of tabs currently in the scene
    public List<Tab> TabsInScene { get; private set; }

    private void Awake()
    {
        TabsInScene = new List<Tab>();
    }

    // Method for creating a new tab in the tab bar given a name and associated graph instance
    public void CreateNewTab(string name, GraphInstance associatedInstance)
    {
        Tab newTab = Instantiate(tabPrefab, this.transform).GetComponent<Tab>();
        newTab.Initiate(name, associatedInstance);
        
        this.TabsInScene.Add(newTab);
    }
}

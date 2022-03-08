using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TabManager : SingletonBehavior<TabManager>
{
    [SerializeField] private GameObject tabPrefab;

    public void CreateNewTab(GraphInstance graphInstance, string label)
    {
        Tab tab = Instantiate(tabPrefab, this.transform).GetComponent<Tab>();
        tab.GraphInstance = graphInstance;
        tab.Label = label;
    }
}

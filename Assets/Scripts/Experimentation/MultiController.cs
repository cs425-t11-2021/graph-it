//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MultiController : MonoBehaviour
{
    public static MultiController singleton;

    [SerializeField]
    private GameObject universePrefab;
    private List<Controller> controllers;
    private Controller activeCont;

    private void Awake() {
        if (singleton == null) {
            MultiController.singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }

        controllers = new List<Controller>();
    }

    private void Start() {
        controllers.Add(Controller.singleton);
        activeCont = Controller.singleton;
    }

    public void CreateNewController() {
        GameObject universe = Instantiate(universePrefab, Vector3.zero, Quaternion.identity);

        Controller cont = universe.GetComponentInChildren<Controller>();
        cont.enabled = false;
        controllers.Add(cont);

        universe.GetComponentInChildren<SelectionManager>().enabled = false;
        universe.GetComponentInChildren<Tester>().enabled =false;
        universe.GetComponentInChildren<DoubleClick>().enabled = false;
        universe.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void SwitchUniverse(int id) {
        if (id < controllers.Count) {
            Controller.singleton.graphObj.gameObject.SetActive(false);
            Controller.singleton.enabled = false;
            DoubleClick.singleton.enabled = false;
            SelectionManager.singleton.enabled = false;

            GameObject universe = controllers[id].transform.parent.gameObject;

            Controller c = universe.GetComponentInChildren<Controller>();
            SelectionManager s = universe.GetComponentInChildren<SelectionManager>();
            DoubleClick d = universe.GetComponentInChildren<DoubleClick>();

            c.enabled = true;
            Controller.singleton = c;
            activeCont = c;
            s.enabled = true;
            SelectionManager.singleton = s;
            d.enabled = true;
            DoubleClick.singleton = d;

            universe.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}

//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used for testing purposes, disable before build
public class Tester : SingletonBehavior<Tester>
{

    // Reference to main graph ds
    private Graph graph_ds;

    public char izard = 'a';

    private void Awake() {
        Debug.Log("hi2");
    }

    private void Start() {
       Debug.Log(Tester.Singleton.izard);
    }
}

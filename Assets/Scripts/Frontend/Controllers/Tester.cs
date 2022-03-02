//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used for testing purposes, disable before build
public class Tester : SingletonBehavior<Tester>
{
    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            Controller.Singleton.CreateGraphInstance();
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            Controller.Singleton.ChangeActiveInstance(Controller.Singleton.instances[1]);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            Controller.Singleton.ChangeActiveInstance(Controller.Singleton.instances[0]);
        }
    }
   
}

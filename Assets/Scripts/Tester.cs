//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script used for testing purposes, disable before build
public class Tester : SingletonBehavior<Tester>
{
    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            GraphPhysics.Singleton.UseGraphPhysics(3);
        }
    }
   
}

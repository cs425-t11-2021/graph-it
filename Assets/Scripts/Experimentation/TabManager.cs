using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{
    private void Start() {
        MultiController.singleton.CreateNewController();
    }

    public void SwitchTab(int index) {
        MultiController.singleton.SwitchUniverse(index);
    }
}

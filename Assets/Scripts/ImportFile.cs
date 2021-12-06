using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportFile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //when program first starts, user should not be able to access the import from file menu pop-up
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

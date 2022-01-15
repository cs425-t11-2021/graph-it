//All code developed by Team 11
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FileMenu : MonoBehaviour
{
    private GameObject importMenuItem;
    private GameObject exportMenuItem;
    private Button fileButton;
    public GameObject importFileButton;
    public GameObject exportFileButton;

    [SerializeField]
    private GameObject newGraphButton;

    // Start is called before the first frame update
    void Start()
    {
        //getting the "ImportToFile" button object to check if clicked
        importMenuItem = transform.GetChild(1).GetChild(2).gameObject;
        //getting the "ExportToFile" button object to check if clicked
        exportMenuItem = transform.GetChild(1).GetChild(3).gameObject;

        //getting the "file" button to monitor if selected
        //fileButton = this.GetComponent<Button>();
        //fileButton.onClick.AddListener(DisplayDropDown);
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == importMenuItem)
        {
            //if the "ImportToFile" is clicked, show the import to file pop-up
            importFileButton.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == exportMenuItem)
        {
            exportFileButton.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject == newGraphButton)
        {
            Controller.singleton.ClearGraphObjs();
            Controller.singleton.Graph.Clear();
        }
    }
}

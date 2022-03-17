//This is a testing script to experiment with detecting mouse hover over a UI element
//Modified from: https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseOver.html 
//needs to be attached to GameObject
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingOnHover : MonoBehaviour
{
    //[SerializeField] private GameObject testButton;

    //When the mouse hovers over the GameObject, it turns to this color (red)
    Color m_MouseOverColor = Color.red;

    //This stores the GameObject’s original color
    Color m_OriginalColor;

    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    Image m_Image; //not sure how needed this is

    void Start()
    {
        //Fetch the mesh renderer component from the GameObject
        m_Image = GetComponent<Image>();
        //Fetch the original color of the GameObject
        m_OriginalColor = m_Image.color;
    }

    void OnMouseOver()
    {
        // Change the color of the GameObject to red when the mouse is over GameObject
        m_Image.color = m_MouseOverColor;
    }

    void OnMouseExit()
    {
        // Reset the color of the GameObject back to normal
        m_Image.color = m_OriginalColor;
    }
}

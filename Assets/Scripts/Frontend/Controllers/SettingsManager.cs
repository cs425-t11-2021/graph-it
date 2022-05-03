using System;
using UnityEngine;

[Serializable]
public struct ColorScheme {
    Color backgroundColor;
    Color textColor;
}

// Class which handles storing and updating various visual settings such as enabling graph labels, snap to grid, etc. 
// This class will be hooked up to a UI settings page in the future.
public class SettingsManager : SingletonBehavior<SettingsManager>
{
    [Header("Default Visual Settings")]

    // Whether or not vertex and edge labels should be displayed
    // bool to store the setting
    // event to be subscribed to by other classes that needs side effects when changes are updated
    [SerializeField]
    private bool displayVertexLabels;
    public event Action<bool> OnToggleVertexLabels;
    public bool DisplayVertexLabels {
        get => this.displayVertexLabels;
        set
        {
            this.displayVertexLabels = value;
            this.OnToggleVertexLabels?.Invoke(value);
            Logger.Log("Display vertex labels set to " + value, this, LogType.INFO);
        }
    }
    
    // Whether or not vertices should snap to grid
    [SerializeField]
    private bool snapVerticesToGrid;
    public event Action<bool> OnToggleGridSnapping;
    public bool SnapVerticesToGrid {
        get => this.snapVerticesToGrid;
        set {
            this.snapVerticesToGrid = value;
            this.OnToggleGridSnapping?.Invoke(value);
            Logger.Log("Snap vertices to grid set to " + value, this, LogType.INFO);
        }
    }

    // Whether or not gridlines should always be displayed
    [SerializeField]
    private bool alwaysShowGridlines;
    public event Action<bool> OnToggleAlwaysShowGridlines;
    public bool AlwaysShowGridlines
    {
        get => this.alwaysShowGridlines;
        set
        {
            if (this.snapVerticesToGrid)
            {
                this.alwaysShowGridlines = value;
                this.OnToggleGridSnapping?.Invoke(value);
                Logger.Log("Always show gridlines set to " + value, this, LogType.INFO);
            }
            else
            {
                this.alwaysShowGridlines = false;
            }
        }
    }
    
    // How big the gap should be between edges and vertices
    [SerializeField] private float edgeVertexGap = 0f;
    public float EdgeVertexGap
    {
        get => this.edgeVertexGap;
    }

    public Sprite[] vertexSprites;

    // Method run at Awake to implement the default settings
    private void ImplementDefaultSettings()
    {
        DisplayVertexLabels = this.displayVertexLabels;
        SnapVerticesToGrid = this.snapVerticesToGrid;
        AlwaysShowGridlines = this.alwaysShowGridlines;
    }

    private void Awake() {
        ImplementDefaultSettings();
    }
}

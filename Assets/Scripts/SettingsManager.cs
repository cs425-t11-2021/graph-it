using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : SingletonBehavior<SettingsManager>
{
    // TODO: Move the visual settings away from controller and into its own object
    [Header("Default Visual Settings")]

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

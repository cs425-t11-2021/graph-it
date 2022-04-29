using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D;

[Serializable]
public struct VisualState
{
    public string name;
    public Color color;
}

public class GraphVisualsAnimator : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private VisualState[] states;
    [SerializeField] private int defaultSortingOrder;
    private VisualState currentState;

    public event Action OnVisualsUpdate;

    public float Scale { get; private set; } = 1f;

    private void Start()
    {
        ChangeState("default");
    }

    private void ChangeRenderingColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer is SpriteRenderer)
            {
                ((SpriteRenderer) renderer).color = color;
            }
            else if (renderer is SpriteShapeRenderer)
            {
                ((SpriteShapeRenderer) renderer).color = color;
            }

            renderer.sortingOrder = this.currentState.name == "default" || this.currentState.name == "algorithm_none" ? defaultSortingOrder : defaultSortingOrder + 1;
        }
    }

    public void ChangeState(string name)
    {
        foreach (VisualState state in this.states)
        {
            if (state.name == name)
            {
                this.currentState = state;
                ChangeRenderingColor(state.color);
                return;
            }
        }
    }

    public void ExpandEffect(float duration, float scale, string changeState)
    {
        StartCoroutine(RunExpandEffect(duration, scale, changeState));
    }

    IEnumerator RunExpandEffect(float duration, float scale, string changeState)
    {
        string previousState = currentState.name;
        ChangeState(changeState);
        
        for (int i = 0; i < 10; i++)
        {
            this.Scale = this.Scale + (scale - this.Scale) / 10f * i;
            OnVisualsUpdate?.Invoke();
            yield return new WaitForSeconds(0.03f);
        }
        
        this.Scale = scale;
        OnVisualsUpdate?.Invoke();
        yield return new WaitForSeconds(duration);
        ChangeState(previousState);

        this.Scale = 1f;
        OnVisualsUpdate?.Invoke();
    }
}


// All code developed by Team 11

using System;
using System.Threading;
using System.Collections.Generic;

public enum StepType { ADD_TO_RESULT, CONSIDER, FINISHED }

public struct AlgorithmStep
{
    public int index;
    public StepType type;
    public string desc;

    public List< Vertex > resultVertices;
    public List< Edge > resultEdges;

    public List< Vertex > newVertices;
    public List< Edge > newEdges;

    public List< Vertex > considerVertices;
    public List< Edge > considerEdges;

    public AlgorithmStep( StepType type, string desc ) : this()
    {
        this.type = type;
        this.desc = desc;
    }
}

public abstract class LoggedAlgorithm : Algorithm
{
    private bool takenFirstStep;
    private int step;
    private List< AlgorithmStep > steps;

    public LoggedAlgorithm( AlgorithmManager algoManager ) : base( algoManager )
    {
        this.step = -1;
        this.steps = new List< AlgorithmStep >();
    }

    public bool NextStepAvailable() => this.step < this.steps.Count - 1;

    public bool BackStepAvailable() => this.step > 0;

    public bool GetStepAvailable() => this.step >= 0;

    public void NextStep()
    {
        if ( !this.NextStepAvailable() )
            throw new System.Exception( "Cannot perform next step when step is currently being computed." );
        this.step++;
    }

    public void BackStep()
    {
        if ( !this.BackStepAvailable() )
            throw new System.Exception( "Cannot perform back step when step is currently being computed." );
        this.step--;
    }

    public AlgorithmStep GetStep()
    {
        if ( !this.GetStepAvailable() )
            throw new System.Exception( "Cannot retrieve step when no step has been taken." );
        return this.steps[ this.step ];
    }

    protected void AddStep(
        StepType type,
        string desc,
        List< Vertex > resultVerts,
        List< Edge > resultEdges,
        List< Vertex > newVerts,
        List< Edge > newEdges,
        List< Vertex > considerVerts,
        List< Edge > considerEdges
    )
    {
        AlgorithmStep step = new AlgorithmStep( type, desc );
        step.index = this.steps.Count + 1;
        step.resultVertices = resultVerts;
        step.resultEdges = resultEdges;
        step.newVertices = newVerts;
        step.newEdges = newEdges;
        step.considerVertices = considerVerts;
        step.considerEdges = considerEdges;
        this.steps.Add( step );
    }
}

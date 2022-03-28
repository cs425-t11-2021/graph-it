
// All code developed by Team 11

using System;
using System.Threading;
using System.Collections.Generic;

public enum StepType { ADD_TO_RESULT, CONSIDER, FINISHED }

public abstract class LoggedAlgorithm : Algorithm
{
    private bool takenFirstStep;
    private int step;
    private List< ( StepType, List< Vertex >, List< Edge >, string ) > steps;

    public LoggedAlgorithm( AlgorithmManager algoManager ) : base( algoManager )
    {
        this.step = -1;
        this.steps = new List< ( StepType, List< Vertex >, List< Edge >, string ) >();
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

    public ( StepType, List< Vertex >, List< Edge >, string ) GetStep()
    {
        if ( !this.GetStepAvailable() )
            throw new System.Exception( "Cannot retrieve step when no step has been taken." );
        return this.steps[ this.step ];
    }

    protected void AddStep( StepType type, List< Vertex > verts, List< Edge > edges, string desc ) => this.steps.Add( ( type, verts, edges, desc ) );
}

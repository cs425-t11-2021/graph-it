
// All code developed by Team 11

using System;
using System.Threading;
using System.Collections.Generic;

public enum StepType { ADD_TO_RESULT, CONSIDER, FINISHED, ERROR }

public struct AlgorithmStep
{
    public int index;
    public StepType type;
    public string desc;

    public List< Vertex > paramVerts;
    public List< Edge > paramEdges;

    public List< Vertex > newVerts;
    public List< Edge > newEdges;

    public List< Vertex > resultVerts;
    public List< Edge > resultEdges;

    public List< Vertex > considerVerts;
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

    public bool IsNextStepAvailable() => this.step < this.steps.Count - 1;

    public bool IsBackStepAvailable() => this.step > 0;

    public bool IsFirstStepAvailable() => this.step >= 0;

    public bool IsStepAvailable( int step ) => this.steps.Count >= step; // index based 1

    public void NextStep()
    {
        if ( !this.IsNextStepAvailable() )
            throw new System.Exception( "Cannot perform next step when step is currently being computed." );
        this.step++;
    }

    public void BackStep()
    {
        if ( !this.IsBackStepAvailable() )
            throw new System.Exception( "Cannot perform back step when step is currently being computed." );
        this.step--;
    }

    // index based 1
    public void GoToStep( int step )
    {
        if ( !this.IsStepAvailable( step ) )
        {
            if ( this.complete )
                throw new System.Exception( "Algorithm completed before step " + step + " was reached." );
            throw new System.Exception( "Cannot perform go to step when step is currently being computed." );
        }

        this.step = step - 1;
    }

    public AlgorithmStep GetStep()
    {
        if ( !this.IsFirstStepAvailable() )
            throw new System.Exception( "Cannot retrieve step when no step has been taken." );
        return this.steps[ this.step ];
    }

    protected void AddStep(
        StepType type,
        string desc,
        IEnumerable< Vertex > newVerts=null,
        IEnumerable< Edge   > newEdges=null,
        IEnumerable< Vertex > resultVerts=null,
        IEnumerable< Edge   > resultEdges=null,
        IEnumerable< Vertex > considerVerts=null,
        IEnumerable< Edge   > considerEdges=null
    )
    {
        AlgorithmStep step = new AlgorithmStep( type, desc );
        step.index = this.steps.Count + 1;
        step.paramVerts    = this.vertexParms is null ? null : new List< Vertex >( this.vertexParms );
        step.paramEdges    = this.edgeParms   is null ? null : new List< Edge   >( this.edgeParms   );
        step.newVerts      = newVerts         is null ? null : new List< Vertex >( newVerts         );
        step.newEdges      = newEdges         is null ? null : new List< Edge   >( newEdges         );
        step.resultVerts   = resultVerts      is null ? null : new List< Vertex >( resultVerts      );
        step.resultEdges   = resultEdges      is null ? null : new List< Edge   >( resultEdges      );
        step.considerVerts = considerVerts    is null ? null : new List< Vertex >( considerVerts    );
        step.considerEdges = considerEdges    is null ? null : new List< Edge   >( considerEdges    );
        this.steps.Add( step );
    }
}

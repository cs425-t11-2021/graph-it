// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NumberOfSpanningTreesAlgorithm : Algorithm
{
    public ulong NumberOfSpanningTrees { get; private set; }

    public NumberOfSpanningTreesAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        this.AlgoManager.RunWeightMatrix(false);
        this.WaitUntilWeightMatrixComplete();

        float[,] weightMatrix = (float[,]) this.AlgoManager.GetWeightMatrix().results["weight matrix"].Item1;

        int n = weightMatrix.GetLength(0);
        
        double[,] laplacian = new double[n,n];

        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                laplacian[i,j] = 0;
        

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i != j && !float.IsInfinity(weightMatrix[i,j]))
                {
                    laplacian[i,j] = -1;
                    laplacian[i,i] += 1;
                }
            }
        }

        double det = 1;

        for (int i = 0; i < n - 1; i++)
        {
            // find pivot, and swap it to the top
            int pivot = -1;
            for (int j = i; j < n - 1; j++)
            {
                if (laplacian[j,i] != 0)
                {
                    if (pivot == -1 || Math.Abs(laplacian[j,i]) < Math.Abs(laplacian[pivot,i]) )
                    {
                        pivot = j;
                    }
                }
            }

            if (pivot == -1) // no pivot, det is 0
            {
                det = 0;
                break;
            }

            SwapRows(laplacian, i, pivot);

            det *= laplacian[i,i];

            for (int j = i + 1; j < n - 1; j++)
            {
                if (laplacian[j,i] != 0)
                {
                    double coeff = laplacian[j,i] / laplacian[i,i];
                    for (int k = i; k < n - 1; k++)
                    {
                        laplacian[j,k] = laplacian[j,k] - laplacian[i,k]*coeff;
                    }
                }
            }
        }

        this.NumberOfSpanningTrees = (ulong) Math.Abs(det);
    }

    private void SwapRows(double[,] array, int row1, int row2)
    {
        if (row1 == row2) return;

        for (int i = 0; i < array.GetLength(0); i++)
        {
            double temp = array[row1,i];
            array[row1,i] = array[row2,i];
            array[row2,i] = temp;
        }
    }

    private void WaitUntilWeightMatrixComplete()
    {
        this.WaitUntilAlgorithmComplete( WeightMatrixAlgorithm.GetHash() );
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "number of spanning trees" ] = ( this.NumberOfSpanningTrees, typeof(ulong) );
        return result;
    }

    public static int GetHash() => typeof ( NumberOfSpanningTreesAlgorithm ).GetHashCode();
    public override int GetHashCode() => NumberOfSpanningTreesAlgorithm.GetHash();
}

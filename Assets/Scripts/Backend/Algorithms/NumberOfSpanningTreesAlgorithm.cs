// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class NumberOfSpanningTreesAlgorithm : Algorithm
{
    public int NumberOfSpanningTrees { get; private set; }

    public NumberOfSpanningTreesAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        this.AlgoManager.RunWeightMatrix(false);

        float[,] laplacian = this.AlgoManager.GetWeightMatrix();

        int n = laplacian.GetLength(0);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i != j && laplacian[i,j] > 0)
                {
                    laplacian[i,j] = -1;
                    laplacian[i,i] += 1;
                }
            }
        }

        float det = 1;

        for (int i = 0; i < n - 1; i++)
        {
            // find pivot, and swap it to the top
            int pivot = -1;
            for (int j = i; j < n - 1; j++)
            {
                if (laplacian[i,j] != 0)
                {
                    pivot = j;
                    break;
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
                    float coeff = laplacian[j,i] / laplacian[i,i];
                    for (int k = i; k < n - 1; k++)
                    {
                        laplacian[j,k] -= coeff * laplacian[i,k];
                    }
                }
            }
        }

        NumberOfSpanningTrees = (int) det;
    }

    private void SwapRows(float[,] array, int row1, int row2)
    {
        if (row1 == row2) return;

        for (int i = 0; i < array.GetLength(0); i++)
        {
            float temp = array[row1,i];
            array[row1,i] = array[row2,i];
            array[row2,i] = temp;
        }
    }

    private void WaitUntilWeightMatrixComplete()
    {
        this.WaitUntilAlgorithmComplete( WeightMatrixAlgorithm.GetHash() );
    }

    public static int GetHash() => typeof ( NumberOfSpanningTreesAlgorithm ).GetHashCode();
    public override int GetHashCode() => NumberOfSpanningTreesAlgorithm.GetHash();
}

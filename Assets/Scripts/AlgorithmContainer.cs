using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Algorithm", menuName = "GraphAlgorithm")]
public class AlgorithmContainer : ScriptableObject
{
    [SerializeField] private Algorithm associatedAlgorithm;
}

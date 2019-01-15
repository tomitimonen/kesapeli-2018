using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class PathTriggerVolume : MonoBehaviour {

    [SerializeField]
    private BezierSpline path;
    public BezierSpline Path { get { return path; } }

    private void Start()
    {
        if (path == null)
        {
            Debug.LogError(gameObject.name+": Path is not set");
        }
    }
}

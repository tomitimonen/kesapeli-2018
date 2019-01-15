using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class PathKeeper : MonoBehaviour {

    [SerializeField]
    public BezierSpline currentSpline;
    [SerializeField]
    bool showDebug = false;

    public BezierSpline outCurrentSpline { get; private set; }
    public Vector3 outSplinePos { get; private set; }
    public Vector3 outTangent { get; private set; }
    public Vector3 outHorTangent { get; private set; }
    public float outRelativePosition { get; private set; }
    public float outPathVelocity { get; private set; }

    float splineAccuracy = 400f;
    Rigidbody rb;
    ConfigurableJoint joint;
    bool strayMode = false;

    Vector3 splinePos;
    Vector3 tangent;
    Vector3 horTangent;
    float relativePosition;
    float pathVelocity;

    Vector3 pathSplinePos;
    Vector3 pathTangent;
    Vector3 pathHorTangent;
    float pathRelativePosition;
    float pathPathVelocity;

    IDictionary<BezierSpline, Path> paths;

    class Path
    {
        public Vector3 splinePos;
        public Vector3 tangent;
        public Vector3 horTangent;
        public float relativePosition;
        public float pathVelocity;
    }

    
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
        paths = new Dictionary<BezierSpline, Path>();
        outCurrentSpline = currentSpline;

        outHorTangent = Vector3.up;
	}
	
	// Update is called once per frame
	void Update () {

        if (showDebug)
        {
            foreach (BezierSpline spline in paths.Keys)
            {
                Path path = paths[spline];
                Debug.DrawLine(path.splinePos, path.splinePos + Vector3.up * 2f, Color.red);
            }
            //Debug.Log(paths.Keys.Count);

            //Debug.DrawLine(splinePos, splinePos + Vector3.up * 2f, Color.red);

            Debug.DrawLine(pathSplinePos, pathSplinePos + Vector3.up * 2f, Color.green);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        PathTriggerVolume vol = other.gameObject.GetComponent<PathTriggerVolume>();
        if (vol != null)
        {
            if (showDebug) Debug.Log("enter path volume");
            BezierSpline newSpline = vol.Path;
            if (newSpline == null) Debug.Log("PathKeeper - OnTriggerEnter: Path not set in path trigger volume");
            if (!paths.ContainsKey(newSpline))
            {
                Path newPath = new Path
                {
                    splinePos = Vector3.zero,
                    tangent = Vector3.right,
                    horTangent = Vector3.right,
                    relativePosition = 0f,
                    pathVelocity = 0f
                };
                paths.Add(newSpline, newPath);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PathTriggerVolume vol = other.gameObject.GetComponent<PathTriggerVolume>();
        if (vol != null)
        {
            if (showDebug) Debug.Log("exit path volume");
            BezierSpline oldSpline = vol.Path;
            if (oldSpline == null) Debug.Log("PathKeeper - OnTriggerExit: Path not set in path trigger volume");
            if (paths.ContainsKey(oldSpline))
            {
                paths.Remove(oldSpline);
            }
        }
    }


    private void FixedUpdate()
    {
        //pelaajaa lähin polku
        //polun hahmoa lähin piste ja suunta polkua pitkin eteen päin BezierSpline spline in paths.Keys
        foreach(BezierSpline spline in paths.Keys)
        {
            Path path = paths[spline];

            path.splinePos = spline.FindNearestPointTo(transform.position, out path.relativePosition, splineAccuracy);
            Vector3 posAtSplineHeight = new Vector3(transform.position.x, path.splinePos.y, transform.position.z);
            path.splinePos = spline.FindNearestPointTo(posAtSplineHeight, out path.relativePosition, splineAccuracy);

            path.tangent = spline.GetTangent(path.relativePosition).normalized;
            path.horTangent = new Vector3(path.tangent.x, 0, path.tangent.z).normalized;
            path.pathVelocity = Vector3.Dot(path.horTangent, rb.velocity);
        }
        /*splinePos = currentSpline.FindNearestPointTo(transform.position, out relativePosition, splineAccuracy);
        Vector3 posAtSplineHeight = new Vector3(transform.position.x, splinePos.y, transform.position.z);
        splinePos = currentSpline.FindNearestPointTo(posAtSplineHeight, out relativePosition, splineAccuracy);

        tangent = currentSpline.GetTangent(relativePosition).normalized;
        horTangent = new Vector3(tangent.x, 0, tangent.z).normalized;
        pathVelocity = Vector3.Dot(horTangent, rb.velocity);*/


        //vanhaa polun pistettä lähin polku
        //polun hahmoa lähin piste ja suunta polkua pitkin eteen päin
        Vector3 pathPos = new Vector3(transform.position.x, pathSplinePos.y, transform.position.z);
        pathSplinePos = currentSpline.FindNearestPointTo(pathPos, out pathRelativePosition, splineAccuracy);
        Vector3 charPosAtSplineHeight = new Vector3(transform.position.x, pathSplinePos.y, transform.position.z);
        pathSplinePos = currentSpline.FindNearestPointTo(charPosAtSplineHeight, out pathRelativePosition, splineAccuracy);

        pathTangent = currentSpline.GetTangent(pathRelativePosition).normalized;
        pathHorTangent = new Vector3(pathTangent.x, 0, pathTangent.z).normalized;
        pathPathVelocity = Vector3.Dot(pathHorTangent, rb.velocity);

        /*if (Mathf.Abs(relativePosition - pathRelativePosition) > .01f && !strayMode)
        {
            if (showDebug) Debug.Log("Stray from path");
            strayMode = true;
        }*/

        //hahmon pakotus polulle configurablejointin avulla
        if (/*!strayMode*/false)
        {
            //velocityn asetus seuraamaan polkua
            rb.velocity = Vector3.Dot(rb.velocity, horTangent) * horTangent + new Vector3(0, rb.velocity.y, 0);
            //hahmon pakotus polulle jointilla
            joint.connectedAnchor = splinePos;
            Vector3 localDir = transform.InverseTransformDirection(horTangent);
            joint.axis = localDir;
            //julkisten muuttujien päivitys
            outSplinePos = splinePos;
            outTangent = tangent;
            outHorTangent = horTangent;
            outRelativePosition = relativePosition;
            outPathVelocity = pathVelocity;
        }
        else
        {
            //velocityn asetus seuraamaan polkua
            rb.velocity = Vector3.Dot(rb.velocity, pathHorTangent) * pathHorTangent + new Vector3(0, rb.velocity.y, 0);
            //hahmon pakotus polulle jointilla
            joint.connectedAnchor = pathSplinePos;
            Vector3 localDir = transform.InverseTransformDirection(pathHorTangent);
            joint.axis = localDir;
            //julkisten muuttujien päivitys
            outSplinePos = pathSplinePos;
            outTangent = pathTangent;
            outHorTangent = pathHorTangent;
            outRelativePosition = pathRelativePosition;
            outPathVelocity = pathPathVelocity;
            outCurrentSpline = currentSpline;
        }

        float smallestDist = Mathf.Infinity;
        BezierSpline closestSpline = null;
        foreach (BezierSpline spline in paths.Keys)
        {
            Path path = paths[spline];
            float dist = Vector3.Distance(transform.position - Vector3.up * .5f, path.splinePos);

            if (dist < smallestDist)
            {
                smallestDist = dist;
                closestSpline = spline;
            }
        }
        if (smallestDist < .5f)
        {
            Path closestPath = paths[closestSpline];
            //if (showDebug) Debug.Log("Return to path");
            //yhdistetään polut
            pathSplinePos = closestPath.splinePos;
            pathTangent = closestPath.tangent;
            pathHorTangent = closestPath.horTangent;
            pathRelativePosition = closestPath.relativePosition;
            pathPathVelocity = closestPath.pathVelocity;
            currentSpline = closestSpline;
        }

        /*if (strayMode && Vector3.Distance(transform.position-Vector3.up*.5f, splinePos) < .5f)
        {
            //if (showDebug) Debug.Log("Return to path");
            strayMode = false;
            //yhdistetään polut
            pathSplinePos = splinePos;
            pathTangent = tangent;
            pathHorTangent = horTangent;
            pathRelativePosition = relativePosition;
            pathPathVelocity = pathVelocity;
        }*/


    }
}

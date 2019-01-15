using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class AirMovement : MonoBehaviour {

    public bool showDebug = false;

    public float runForce = 180000f;
    public float runSpeed = 4f;
    public float verticalForce = 180000f;
    public float verticalSpeed = 4f;
    public float knockBackTime = .5f;

    public int runDirection { get; private set; }
    public float invertDir { get; private set; }
    public int verticalDirection { get; private set; }

    float againstWallThreshold = .9f;

    bool damageKnockbackStopRunning = false;

    bool isAgainstWallBackwards = false;
    bool isAgainstWallForwards = false;
    bool collisionEnterAgainstWallBackwards = false;
    bool collisionEnterAgainstWallForwards = false;
    float colliderHeight;
    float colliderRadius;
    Coroutine knockBackCoroutine;

    Rigidbody rb;
    ConfigurableJoint joint;
    Collider collider;
    Vector3 feetPosition;
    HealthSystem healthSystem;
    PathKeeper keeper;
    GameObject cameraTarget;

    public void runForwards()
    {
        runDirection = 1;
    }
    public void runBackwards()
    {
        runDirection = -1;
    }
    public void stop()
    {
        runDirection = 0;
    }
    public void flyUp()
    {
        verticalDirection = 1;
    }
    public void flyDown()
    {
        verticalDirection = -1;
    }

    public void knockBack()
    {
        if (knockBackCoroutine != null) StopCoroutine(knockBackCoroutine);
        knockBackCoroutine = StartCoroutine(stopRunningAfterDamageKnockback());
    }

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        joint = gameObject.GetComponent<ConfigurableJoint>();
        collider = gameObject.GetComponent<Collider>();
        feetPosition = new Vector3(0, (-collider.bounds.extents.y) * .9f, 0);
        colliderHeight = collider.bounds.extents.y;
        colliderRadius = collider.bounds.extents.z;
        keeper = GetComponent<PathKeeper>();
        runDirection = 0;
        verticalDirection = 0;
        invertDir = 1f;
    }
	
	// Update is called once per frame
	void Update () {

        invertDir = runDirection == -1 ? -1f : 1f;

        //laita pelaajan suunta polun suuntaiseksi
        Quaternion direction = Quaternion.LookRotation(runDirection == -1 ? -keeper.outHorTangent : keeper.outHorTangent, Vector3.up);
        transform.rotation = direction;

        RaycastHit hit;

        //törmäystarkistukset seiniin, juoksuvoima pois jos seinää vasten
        float rayDistance = colliderRadius * 1.2f;
        Vector3 verticalOffset = Vector3.up * colliderHeight * .6f;
        Vector3 startPoint = transform.position + verticalOffset;

        bool raycastWallBackward = false;
        bool raycastWallForward = false;

        raycastWallBackward |= raycastLevelCollision(startPoint, -invertDir * transform.forward, rayDistance);
        raycastWallForward |= raycastLevelCollision(startPoint, invertDir * transform.forward, rayDistance);

        startPoint = transform.position - verticalOffset;

        raycastWallBackward |= raycastLevelCollision(startPoint, -invertDir * transform.forward, rayDistance);
        raycastWallForward |= raycastLevelCollision(startPoint, invertDir * transform.forward, rayDistance);

        isAgainstWallBackwards = raycastWallBackward || collisionEnterAgainstWallBackwards;
        isAgainstWallForwards = raycastWallForward || collisionEnterAgainstWallForwards;

        if (showDebug && isAgainstWallBackwards) Debug.DrawLine(transform.position, transform.position - invertDir * transform.forward, Color.blue);
        if (showDebug && isAgainstWallForwards) Debug.DrawLine(transform.position, transform.position + invertDir * transform.forward, Color.blue);

    }

    bool raycastLevelCollision(Vector3 start, Vector3 dir, float dist)
    {
        RaycastHit hit;
        if (showDebug) Debug.DrawLine(start, start + dist * dir, Color.yellow);
        return Physics.Raycast(start, dir, out hit, dist) && hit.collider.gameObject.CompareTag("LevelCollision");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("LevelCollision"))
        {
            collisionEnterAgainstWallBackwards = false;
            collisionEnterAgainstWallForwards = false;

            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 normal = contact.normal;
                if (Vector3.Dot(-normal, invertDir * transform.forward) > againstWallThreshold)
                {
                    collisionEnterAgainstWallForwards = true;
                    //Debug.Log("forwards wall collision");
                }
                if (Vector3.Dot(-normal, -invertDir * transform.forward) > againstWallThreshold)
                {
                    collisionEnterAgainstWallBackwards = true;
                    //Debug.Log("backwards wall collision");
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("LevelCollision"))
        {
            collisionEnterAgainstWallBackwards = false;
            collisionEnterAgainstWallForwards = false;
            //Debug.Log("exit collision");
        }
    }

    private void FixedUpdate()
    {
        float relativeDT = Time.deltaTime / Time.timeScale;
        //juoksemisen voimat
        if (!damageKnockbackStopRunning) //knockback pysäyttää juoksemisen vähäksi aikaa
        {
            //voima eteen ja taakse
            Vector3 runForceAdd = keeper.outHorTangent * runForce * relativeDT;
            if (runDirection == -1 && -keeper.outPathVelocity < runSpeed && !isAgainstWallBackwards) rb.AddForce(-runForceAdd);
            else if (runDirection == 1 && keeper.outPathVelocity < runSpeed && !isAgainstWallForwards) rb.AddForce(runForceAdd);
            //voima ylös ja alas
            Vector3 vertForceAdd = Vector3.up * verticalForce * relativeDT;
            float vertSpeed = Vector3.Dot(rb.velocity, Vector3.up);
            if (verticalDirection == -1 && vertSpeed > -verticalSpeed) rb.AddForce(-vertForceAdd);
            else if (verticalDirection == 1 && vertSpeed < verticalSpeed) rb.AddForce(vertForceAdd);
        }
        
    }

    IEnumerator stopRunningAfterDamageKnockback()
    {
        damageKnockbackStopRunning = true;
        
        yield return new WaitForSeconds(knockBackTime);
        damageKnockbackStopRunning = false;
    }
}

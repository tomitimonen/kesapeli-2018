using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public GameObject follow;
    public float followSpeed = .05f;
    public float cameraDistance = 5f;
    public float cameraHeight = 3f;
    Vector3 prevPos;
	// Use this for initialization
	void Start () {
        prevPos = follow.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 newpos = follow.transform.position + follow.transform.right * cameraDistance + Vector3.up * cameraHeight;
        Vector3 prediction = (follow.transform.position - prevPos);
        transform.position = Vector3.Slerp(transform.position, newpos + prediction / followSpeed, followSpeed);

        transform.LookAt(follow.transform, Vector3.up);

        prevPos = follow.transform.position;
	}

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        Renderer rend = obj.GetComponent<Renderer>();
        rend.enabled = false;
        Debug.Log("enter");
    }
}

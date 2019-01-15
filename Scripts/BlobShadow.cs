using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobShadow : MonoBehaviour {


    public GameObject shadowPlane;
    public float shadowHeight;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        shadowPlane.transform.position = new Vector3(transform.position.x, shadowHeight, transform.position.z);
        shadowPlane.transform.rotation = Quaternion.identity;

    }
}

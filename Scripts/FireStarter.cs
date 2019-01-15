using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStarter : MonoBehaviour {

    public float startSpeed;
    public GameObject fireObject;

    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * startSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("LevelCollision"))
        {
            Vector3 hitPos = collision.contacts[0].point;
            Instantiate(fireObject, hitPos, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : Weapon {

    public int damageValue = 1;
    public float startSpeed = 20f;
    public bool destroyOnHit = true;

    Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * startSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude > 0.0001f)
        { 
            Quaternion newdir = Quaternion.LookRotation(rb.velocity, Vector3.up);
            transform.rotation = newdir;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyOnHit) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 knockBackDir = (other.transform.position - transform.position).normalized;
        bool success = Attack(other.gameObject, damageValue, knockBackDir);
        if (success)
        {
            if (destroyOnHit) Destroy(gameObject);
            //osumaefekti iskulle tai muuta vastaavaa
        }
    }
}

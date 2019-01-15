using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaker : MonoBehaviour, IDamageable {

    public GameObject owner;
    IDamageable ownerDamageable;
	// Use this for initialization
	void Start () {
        ownerDamageable = owner.GetComponent(typeof(IDamageable)) as IDamageable;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsAlive()
    {
        return ownerDamageable.IsAlive();
    }

    public void TakeDamage(int amount, Vector3 knockBackDir)
    {
        ownerDamageable.TakeDamage(amount, knockBackDir);
    }
}

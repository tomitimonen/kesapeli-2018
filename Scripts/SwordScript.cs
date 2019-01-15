using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : Weapon {

    public int damageValue = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Vector3 knockBackDir = (other.transform.position - transform.position).normalized;
        bool success = Attack(other.gameObject, damageValue, knockBackDir);
        if (success)
        {
            //osumaefekti iskulle tai muuta vastaavaa
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareCrowEnemy : EnemyBase, IDamageable {

    //public GameObject deathEffect;

    bool isAlive = true;

    // Use this for initialization
    protected override void Start () {
        base.Start();
    }
	

    public override void TakeDamage(int damage, Vector3 knockBackDirection)
    {
        base.TakeDamage(damage, knockBackDirection);
        OnDie();
        //Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

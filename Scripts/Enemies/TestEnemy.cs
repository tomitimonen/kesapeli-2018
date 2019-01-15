using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : EnemyBase, IDamageable {

    bool isAlive = true;
    PathKeeper keeper;
    GameObject target;
    IDamageable targetDamageable;
    PathKeeper targetKeeper;
    Rigidbody rb;
    GroundMovement movement;

    GameObject weapon;
    Quaternion weaponRotStart = Quaternion.AngleAxis(-90f, Vector3.right) * Quaternion.AngleAxis(-30f, Vector3.up);
    Quaternion weaponRotEnd = Quaternion.AngleAxis(0f, Vector3.right) * Quaternion.AngleAxis(-30f, Vector3.up);
    bool slash = false;
    float slashRate = 5f;
    float slashAlpha = 0f;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        keeper = GetComponent<PathKeeper>();
        target = GameObject.Find("Player");
        targetDamageable = target.GetComponent<IDamageable>();
        targetKeeper = target.GetComponent<PathKeeper>();
        rb = GetComponent<Rigidbody>();
        weapon = transform.Find("SwordPivotEnemy").gameObject;
        movement = GetComponent<GroundMovement>();
    }
	
	// Update is called once per frame
	void Update () {
        
		if (target != null && targetKeeper != null)
        {
            //jos pelaaja ja vihollinen ovat samalla polulla niin juokse pelaajaa kohti
            if (targetKeeper.outCurrentSpline == keeper.outCurrentSpline && targetDamageable.IsAlive())
            {
                float targetRelPos = targetKeeper.outRelativePosition;
                float relPos = keeper.outRelativePosition;

                if (relPos > targetRelPos) moveBackwards();
                else moveForwards();
            }

        }

        //miekan pyöritys, korvautuu myöhemmin animaatiolla
        if (slash)
        {
            slashAlpha += slashRate * Time.deltaTime;
            if (slashAlpha >= 1f)
            {
                slashAlpha = 0f;
                slash = false;
            }
        }
        Quaternion weaponRot = Quaternion.Slerp(weaponRotStart, weaponRotEnd, slashAlpha);
        weapon.transform.rotation = transform.rotation * weaponRot;

        slash = true;
    }

    void moveForwards()
    {
        movement.runForwards();
    }

    void moveBackwards()
    {
        movement.runBackwards();
    }

    void stop()
    {
        movement.stop();
    }

    public override void TakeDamage(int damage, Vector3 knockBackDirection)
    {
        base.TakeDamage(damage, knockBackDirection);
        /*rb.constraints = RigidbodyConstraints.None;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;
        isAlive = false;
        keeper.enabled = false;*/
        OnDie();
        Destroy(gameObject);
    }
}

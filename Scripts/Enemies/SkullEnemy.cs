using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullEnemy : EnemyBase, IDamageable {

    public float attackDistance = 5f;
    public float attackTime = .5f;
    public float attackStartDelay = .5f;
    public float flyHeight = 1f;
    public GameObject projectile;
    public GameObject projectileBone;
    //public GameObject deathEffect;

    float lastAttackTime = 0f;

    bool isAlive = true;
    PathKeeper keeper;
    GameObject target;
    IDamageable targetDamageable;
    PathKeeper targetKeeper;
    Rigidbody rb;
    AirMovement movement;

    Animator anim;

    //animaatiot
    float animIdleRunBlendRate = 12f;
    float animIdleRunBlend = 0f;


    // Use this for initialization
    protected override void Start () {
        base.Start();
        keeper = GetComponent<PathKeeper>();
        target = GameObject.Find("Player");
        targetDamageable = target.GetComponent<IDamageable>();
        targetKeeper = target.GetComponent<PathKeeper>();
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<AirMovement>();

        anim = GetComponentInChildren<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        
		if (target != null && targetKeeper != null && targetDamageable.IsAlive())
        {
            //jos pelaaja ja vihollinen ovat samalla polulla niin juokse pelaajaa kohti
            if (targetKeeper.outCurrentSpline == keeper.outCurrentSpline)
            {
                float targetRelPos = targetKeeper.outRelativePosition;
                float relPos = keeper.outRelativePosition;

                if (relPos > targetRelPos) moveBackwards();
                else moveForwards();

                if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                {
                    if (Time.time - lastAttackTime >= attackTime)
                    {
                        lastAttackTime = Time.time;
                        /*Vector3 startPos = projectileBone.transform.position;
                        Quaternion startRot = Quaternion.LookRotation(projectileBone.transform.forward);
                        Instantiate(projectile, startPos, startRot);*/
                        StartCoroutine(AttackCoroutine());
                        anim.SetTrigger("Shoot");
                    }
                    //lyöntianimaatio
                    //anim.SetTrigger("Slash");
                }
            }

            //liike ylös ja alas, kallo pyrkii väistämään esteitä
            RaycastHit hit;
            Debug.DrawLine(transform.position, -Vector3.up*flyHeight, Color.red);
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, flyHeight) && hit.collider.gameObject.CompareTag("LevelCollision"))
            {
                movement.flyUp();
            }
            else movement.flyDown();

        }

        //juoksu- ja idleanimaatioiden blendaus BaseLayerissa
        //anim.SetFloat("RunBlend", Mathf.Lerp(anim.GetFloat("RunBlend"), animIdleRunBlend, animIdleRunBlendRate * Time.deltaTime));

    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackStartDelay);
        Vector3 startPos = projectileBone.transform.position;
        Quaternion startRot = Quaternion.LookRotation(projectileBone.transform.forward);
        Instantiate(projectile, startPos, startRot);
    }

    void moveUp()
    {

    }
    void moveDown()
    {

    }
    void moveForwards()
    {
        movement.runForwards();
        animIdleRunBlend = 1f;
    }

    void moveBackwards()
    {
        movement.runBackwards();
        animIdleRunBlend = 1f;
    }

    void stop()
    {
        movement.stop();
        animIdleRunBlend = 0f;
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
        //Instantiate(deathEffect, transform.position, transform.rotation);
        OnDie();
        Destroy(gameObject);
    }
}

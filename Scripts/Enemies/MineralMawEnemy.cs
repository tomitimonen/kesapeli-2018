using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralMawEnemy : EnemyBase, IDamageable {

    public float attackDistance = 5f;
    public float attackTime = .5f;
    public float attackStartDelay = .3f;
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
    GroundMovement movement;
    HealthSystem healthSystem;
    MaterialBlinker damageBlinker;
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
        movement = GetComponent<GroundMovement>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.dieHandler = OnDie;
        damageBlinker = GetComponent<MaterialBlinker>();

        anim = GetComponentInChildren<Animator>();
        anim.SetFloat("RunBlend", 0f);

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
                        anim.SetTrigger("Slash");
                        //ammus ilmestyy vasta viiveen jälkeen
                        StartCoroutine("AttackCoroutine");
                    }

                }
            }

        }

        //juoksu- ja idleanimaatioiden blendaus BaseLayerissa
        anim.SetFloat("RunBlend", Mathf.Lerp(anim.GetFloat("RunBlend"), animIdleRunBlend, animIdleRunBlendRate * Time.deltaTime));

    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackStartDelay);
        Vector3 startPos = projectileBone.transform.position;
        Quaternion startRot = Quaternion.LookRotation(projectileBone.transform.forward);
        Instantiate(projectile, startPos, startRot);
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

    protected override void OnDie()
    {
        base.OnDie();
        Destroy(gameObject);
    }

    public override void TakeDamage(int damage, Vector3 knockBackDirection)
    {
        base.TakeDamage(damage, knockBackDirection);
        //Instantiate(deathEffect, transform.position, transform.rotation);
        damageBlinker.Blink();
        Vector3 moveDir = Vector3.Project(knockBackDirection, movement.invertDir * keeper.outHorTangent) + Vector3.up * .2f;
        moveDir = moveDir.normalized;
        rb.velocity = moveDir * 8f;
        movement.knockBack();
    }

}

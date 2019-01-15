using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobBoiEnemy : EnemyBase, IDamageable {

    public float attackDistance = 2f;

    bool isAlive = true;
    PathKeeper keeper;
    GameObject target;
    IDamageable targetDamageable;
    PathKeeper targetKeeper;
    Rigidbody rb;
    GroundMovement movement;

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

        anim = transform.Find("BlobBoiModel").GetComponent<Animator>();
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
                    //lyöntianimaatio
                    anim.SetTrigger("Slash");
                }
            }

        }

        //juoksu- ja idleanimaatioiden blendaus BaseLayerissa
        anim.SetFloat("RunBlend", Mathf.Lerp(anim.GetFloat("RunBlend"), animIdleRunBlend, animIdleRunBlendRate * Time.deltaTime));

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
        OnDie();
        Destroy(gameObject);
    }
}

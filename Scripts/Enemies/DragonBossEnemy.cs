using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class DragonBossEnemy : MonoBehaviour, IDamageable {

    public float attackDistance = 5f;
    public float attackTime = .5f;
    public ParticleSystem fireBreathParticleSystem;
    public GameObject fireBall;
    public GameObject headBone;
    public GameObject fireStarter;
    public GameObject tailWeapon;
    public GameObject headWeapon;
    public GameObject damageTaker;
    public GameObject shadowHeightObject;
    public Canvas endingCanvas;
    //public GameObject deathEffect;

    float lastAttackTime = 0f;

    bool isAlive = true;
    GameObject target;
    BezierSpline targetPath;

    enum BossState
    {
        Init,
        TailSweep,
        FireRain,
        FireTrail,
        Bite,
        Paralyzed,
        Dead
    }

    BossState bossState = BossState.Init;

    delegate void stateEntry();
    Dictionary<BossState, stateEntry> stateEntries;
    delegate BossState stateUpdate();
    Dictionary<BossState, stateUpdate> stateUpdates;
    delegate void stateExit();
    Dictionary<BossState, stateExit> stateExits;

    int tailSweepCount = 0;

    bool initDone = false;
    bool tailSweepDone = false;
    bool fireBreathDone = false;
    bool fireRainDone = false;
    bool biteDone = false;
    bool paralyzedDone = false;

    bool faceTarget = true;

    HealthSystem healthSystem;

    Animator anim;

    //animaatiot

    //tilojen päivitysmetodit
    BossState initStateUpdate()
    {
        BossState nextState;
        if (initDone) nextState = BossState.TailSweep;
        else nextState = BossState.Init;
        return nextState;
    }
    BossState tailSweepStateUpdate()
    {
        BossState nextState;
        if (tailSweepDone) nextState = BossState.FireRain;
        else nextState = BossState.TailSweep;
        return nextState;
    }
    BossState fireRainStateUpdate()
    {
        BossState nextState = BossState.FireRain;

        if (fireRainDone)
        {
            if (tailSweepCount < 1)
            {
                tailSweepCount++;
                nextState = BossState.TailSweep;
            }
            else
            {
                tailSweepCount = 0;
                nextState = BossState.FireTrail;
            }
        }
        return nextState;
    }
    BossState fireTrailStateUpdate()
    {
        BossState nextState;
        if (fireBreathDone) nextState = BossState.Bite;
        else nextState = BossState.FireTrail;
        return nextState;
    }
    BossState biteStateUpdate()
    {
        BossState nextState;
        if (biteDone) nextState = BossState.Paralyzed;
        else nextState = BossState.Bite;
        return nextState;
    }
    BossState paralyzedStateUpdate()
    {
        BossState nextState;
        if (paralyzedDone) nextState = isAlive ? BossState.TailSweep : BossState.Dead;
        else nextState = BossState.Paralyzed;
        return nextState;
    }
    BossState deadStateUpdate()
    {
        return BossState.Dead;
    }
    //tilojen aloitusmetodit
    void initStateEntry()
    {
        initDone = false;
        StartCoroutine(stopInit());
    }
    void tailSweepStateEntry()
    {
        tailSweepDone = false;
        faceTarget = false;
        tailWeapon.SetActive(true);
        anim.SetTrigger("TailSweep");
        StartCoroutine(stopTailSweep());
        StartCoroutine(disableTailWeapon());
    }
    void fireRainStateEntry()
    {
        fireRainDone = false;
        faceTarget = true;
        StartCoroutine(spawnFireBalls());
    }
    void fireTrailStateEntry()
    {
        fireBreathDone = false;
        fireBreathParticleSystem.Play();
        StartCoroutine(stopFireBreath());
    }
    void biteStateEntry()
    {
        biteDone = false;
        faceTarget = true;
        headWeapon.SetActive(true);
        anim.SetTrigger("Bite");
        StartCoroutine(stopBite());
    }
    void paralyzedStateEntry()
    {
        paralyzedDone = false;
        damageTaker.SetActive(true);
        anim.SetTrigger("FallOver");
        StartCoroutine(stopParalyzed());
    }
    //tilojen lopetusmetodit
    void tailSweepStateExit()
    {
        //tailWeapon.SetActive(false);
    }
    void biteStateExit()
    {
        headWeapon.SetActive(false);
        faceTarget = false;
    }
    void paralyzedStateExit()
    {
        damageTaker.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.dieHandler = OnDie;
        anim = GetComponentInChildren<Animator>();
        target = GameObject.Find("Player");
        //listat tilojen metodeista
        stateEntries = new Dictionary<BossState, stateEntry>();
        stateUpdates = new Dictionary<BossState, stateUpdate>();
        stateExits = new Dictionary<BossState, stateExit>();
        //tilojen päivitysmetodit
        stateUpdates[BossState.Init] = initStateUpdate;
        stateUpdates[BossState.TailSweep] = tailSweepStateUpdate;
        stateUpdates[BossState.FireRain] = fireRainStateUpdate;
        stateUpdates[BossState.FireTrail] = fireTrailStateUpdate;
        stateUpdates[BossState.Bite] = biteStateUpdate;
        stateUpdates[BossState.Paralyzed] = paralyzedStateUpdate;
        stateUpdates[BossState.Dead] = deadStateUpdate;
        //tilojen aloitusmetodit
        stateEntries[BossState.Init] = initStateEntry;
        stateEntries[BossState.TailSweep] = tailSweepStateEntry;
        stateEntries[BossState.FireRain] = fireRainStateEntry;
        stateEntries[BossState.FireTrail] = fireTrailStateEntry;
        stateEntries[BossState.Bite] = biteStateEntry;
        stateEntries[BossState.Paralyzed] = paralyzedStateEntry;
        //tilojen lopetusmetodit
        stateExits[BossState.TailSweep] = tailSweepStateExit;
        stateExits[BossState.Bite] = biteStateExit;
        stateExits[BossState.Paralyzed] = paralyzedStateExit;

        if (stateEntries.ContainsKey(bossState)) stateEntries[bossState]();
    }
	
	// Update is called once per frame
	void Update () {
        
		if (target != null)
        {
            //päivitä tilakone
            if (!stateUpdates.ContainsKey(bossState))
            {
                Debug.LogError("DragonBossEnemy - No update method defined for state"+bossState.ToString());
            }
            stateUpdate updateFunc = stateUpdates[bossState];
            BossState newState = updateFunc();
            if (newState != bossState)
            {
                Debug.Log("BossState: "+newState.ToString());
                if (stateExits.ContainsKey(bossState)) stateExits[bossState]();
                if (stateEntries.ContainsKey(newState))  stateEntries[newState]();
                bossState = newState;
            }
        }
    }

    void FixedUpdate()
    {
        if (faceTarget)
        {
            Vector3 forward = target.transform.position - headBone.transform.position;
            forward = Vector3.ProjectOnPlane(forward, Vector3.up);
            Quaternion targetDir = Quaternion.LookRotation(forward, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetDir, .05f);
        } 
    }
    IEnumerator stopInit()
    {
        yield return new WaitForSeconds(1.5f);
        initDone = true;
    }
    IEnumerator stopTailSweep()
    {
        yield return new WaitForSeconds(3f);
        tailSweepDone = true;
    }
    IEnumerator disableTailWeapon()
    {
        yield return new WaitForSeconds(2f);
        tailWeapon.SetActive(false);
    }
    IEnumerator stopFireBreath()
    {
        for (int i=0; i<10; i++)
        {
            Instantiate(fireStarter, fireBreathParticleSystem.gameObject.transform.position, fireBreathParticleSystem.gameObject.transform.rotation);
            yield return new WaitForSeconds(.2f);
        }
        fireBreathParticleSystem.Stop();
        fireBreathDone = true;
    }
    IEnumerator stopBite()
    {
        yield return new WaitForSeconds(1f);
        biteDone = true;
    }
    IEnumerator stopParalyzed()
    {
        yield return new WaitForSeconds(5f);
        if (!paralyzedDone) anim.SetTrigger("GetUp");
        yield return new WaitForSeconds(2f);
        paralyzedDone = true;
    }

    IEnumerator startEnding()
    {
        yield return new WaitForSeconds(5f);
        endingCanvas.enabled = true;

    }


    IEnumerator spawnFireBalls()
    {
        targetPath = target.GetComponent<PathKeeper>().currentSpline;

        for (int i=0; i<6; i++)
        {
            Vector3 targetPos = targetPath.GetPoint(Random.value);
            Vector3 startPos = new Vector3(targetPos.x + Random.Range(-2f, 2f), transform.position.y + 10f, targetPos.z + Random.Range(-2f, 2f));
            //Vector3 startPos = transform.position + Vector3.up * 8f + Vector3.right * Random.Range(-3f, 3f) + Vector3.forward * Random.Range(-3f, 3f);
            Vector3 forward = /*target.transform.position*/targetPos - startPos;
            Quaternion dir = Quaternion.LookRotation(forward);
            GameObject fb = Instantiate(fireBall, startPos, dir);
            BlobShadow shadow = fb.GetComponent<BlobShadow>();
            shadow.shadowHeight = shadowHeightObject.transform.position.y;
            yield return new WaitForSeconds(.5f);
        }
        yield return new WaitForSeconds(3f);
        fireRainDone = true;
        
    }


    public void TakeDamage(int damage, Vector3 knockBackDirection)
    {
        /*rb.constraints = RigidbodyConstraints.None;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;
        isAlive = false;
        keeper.enabled = false;*/
        //Instantiate(deathEffect, transform.position, transform.rotation);
        Debug.Log("Damaged dragon");
        healthSystem.TakeDamage(damage);
        paralyzedDone = true;
        anim.SetTrigger("GetUp");
        //Destroy(gameObject);
    }
    void OnDie()
    {
        //Destroy(gameObject);
        anim.SetBool("Die", true);
        if (isAlive)
        {
            isAlive = false;
            StartCoroutine(startEnding());
        }
    }
    public bool IsAlive()
    {
        return isAlive;
    }
}

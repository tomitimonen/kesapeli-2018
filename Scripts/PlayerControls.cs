using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class PlayerControls : MonoBehaviour, IDamageable {

    public bool showDebug = false;
    public GameObject healEffect;
    public GameObject swordHitbox;

    public AudioSource footStepSound;
    public AudioSource slashSound;
    public AudioSource hurtSound;
    public AudioSource jumpSound;
    public AudioSource deathSound;
    public AudioSource landingSound;
    public AudioSource spellAudioSource;

    Rigidbody rb;
    HealthSystem healthSystem;
    PathKeeper keeper;
    GameObject cameraTarget;
    GroundMovement movement;
    Animator anim;
    MaterialBlinker damageBlinker;
    FootstepHandler footStepHandler;

    bool isAlive = true;

    //animaatiot
    float animIdleRunBlendRate = 12f;
    float animLayerBlendRate = 12f;

    float animIdleRunBlend = 0f;
    float animBaseLayerWeight = 1f;
    float animFallLayerWeight = 0f;
    float animHurtLayerWeight = 0f;
    float animJumpLayerWeight = 0f;
    float animLandingLayerWeight = 0f;

    int animBaseLayer;
    int animFallLayer;
    int animHurtLayer;
    int animJumpLayer;
    int animLandingLayer;

    GameObject mainCamera;
    GameObject cmDefaultCamera;

    // Use this for initialization
    void Start () {
        //haetaan kaikki tarvittavat komponentit
        rb = GetComponent<Rigidbody>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.dieHandler = OnDie;
        keeper = GetComponent<PathKeeper>();
        cameraTarget = GameObject.Find("CameraTarget");
        movement = GetComponent<GroundMovement>();
        movement.OnHitGround = OnHitGround;
        anim = transform.Find("PlayerModel").GetComponent<Animator>();
        damageBlinker = GetComponent<MaterialBlinker>();
        footStepHandler = transform.Find("PlayerModel").GetComponent<FootstepHandler>();
        footStepHandler.FootStepHandler = Footstep;

        //animaatiot
        anim.SetFloat("RunBlend", 0f);
        animBaseLayer = anim.GetLayerIndex("BaseLayer");
        animFallLayer = anim.GetLayerIndex("FallLayer");
        animHurtLayer = anim.GetLayerIndex("HurtLayer");
        animJumpLayer = anim.GetLayerIndex("JumpLayer");
        animLandingLayer = anim.GetLayerIndex("LandingLayer");

        anim.GetBehaviour<AttackLayerScript>().OnSlashEndHandler = OnSlashEnd;
        swordHitbox.SetActive(false);

        //lataa pelaajan tilanne tallennuksesta
        GameData.Player playerData = GameDataManager.Instance.GameData.player;
        if (playerData.firstCheckPointReached)
        {
            //paikka, suunta ja polku
            transform.position = playerData.position.toVector3();
            transform.rotation = playerData.rotation.toQuaternion();
            keeper.currentSpline = GameObject.Find(playerData.pathName).GetComponent<BezierSpline>();
            //health
            healthSystem.currentHealth = playerData.currentHealth;
            healthSystem.maxHealth = playerData.maxHealth;

        }

        //siirrä kamera pelaajan paikalle
        cmDefaultCamera = GameObject.FindGameObjectWithTag("CMDefaultCamera");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (cmDefaultCamera != null)
        {
            cmDefaultCamera.SetActive(false);
            mainCamera.SetActive(false);
            cmDefaultCamera.transform.position = transform.position;
            mainCamera.transform.position = transform.position;
            StartCoroutine(ActivateCameras());
        }

    }

    public void MoveCameras()
    {
        if (cmDefaultCamera != null)
        {
            cmDefaultCamera.SetActive(false);
            mainCamera.SetActive(false);
            cmDefaultCamera.transform.position = transform.position;
            mainCamera.transform.position = transform.position;
            StartCoroutine(ActivateCameras());
        }
    }

    IEnumerator ActivateCameras()
    {
        yield return new WaitForSeconds(.1f);
        mainCamera.SetActive(true);
        cmDefaultCamera.SetActive(true);
    }
    public void OnSlashEnd()
    {
        swordHitbox.SetActive(false);
    }

    public void Slash()
    {
        swordHitbox.SetActive(true);
        anim.SetTrigger("Slash");
        slashSound.Play();
    }

    public void OnHitGround()
    {
        if (!anim.GetBool("IsLanding"))
        {
            anim.SetTrigger("Land");
            landingSound.Play();
        }
    }

    public void Footstep()
    {
        if (movement.onGround)
        {
            footStepSound.Play();
        }
    }

    void OnDie()
    {
        if (isAlive)
        {
            rb.constraints = RigidbodyConstraints.None;
            //joint.angularYMotion = ConfigurableJointMotion.Free;
            //joint.zMotion = ConfigurableJointMotion.Free;
            //joint.zMotion = ConfigurableJointMotion.Free;
            isAlive = false;
            keeper.enabled = false;
            movement.enabled = false;
            anim.enabled = false;
            deathSound.Play();
            //Debug.Log("IsAlive: " + isAlive);
        }
    }
	
	// Update is called once per frame
	void Update () {

        //animaatiot
        //juoksu- ja idleanimaatioiden blendaus BaseLayerissa
        anim.SetFloat("RunBlend", Mathf.Lerp(anim.GetFloat("RunBlend"), animIdleRunBlend, animIdleRunBlendRate * Time.deltaTime));
        //layereiden blendaus
        anim.SetLayerWeight(animBaseLayer, Mathf.Lerp(anim.GetLayerWeight(animBaseLayer), animBaseLayerWeight, animLayerBlendRate * Time.deltaTime));
        anim.SetLayerWeight(animFallLayer, Mathf.Lerp(anim.GetLayerWeight(animFallLayer), animFallLayerWeight, animLayerBlendRate * Time.deltaTime));
        anim.SetLayerWeight(animHurtLayer, Mathf.Lerp(anim.GetLayerWeight(animHurtLayer), animHurtLayerWeight, animLayerBlendRate * Time.deltaTime));
        anim.SetLayerWeight(animJumpLayer, Mathf.Lerp(anim.GetLayerWeight(animJumpLayer), animJumpLayerWeight, animLayerBlendRate * Time.deltaTime * 2f));
        anim.SetLayerWeight(animLandingLayer, Mathf.Lerp(anim.GetLayerWeight(animLandingLayer), animLandingLayerWeight, animLayerBlendRate * Time.deltaTime * 2f));

        bool hurting = anim.GetBool("IsHurting");
        bool jumping = anim.GetBool("IsJumping");
        bool landing = anim.GetBool("IsLanding");
        if (hurting)
        {
            animBaseLayerWeight = 0f;
            animHurtLayerWeight = 1f;
            animLandingLayerWeight = 0f;
        }
        else if (jumping)
        {
            animBaseLayerWeight = 0f;
            animJumpLayerWeight = 1f;
            animHurtLayerWeight = 0f;
            animLandingLayerWeight = 0f;
        }
        else if (landing)
        {
            animBaseLayerWeight = 0f;
            animJumpLayerWeight = 0f;
            animHurtLayerWeight = 0f;
            animLandingLayerWeight = 1f;
        }
        else
        {
            animBaseLayerWeight = 1f;
            animHurtLayerWeight = 0f;
            animJumpLayerWeight = 0f;
            animLandingLayerWeight = 0f;
        }
        if (rb.velocity.y < 0 && !movement.onGround)
        {
            animBaseLayerWeight = 0f;
            if (!hurting) animFallLayerWeight = 1f;
            anim.SetBool("Falling", true);
        }
        else
        {
            if (!hurting) animBaseLayerWeight = 1f;
            animFallLayerWeight = 0f;
            anim.SetBool("Falling", false);
        }

    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            //aseta cameratargetin paikka ja suunta pelaajan paikalle ja polkua pitkin eteen päin
            cameraTarget.transform.position = transform.position;
            cameraTarget.transform.rotation = Quaternion.LookRotation(movement.invertDir * transform.forward, Vector3.up);

        }
    }


    public void SwipeLeft()
    {
        if (isAlive)
        {
            if (movement.runDirection == -1)
            {
                Slash();
            }
            else
            {
                movement.runBackwards();
            }
            animIdleRunBlend = 1f;
        }
        
    }
    public void SwipeRight()
    {
        if (isAlive)
        {
            if (movement.runDirection == 1)
            {
                Slash();
            }
            else
            {
                movement.runForwards();
            }
            animIdleRunBlend = 1f;
        }
    }
    public void SwipeUp()
    {
        //text.text = "Swipe Up";
    }
    public void SwipeDown()
    {
        /*movement.stop();
        animIdleRunBlend = 0f;*/
    }
    public void Tap(float power)
    {
        //text.text = "Tap";
        if (isAlive)
        {
            movement.jump(power);
            anim.SetTrigger("Jump");
            jumpSound.Play();
        }
    }

    public void CastSpell(int spellNumber, Spell spell)
    {
        Debug.Log("Cast Spell " + spellNumber);
        switch(spellNumber)
        {
            case 0:
                healthSystem.currentHealth = healthSystem.maxHealth;
                Instantiate(healEffect, transform);
                spellAudioSource.clip = spell.castSound;
                spellAudioSource.Play();
                break;

            default:
                break;
        }
    }

    public void TakeDamage(int damage, Vector3 knockBackDirection)
    {
        healthSystem.TakeDamage(damage);
        damageBlinker.Blink();
        if (showDebug) Debug.Log("Current Health: " + healthSystem.currentHealth);
        anim.SetTrigger("Hurt");
        Vector3 moveDir = Vector3.Project(knockBackDirection, movement.invertDir * keeper.outHorTangent)+Vector3.up*.2f;
        moveDir = moveDir.normalized;
        //rb.AddForce(moveDir * 3f, ForceMode.VelocityChange);
        rb.velocity = moveDir * 4f;
        movement.knockBack();
        if (isAlive) hurtSound.Play();

    }
    public bool IsAlive()
    {
        return isAlive;
    }

}

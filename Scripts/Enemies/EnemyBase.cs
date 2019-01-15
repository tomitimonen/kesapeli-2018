using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable {

    public AudioSource hurtSound;
    public GameObject deathEffect;

    private PersistentID id;
    private HealthSystem baseHealthSystem;
    //pelin tallennukseen tarvittavat tiedot
    private GameData gameData;
    private GameData.Enemy enemyData;

    // Use this for initialization
    protected virtual void Start () {
        //tarkista ja päivitä vihollisen tiedot pelin tallennuksessa
        baseHealthSystem = GetComponent<HealthSystem>();
        id = GetComponent<PersistentID>();
        gameData = GameDataManager.Instance.GameData;

        if (!gameData.enemies.ContainsKey(id.id))
        {
            gameData.enemies[id.id] = new GameData.Enemy();
            enemyData = gameData.enemies[id.id];
            enemyData.isAlive = true;
            if (baseHealthSystem != null) enemyData.health = baseHealthSystem.currentHealth;

        }
        else
        {
            enemyData = gameData.enemies[id.id];
            if (!enemyData.isAlive)
            {
                //Debug.Log("EnemyBase - "+id.id+" Not Alive");
                Destroy(gameObject);
            }
            else
            {
                if (baseHealthSystem != null) baseHealthSystem.currentHealth = enemyData.health;
            }
        }
	}

    public virtual void TakeDamage(int damage, Vector3 knockBackDirection)
    {
        if (baseHealthSystem != null)
        {
            baseHealthSystem.TakeDamage(damage);
            enemyData.health = baseHealthSystem.currentHealth;
            //Debug.Log("EnemyBase - Set health in GameData to " + enemyData.health);
        }
        if (hurtSound != null)
        {
            if (baseHealthSystem != null && baseHealthSystem.currentHealth <= 0) return;
            hurtSound.Play();
        }
    }

    public virtual bool IsAlive()
    {
        return true;
    }


    protected virtual void OnDie()
    {
        enemyData.isAlive = false;
        if (deathEffect != null) Instantiate(deathEffect, transform.position, transform.rotation);
    }

}

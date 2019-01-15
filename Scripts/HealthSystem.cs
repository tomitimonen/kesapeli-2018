using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {

    public int maxHealth;
    public int currentHealth;
    public delegate void OnDie();
    public OnDie dieHandler;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) dieHandler();
    }

}

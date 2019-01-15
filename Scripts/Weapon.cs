using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public string targetTag;

    IDamageable getDamageable(GameObject target)
    {
        return target.GetComponent(typeof(IDamageable)) as IDamageable;

    }

    bool isAttackable(GameObject target)
    {
        return target.CompareTag(targetTag);
    }

    protected bool Attack(GameObject target, int damage, Vector3 knockBackDirection)
    {
        bool success = false;
        IDamageable victim = getDamageable(target);
        if (victim != null && isAttackable(target))
        {
            victim.TakeDamage(damage, knockBackDirection);
            success = true;
        }
        return success;
    }
}

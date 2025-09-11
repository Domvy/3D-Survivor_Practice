using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f; // 시작 체력
    public float health { get; protected set; } // 현재 체력
    public bool dead { get; protected set; }
    public event Action onDeath; // 사망 시 이벤트

    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    public virtual void Die()
    {
        if (onDeath != null) 
        {
            onDeath();
        }
        dead = true;
    }    
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            return;
        }
        health += newHealth;
    }
}

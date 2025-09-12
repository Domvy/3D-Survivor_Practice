using System;
using UnityEngine;
using Photon;
using Photon.Pun;

public class LivingEntity : MonoBehaviourPun, IDamageable
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

    [PunRPC]
    public void ApplyUpdateHealth(float newHealth, bool newDead) // 모든 플레이어 체력, 사망 상태 동기화
    {
        health = newHealth;
        dead = newDead;
    }
    public virtual void Die()
    {
        if (onDeath != null) 
        {
            onDeath();
        }
        dead = true;
    }
    [PunRPC]
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (PhotonNetwork.IsMasterClient) // 호스트만 실행
        {
            health -= damage;

            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead); // 동기화
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal); // 다른 클라이언트도 함수 실행
        }        
        if (health <= 0 && !dead)
        {
            Die();
        }
    }
    [PunRPC]
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead)
        {
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            health += newHealth;

            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, health, dead);
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);
        }
        
    }
}

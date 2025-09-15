using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;


public class Zombie : LivingEntity
{
    public LayerMask whatIsTarget; // 추적 대상 레이어
    private LivingEntity targetEntity; // 추적 대상 오브젝트
    private NavMeshAgent navMeshAgent; // 경로 계산용

    public ParticleSystem hitEffect; // 피격 효과
    public AudioClip deathSound;
    public AudioClip hitSound;

    private Animator zombieAnimator;
    private AudioSource zombieAudioPlayer;
    private Renderer zombieRenderer;

    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;

    private bool hasTarget // 추적 대상이 존재하는지 확인
    {
        get
        {
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioPlayer = GetComponent<AudioSource>();
        zombieRenderer = GetComponentInChildren<Renderer>();
    }
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return; // 호스트만 실행

        StartCoroutine(UpdatePath());
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        zombieAnimator.SetBool("HasTarget", hasTarget);
    }

    [PunRPC]
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor)
    {
        startingHealth = newHealth; // 최대 체력
        health = newHealth; // 현재 체력
        damage = newDamage; // 공격력
        navMeshAgent.speed = newSpeed; // 이동속도
        zombieRenderer.material.color = skinColor; // 외형 색
    }
    public IEnumerator UpdatePath()
    {
        while (!dead)
        {
            if (hasTarget)
            {
                navMeshAgent.isStopped = false; // 정지 해제
                navMeshAgent.SetDestination(targetEntity.transform.position); // 타겟을 향해 이동
            }
            else
            {
                navMeshAgent.isStopped = true; // 이동 정지
                /* 근처 whatIsTarget 레이어만 탐색하여 추가 */
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
                for (int i = 0; i < colliders.Length; i++)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead) // 피격 이펙트 표현
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            zombieAudioPlayer.PlayOneShot(hitSound);
        }

        base.OnDamage(damage, hitPoint, hitNormal);
    }
    public override void Die()
    {
        base.Die();

        Collider[] zombieColliders = GetComponents<Collider>(); // 콜라이더 비활성화
        for (int i = 0; i < zombieColliders.Length; i++)
        {
            zombieColliders[i].enabled = false;
        }

        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
        zombieAnimator.SetTrigger("Die");
        zombieAudioPlayer.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            LivingEntity attackTarget = other.GetComponent<LivingEntity>(); // 상대 오브젝트 타입 확인

            if (attackTarget != null && attackTarget == targetEntity)
            {
                lastAttackTime = Time.time;
                Vector3 hitPoint = other.ClosestPoint(transform.position); // 충돌 위치와 가장 가까운 내 위치 반환
                Vector3 hitNormal = transform.position - other.transform.position; // 방향 할당(나를 향한)

                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }

}

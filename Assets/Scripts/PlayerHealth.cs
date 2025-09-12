using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider; // 체력 바

    public AudioClip deathClip;
    public AudioClip hitClip;
    public AudioClip itemPickupClip;

    private AudioSource playerAudioPlayer;
    private Animator playerAnimator;

    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;

        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);

        healthSlider.value = health; // 슬라이더 갱신
    }
    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!dead)
        {
            playerAudioPlayer.PlayOneShot(hitClip); // 효과음 재생
        }

        base.OnDamage(damage, hitPoint, hitDirection);

        healthSlider.value = health;
    }
    public override void Die()
    {
        base.Die();

        healthSlider.gameObject.SetActive(false);

        playerAudioPlayer.PlayOneShot(deathClip);
        playerAnimator.SetTrigger("Die");
        playerMovement.enabled = false;
        playerShooter.enabled = false;

        Invoke("Respawn", 5f); // 부활
    }
    [PunRPC]
    public void Respawn() // 플레이어 부활
    {
        if (photonView.IsMine)
        {
            Vector3 randomPos = Random.insideUnitSphere * 5f; // 5유닛 반경 내 랜덤위치
            randomPos.y = 0f;
            transform.position = randomPos;
        }

        gameObject.SetActive(false); // OnEnable(초기화) 실행용
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other) // 아이템 습득 처리용
    {
        if (!dead)
        {
            IItem item = other.GetComponent<IItem>(); // 충돌한 오브젝트에서 IITEM 인터페이스 받아옴
            if (item != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    item.Use(gameObject);
                }                
                playerAudioPlayer.PlayOneShot(itemPickupClip);
            }
        }
    }
}

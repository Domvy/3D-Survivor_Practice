using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State // 총 상태
    {
        Ready,
        Empty,
        Reloading
    }

    public State state { get; private set; }
    public Transform fireTransform; // 사격 위치

    public ParticleSystem muzzleFlashEffect; // 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 표현
    
    private LineRenderer bulletLineRenderer; // 탄 궤적
    private AudioSource gunAudioPlyer; // 발사 소리
    
    public GunData gunData;

    private float fireDistance = 50f; // 사거리
    private float lastFireTime; // 마지막 발사 시간
    public int ammoRemain = 100; // 총 탄환
    public int magAmmo; // 현재 탄환

    private void Awake()
    {
        gunAudioPlyer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }
    private void OnEnable()
    {
        ammoRemain = gunData.startAmmoRemain;
        magAmmo = gunData.magCapacity;
        state = State.Ready;
        lastFireTime = 0f;
    }

    public void Fire()
    {
        if (state == State.Ready && Time.time >= lastFireTime + gunData.timeBetFire) // 준비상태 & 사격 딜레이 확인
        {
            lastFireTime = Time.time;
            Shot();
        }
    }
    private void Shot()
    {
        RaycastHit hit;
        Vector3 hitposition = Vector3.zero;

        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>(); // IDamageable 인터페이스를 가져옴
            if (target != null)
            {
                target.OnDamage(gunData.damage, hit.point, hit.normal); // 충돌 오브젝트에 데미지 처리
            }
            hitposition = hit.point; // 위치 저장
        }
        else
        {
            hitposition = fireTransform.position + fireTransform.forward * fireDistance; // 충돌 실패 시 최대사거리로 계산
        }

        StartCoroutine(ShotEffect(hitposition)); // 이펙트 출력
        magAmmo--;
        if (magAmmo <= 0)
        {
            state = State.Empty; // 재장전을 위해 비어있는 상태로 변경
        }
    }

    private IEnumerator ShotEffect(Vector3 hitPosition) // 사격 궤적 그리기
    {
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();
        gunAudioPlyer.PlayOneShot(gunData.shotClip);

        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;
        yield return new WaitForSeconds(0.03f);
        bulletLineRenderer.enabled = false;
    }
    public bool Reload()
    {
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= gunData.magCapacity) // 장전 불가능한 예외처리
        {
            return false;
        }

        StartCoroutine(ReloadRoutine());
        return true;
    }
    private IEnumerator ReloadRoutine()
    {
        state = State.Reloading;
        gunAudioPlyer.PlayOneShot(gunData.reloadClip);
        yield return new WaitForSeconds(gunData.reloadTime);
        int ammoToFill = gunData.magCapacity - magAmmo; // 채워야 할 갯수 계산
        if (ammoToFill >= ammoRemain)
        {
            ammoToFill = ammoRemain; // 남은 개수가 부족한 경우 반영
        }
        magAmmo += ammoToFill;
        ammoRemain -= ammoToFill;
        state = State.Ready;
    }
}

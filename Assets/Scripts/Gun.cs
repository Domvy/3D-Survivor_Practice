using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Splines;

public class Gun : MonoBehaviourPun, IPunObservable
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
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient); // 사격 처리는 호스트가 담당
        
        magAmmo--;
        if (magAmmo <= 0)
        {
            state = State.Empty; // 재장전을 위해 비어있는 상태로 변경
        }
    }
    [PunRPC]
    private void ShotProcessOnServer()
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;

        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>(); // IDamageable 인터페이스를 가져옴
            if (target != null)
            {
                target.OnDamage(gunData.damage, hit.point, hit.normal); // 충돌 오브젝트에 데미지 처리
            }
            hitPosition = hit.point; // 위치 저장
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance; // 충돌 실패 시 최대사거리로 계산
        }

        photonView.RPC("ShotEffectProcessOnClients", RpcTarget.All, hitPosition); // 모든 클라이언트가 이펙트 실행
    }
    [PunRPC]
    private void ShotEffectProcessOnClients(Vector3 hitPosition) // 이펙트 출력
    {
        StartCoroutine(ShotEffect(hitPosition)); 
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // 데이터 동기화
    {
        if (stream.IsWriting) // 로컬 플레이어 일 때 데이터 보냄
        {
            stream.SendNext(ammoRemain);
            stream.SendNext(magAmmo);
            stream.SendNext(state);
        }
        else // 데이터 받음
        {
            ammoRemain = (int)stream.ReceiveNext();
            magAmmo = (int)stream.ReceiveNext();
            state = (State)stream.ReceiveNext();
        }
    }
    [PunRPC]
    public void AddAmmo(int ammo) // 리모트 클라이언트 탄환 증가용 함수(아이템 사용 시)
    {
        ammoRemain += ammo;
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

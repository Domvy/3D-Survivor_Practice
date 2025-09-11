using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
    public AudioClip shotClip;
    public AudioClip reloadClip;

    public float damage = 25; // 공격력
    public int startAmmoRemain = 100; // 총 탄환
    public int magCapacity = 25; // 탄창 용량
    public float timeBetFire = 0.12f; // 발사 간격
    public float reloadTime = 1.8f; // 재장전 시간
}

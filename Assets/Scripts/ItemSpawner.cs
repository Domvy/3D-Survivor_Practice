using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;

public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items; // 아이템 리스트
    public Transform playerTransform; // 플레이어 위치
    
    public float maxDistance = 5f; // 플레이어 반경 최대 거리
    public float timeBetSpawnMax = 7f;
    public float timeBetSpawnMin = 2f;
    
    private float timeBetSpawn; // 생성 시간 간격
    private float lastSpawnTime; // 마지막 생성 시간

    private void Start()
    {
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0f;
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return; // 호스트만 아이템 생성

        if (Time.time >= lastSpawnTime + timeBetSpawn && playerTransform != null)
        {
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            Spawn();
        }
    }

    private void Spawn()
    {
        // 플레이어 근처 위치 랜덤으로 가져오기 => (0,0,0) 기준으로 변경
        Vector3 spawnPosition = GetRandomPointOnNavMesh(Vector3.zero, maxDistance);
        spawnPosition += Vector3.up * 0.5f; // 바닥에서 살짝 뜨게
        GameObject selectedItem = items[Random.Range(0, items.Length)]; // 생성할 아이템 선택
        GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPosition, Quaternion.identity); // 아이템 생성

        StartCoroutine(DestroyAfter(item, 5f)); // PhotonNetwork의 Destroy는 지연시간을 받지 못하므로 코루틴으로 변경
    }

    IEnumerator DestroyAfter(GameObject target, float delay) // 아이템 파괴 코루틴
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }

    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
    {
        // 플레이어 위치를 기준으로 구 안에서 랜덤한 1개의 위치 반환
        Vector3 randomPos = Random.insideUnitSphere * distance + center;
        NavMeshHit hit; // nav 위치 반환용 변수
        // randomPos와 가장 가까운 NavMesh 위의 점을 찾아서 hit으로 반환
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);

        return hit.position;
    }
}

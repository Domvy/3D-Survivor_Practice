using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    public Zombie zombiePrefab; // 좀비 프리팹
    public ZombieData[] zombieDatas; // 좀비 데이터
    public Transform[] spawnPoints; // 소환 위치

    private List<Zombie> zombies = new List<Zombie>(); // 생성된 좀비 관리용 리스트
    private int wave; // 현재 웨이브 카운트

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameOver)
        {
            return; // 게임 종료 시 스폰 종료
        }
        if (zombies.Count <= 0)
        {
            SpawnWave(); 
        }

        UpdateUI(); 
    }

    private void UpdateUI() // UI 최신화
    {
        UIManager.instance.UpdateWaveText(wave, zombies.Count);
    }
    private void SpawnWave() // 좀비 생성
    {
        int spawnCount = Mathf.RoundToInt(wave * 1.5f); // 좀비 갯수를 생성(wave * 1.5배 반올림 만큼)
        wave++;

        for (int i = 0; i < spawnCount; i++)
        {
            CreateZombie();
        }

    }
    private void CreateZombie() // 생성 및 위치 설정 + 처치 시 이벤트 설정
    {
        ZombieData zombieData = zombieDatas[Random.Range(0, zombieDatas.Length)]; // 생성할 좀비 종류 선택
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)]; // 위치 랜덤 선택
        Zombie zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation); // 좀비 생성

        zombie.Setup(zombieData); // 데이터 설정
        zombies.Add(zombie); // 리스트 추가
        zombie.onDeath += () => zombies.Remove(zombie); // 사망 시 이벤트에 '사망한 좀비 리스트 제거' 메서드 추가
        zombie.onDeath += () => Destroy(zombie.gameObject, 10f); // 사망 시 이벤트에 '10초 뒤 오브젝트 삭제'
        zombie.onDeath += () => GameManager.instance.AddScore(100); // 점수 상승
    }
}

using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections;

public class ZombieSpawner : MonoBehaviourPun, IPunObservable
{
    public Zombie zombiePrefab; // 좀비 프리팹
    public ZombieData[] zombieDatas; // 좀비 데이터
    public Transform[] spawnPoints; // 소환 위치

    private List<Zombie> zombies = new List<Zombie>(); // 생성된 좀비 관리용 리스트
    private int zombieCount = 0; // 남은 좀비 수
    private int wave; // 현재 웨이브 카운트

    private void Awake()
    {
        /* 미리 작성한 직렬화 클래스를 사용해 컬러 데이터를 전달 */
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor);
    }
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameManager.instance != null && GameManager.instance.isGameOver)
            {
                return; // 게임 종료 시 스폰 종료
            }
            if (zombies.Count <= 0)
            {
                SpawnWave();
            }
        }        

        UpdateUI(); 
    }

    private void UpdateUI() // UI 최신화
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UIManager.instance.UpdateWaveText(wave, zombies.Count); // 호스트는 직접 갱신
        }
        else
        {
            UIManager.instance.UpdateWaveText(wave, zombieCount); // 받은 데이터로 갱신
        }
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
        GameObject createdZombie = PhotonNetwork.Instantiate(zombiePrefab.gameObject.name, spawnPoint.position, spawnPoint.rotation); // 좀비 생성
        Zombie zombie = createdZombie.GetComponent<Zombie>();

        zombie.photonView.RPC("Setup", RpcTarget.All, zombieData.health, zombieData.damage, zombieData.speed, zombieData.skinColor);
        zombies.Add(zombie); // 리스트 추가
        zombie.onDeath += () => zombies.Remove(zombie); // 사망 시 이벤트에 '사망한 좀비 리스트 제거' 메서드 추가
        zombie.onDeath += () => StartCoroutine(DestroyAfter(zombie.gameObject, 10f)); // 사망 시 이벤트에 '10초 뒤 오브젝트 삭제'
        zombie.onDeath += () => GameManager.instance.AddScore(100); // 점수 상승
    }
    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(zombies.Count);
            stream.SendNext(wave);
        }
        else
        {
            zombieCount = (int)stream.ReceiveNext();
            wave = (int)stream.ReceiveNext();
        }
    }
}

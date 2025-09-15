using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private static GameManager m_instance;
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                Debug.LogError("GameManager 인스턴스가 존재하지 않습니다.");
            }
            return m_instance;
        }
    }

    public GameObject playerPrefab;
    public bool isGameOver { get; private set; }
    private int score = 0; // 게임 점수

    void Awake()
    {
        if (instance == null)
        {
            m_instance = this;
            //DontDestroyOnLoad(gameObject); 로드 시 초기화를 위해 비활성화
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        /*FindFirstObjectByType<PlayerHealth>().onDeath += EndGame;*/ // 플레이어 사망 시 게임오버 이벤트 실행 => 부활 기능으로 비활성화

        Vector3 randomSpawnPos = Random.insideUnitSphere * 5f;
        randomSpawnPos.y = 0f;

        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom(); // 방 떠남
        }
    }

    public void AddScore(int newScore) // 점수 UI 갱신
    {
        if (!isGameOver)
        {
            score += newScore;
            UIManager.instance.UpdateScoreText(score);
        }
    }
    public void EndGame() // 게임오버 UI 활성화
    {
        isGameOver = true;
        UIManager.instance.SetActiveGameOverUI(true);
    }
    public override void OnLeftRoom() // 룸 이탈 시 콜백 메서드
    {
        SceneManager.LoadScene("Lobby"); // 로비로 돌아감
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // 점수 동기화
    {
        if (stream.IsWriting) stream.SendNext(score);
        else
        {
            score = (int)stream.ReceiveNext();
            UIManager.instance.UpdateScoreText(score);
        }
    }
}

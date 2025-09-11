using UnityEngine;

public class GameManager : MonoBehaviour
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
        FindFirstObjectByType<PlayerHealth>().onDeath += EndGame; // 플레이어 사망 시 게임오버 이벤트 실행
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
}

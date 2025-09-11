using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                Debug.LogError("UIManager 인스턴스가 존재하지 않습니다.");
            }
            return m_instance;
        }
    }

    public Text ammoText;
    public Text scoreText;
    public Text waveText;
    public GameObject gameOverUI;

    void Awake()
    {
        if (instance == null)
        {
            m_instance = this;
        }
        else
        {            
            Destroy(gameObject);
        }
    }

    public void UpdateAmmoText(int magAmmo, int ammoRemain)
    {
        ammoText.text = magAmmo + "/" + ammoRemain;
    }
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }
    public void UpdateWaveText(int waves, int count)
    {
        waveText.text = "Wave : " + waves + "\nEnemyLeft : " + count;        
    }
    public void SetActiveGameOverUI(bool active)
    {
        gameOverUI.SetActive(active);
    }
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

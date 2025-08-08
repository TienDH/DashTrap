using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int score = 0;
    [SerializeField] private GameObject exitGame;
    [SerializeField] private GameObject pausePanel; // Panel tạm dừng
    public Image fillImage;  // Gán Image (Filled)
    public int maxScore = 48;
    [SerializeField] private GameObject winPanel;
    private bool isPaused = false; // Trạng thái tạm dừng
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ GameManager giữa các scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Kiểm tra các thành phần UI
        if (fillImage == null || exitGame == null || winPanel == null || pausePanel == null)
        {
            Debug.LogError("Một hoặc nhiều thành phần UI chưa được gán trong GameManager!");
        }
        exitGame.SetActive(false);
        winPanel.SetActive(false);
        pausePanel.SetActive(false); // Ẩn panel tạm dừng khi bắt đầu
    }

    public void AddScore(int value)
    {
        score += value;

        int clampedScore = Mathf.Clamp(score, 0, maxScore);
        if (fillImage != null)
        {
            fillImage.fillAmount = (float)clampedScore / maxScore;
        }

        if (clampedScore == maxScore)
        {
            if (exitGame != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.clip_4);
                exitGame.SetActive(true);
            }
        }
    }

    public void PlayerDie()
    {
        Time.timeScale = 1f; // Reset timescale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void WinGame()
    {
        Time.timeScale = 0f;
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Invoke(nameof(LoadNextLevel), 2f); // Tùy chọn delay
        }
        UnlockNewLevel();
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        LevelManager.Instance.LoadNextLevel();
    }

    // Hàm tạm dừng/tiếp tục game
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f; // Tạm dừng game
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Pause Panel chưa được gán trong GameManager");
            }
        }
        else
        {
            Time.timeScale = 1f; // Tiếp tục game
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }
    }

    // Hàm reset game
    public void ResetGame()
    {
        score = 0; // Reset điểm
        if (fillImage != null)
        {
            fillImage.fillAmount = 0f; // Reset thanh tiến độ
        }
        Time.timeScale = 1f; // Reset timescale
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // Ẩn panel tạm dừng
        }
        if (winPanel != null)
        {
            winPanel.SetActive(false); // Ẩn panel thắng
        }
        if (exitGame != null)
        {
            exitGame.SetActive(false); // Ẩn panel thoát
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Tải lại scene hiện tại
    }
    public void LoadSceneByName(string sceneName)
    {
        ResetGame();
        SceneManager.LoadScene(sceneName);
    }
    
    public void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("Unlocked", PlayerPrefs.GetInt("Unlocked", 1) + 1);
            PlayerPrefs.Save();
    
        }
    }
}
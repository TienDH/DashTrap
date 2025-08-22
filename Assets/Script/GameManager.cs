using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int score = 0;
    [SerializeField] private GameObject exitGame;
    [SerializeField] private GameObject pausePanel;
    public Image fillImage;
    public int maxScore = 48;
    [SerializeField] private GameObject winPanel;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        if (exitGame != null) exitGame.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Cập nhật thanh tiến độ score ngay khi bắt đầu game
        UpdateScoreBar();
    }

    private void UpdateScoreBar()
    {
        int clampedScore = Mathf.Clamp(score, 0, maxScore);
        if (fillImage != null && maxScore > 0)
        {
            fillImage.fillAmount = (float)clampedScore / maxScore;
        }
    }

    public void AddScore(int value)
    {
        score += value;
        int clampedScore = Mathf.Clamp(score, 0, maxScore);
        UpdateScoreBar();
        Debug.Log($"Current Score: {clampedScore}, Max Score: {maxScore}");

        if (clampedScore == maxScore)
        {
            if (exitGame != null)
            {
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.clip_4);
                exitGame.SetActive(true);
            }
        }
    }

    public void PlayerDie()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void WinGame()
    {
        Time.timeScale = 0f;
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Invoke(nameof(LoadNextLevel), 2f);
        }
        UnlockNewLevel();
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        LevelManager.Instance.LoadNextLevel();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
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
            Time.timeScale = 1f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }
    }

    public void ResetGame()
    {
        score = 0;
        UpdateScoreBar();
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (exitGame != null) exitGame.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
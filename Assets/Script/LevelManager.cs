using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Danh sách tên scene level (phải khớp với Build Settings)")]
    public string[] levelSceneNames;

    private int currentLevelIndex = 0;

    private const string ProgressKey = "UnlockedLevel"; // Key lưu PlayerPrefs

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cập nhật chỉ số level hiện tại dựa trên scene name
        for (int i = 0; i < levelSceneNames.Length; i++)
        {
            if (scene.name == levelSceneNames[i])
            {
                currentLevelIndex = i;
                break;
            }
        }
    }

    public void LoadLevel(int index)
    {
        if (index >= 0 && index < levelSceneNames.Length)
        {
            SceneManager.LoadScene(levelSceneNames[index]);
        }
        else
        {
            Debug.LogError("Level index không hợp lệ");
        }
    }

    public void ReloadCurrentLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadNextLevel()
    {
        int nextIndex = currentLevelIndex + 1;

        if (nextIndex < levelSceneNames.Length)
        {
            UnlockLevel(nextIndex); // Mở khóa màn tiếp theo
            LoadLevel(nextIndex);
        }
        else
        {
            Debug.Log("Đã hoàn thành tất cả các level!");
        }
    }

    public int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt(ProgressKey, 0);
    }

    public void UnlockLevel(int index)
    {
        int unlocked = GetUnlockedLevel();
        if (index > unlocked)
        {
            PlayerPrefs.SetInt(ProgressKey, index);
            PlayerPrefs.Save();
        }
    }

    public bool IsLevelUnlocked(int index)
    {
        return index <= GetUnlockedLevel();
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public string GetCurrentSceneName()
    {
        return levelSceneNames[currentLevelIndex];
    }
}

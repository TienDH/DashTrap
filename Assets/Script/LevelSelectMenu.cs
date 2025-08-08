using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    public Button[] buttons;

    private void Awake()
    {
        int unlockLevel = PlayerPrefs.GetInt("Unlocked", 1);
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        for (int i = 0; i < unlockLevel; i++)
        {
            buttons[i].interactable = true;
        }
    }
    public void OpenLvScene(int index)           
    {
        string levelName = "Lv" + index;
        SceneManager.LoadScene(levelName);
    }   
}

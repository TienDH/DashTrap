using UnityEngine;
using UnityEngine.UI;

public class AudioToggleUI : MonoBehaviour
{
    [Header("Music")]
    public Button musicButton;
    public Image musicIcon;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    [Header("SFX")]
    public Button sfxButton;
    public Image sfxIcon;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private void Start()
    {
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);
        UpdateIcons();
    }

    void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
        UpdateIcons();
    }

    void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
        UpdateIcons();
    }

    void UpdateIcons()
    {
        // music icon
        musicIcon.sprite = AudioManager.Instance.isMusicOn ? musicOnSprite : musicOffSprite;
        // sfx icon
        sfxIcon.sprite = AudioManager.Instance.isSFXOn ? sfxOnSprite : sfxOffSprite;
    }
}

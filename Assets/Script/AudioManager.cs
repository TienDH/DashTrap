using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip clip_1;
    public AudioClip clip_2;
    public AudioClip clip_3;
    public AudioClip clip_4;
    public AudioClip Music;
    public AudioClip WinClip;
    public AudioClip LossClip2;
    private float lastCoinSFXTime = 0f;
    private float coinSFXCooldown = 0.06f;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public bool isMusicOn = true;
    public bool isSFXOn = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.parent != null)
            {
                Debug.LogError("AudioManager must be attached to a root GameObject! Moving to root...");
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
            isSFXOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicSource == null || sfxSource == null)
        {
            Debug.LogError("Một hoặc nhiều AudioSource chưa được gán trong AudioManager!");
        }
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            musicSource.mute = !isMusicOn;
            PlayMusic(Music, true);
        }
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
            sfxSource.mute = !isSFXOn;
        }
    }

    private void Update()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            musicSource.mute = !isMusicOn;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
            sfxSource.mute = !isSFXOn;
        }
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        if (musicSource != null)
        {
            musicSource.mute = !isMusicOn;
        }
        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        if (sfxSource != null)
        {
            sfxSource.mute = !isSFXOn;
        }
        PlayerPrefs.SetInt("SFXOn", isSFXOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, bool isCoin = false)
    {
        if (!isSFXOn || sfxSource == null || clip == null) return;

        if (isCoin)
        {
            if (Time.time - lastCoinSFXTime < coinSFXCooldown)
                return;
            lastCoinSFXTime = Time.time;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}
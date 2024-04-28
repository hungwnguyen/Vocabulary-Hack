using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region variable
    private static SoundManager _instance;
    private ObjectPooler<AudioClip> _pool;
    public static SoundManager Instance { get => _instance; set { _instance = value; } }

    [SerializeField] public AudioClipData audioClip;
    [SerializeField] private AudioSource BGMusic;
    [SerializeField] private AudioSource FXSound;

    public float fx, bg;

    private Dictionary<string, AudioSource> FXLoop;
    private Dictionary<string, AudioSource> BgMusic;
    private AudioSource UI;
    #endregion

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        _pool = new ObjectPooler<AudioClip>();
        FXLoop = new Dictionary<string, AudioSource>();
        BgMusic = new Dictionary<string, AudioSource>();
        Instance._pool.OnSpawned += Instance.CustomSpawnHandler;
        fx = PlayerPrefs.GetFloat("fx", 1);
        bg = PlayerPrefs.GetFloat("bg", 1);
        CreateUISound();
    }

    #region Create GameObject Music

    private void CreateUISound()
    {
        if (Instance.UI == null)
        {
            GameObject m_currentAudioFXSound = new GameObject(Instance.audioClip.aud_touch.name);
            m_currentAudioFXSound.AddComponent<AudioSource>();
            Instance.UI = m_currentAudioFXSound.GetComponent<AudioSource>();
            CopyProperties(_instance.FXSound, Instance.UI, Instance.audioClip.aud_touch, Instance.fx);
            DontDestroyOnLoad(m_currentAudioFXSound);
        }
    }

    public static void CreatePlayFXSound(AudioClip aClip)
    {
        if (_instance.fx > 0)
            Instance._pool.SpawnFromPool(aClip, aClip.name);
    }

    public static bool CreatePlayFXSound(string aClipName)
    {
        bool check = Instance._pool.poolDictionary.ContainsKey(aClipName);
        if (_instance.fx > 0 && check)
        {
            Instance._pool.SpawnFromPool(aClipName);
        }
        return check;

    }

    public static void CreatePlayFXSound()
    {
        if (_instance.fx > 0)
        {
            Instance.UI.Play();
        }
    }

    public static void CreatePlayFXLoop(AudioClip aClip)
    {
        if (!Instance.FXLoop.ContainsKey(aClip.name))
        {
            GameObject obj = new GameObject(aClip.name);
            obj.transform.position = Vector3.zero;
            obj.AddComponent<AudioSource>();
            Instance.FXLoop.Add(aClip.name, obj.GetComponent<AudioSource>());
            CopyProperties(Instance.BGMusic, Instance.FXLoop[aClip.name], aClip, Instance.fx);
            DontDestroyOnLoad(obj);
        }
        Instance.PlayFXLoop(aClip);
    }

    public static void CreatePlayBgMusic(AudioClip aClip)
    {
        if (!Instance.BgMusic.ContainsKey(aClip.name))
        {
            GameObject obj = new GameObject(aClip.name);
            obj.transform.position = Vector3.zero;
            obj.AddComponent<AudioSource>();
            Instance.BgMusic.Add(aClip.name, obj.GetComponent<AudioSource>());
            CopyProperties(Instance.BGMusic, Instance.BgMusic[aClip.name], aClip, Instance.bg);
            DontDestroyOnLoad(obj);
        }
        Instance.PlayBgMusic(aClip);
    }

    private void PlayBgMusic(AudioClip aClip)
    {
        if (bg > 0)
        {
            Instance.BgMusic[aClip.name].gameObject.SetActive(true);
            Instance.BgMusic[aClip.name].volume = bg;
            if (!Instance.BgMusic[aClip.name].isPlaying)
                Instance.BgMusic[aClip.name].Play();
            if (Time.timeScale == 0)
            {
                Instance.BgMusic[aClip.name].Pause();
            }
        }
    }

    private void PlayFXLoop(AudioClip aClip)
    {
        if (fx > 0)
        {
            Instance.FXLoop[aClip.name].gameObject.SetActive(true);
            Instance.FXLoop[aClip.name].volume = fx;
            if (!Instance.FXLoop[aClip.name].isPlaying)
                Instance.FXLoop[aClip.name].Play();

            if (Time.timeScale == 0)
            {
                Instance.FXLoop[aClip.name].Pause();
            }
        }
    }

    public void StopFXLoop(AudioClip aClip)
    {
        if (this.FXLoop != null)
        {
            if (this.FXLoop.ContainsKey(aClip.name))
            {
                this.FXLoop[aClip.name].Stop();
                this.FXLoop[aClip.name].gameObject.SetActive(false);
            }
        }
    }

    public void StopBgMusic(AudioClip aClip)
    {
        if (this.BgMusic != null)
        {
            if (this.BgMusic.ContainsKey(aClip.name))
            {
                this.BgMusic[aClip.name].Stop();
                this.BgMusic[aClip.name].gameObject.SetActive(false);
            }
        }
    }

    private GameObject CustomSpawnHandler(AudioClip aClip)
    {
        GameObject m_currentAudioFXSound = new GameObject(aClip.name);
        m_currentAudioFXSound.AddComponent<AudioSource>();
        m_currentAudioFXSound.AddComponent<Sound>();
        AudioSource component = m_currentAudioFXSound.GetComponent<AudioSource>();
        CopyProperties(_instance.FXSound, component, aClip, Instance.fx);
        DontDestroyOnLoad(m_currentAudioFXSound);
        return m_currentAudioFXSound;
    }

    static void CopyProperties(AudioSource sourceToCopyFrom, AudioSource newSource, AudioClip aClip, float vol)
    {
        // Copy all the properties from the sourceToCopyFrom AudioSource to the new AudioSource
        newSource.clip = aClip;
        newSource.volume = vol;
        newSource.pitch = sourceToCopyFrom.pitch;
        newSource.panStereo = sourceToCopyFrom.panStereo;
        newSource.spatialBlend = sourceToCopyFrom.spatialBlend;
        newSource.reverbZoneMix = sourceToCopyFrom.reverbZoneMix;
        newSource.dopplerLevel = sourceToCopyFrom.dopplerLevel;
        newSource.spread = sourceToCopyFrom.spread;
        newSource.minDistance = sourceToCopyFrom.minDistance;
        newSource.maxDistance = sourceToCopyFrom.maxDistance;
        newSource.playOnAwake = sourceToCopyFrom.playOnAwake;
        newSource.loop = sourceToCopyFrom.loop;
        newSource.rolloffMode = sourceToCopyFrom.rolloffMode;
        newSource.outputAudioMixerGroup = sourceToCopyFrom.outputAudioMixerGroup;

        // Copy the custom rolloff curve if applicable
        if (newSource.rolloffMode == AudioRolloffMode.Custom)
        {
            AnimationCurve volumeRolloffCurve = sourceToCopyFrom.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            newSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeRolloffCurve);
        }
    }
    #endregion

    #region Sound Setting

    public static void PauseAllMusic()
    {
        foreach (AudioSource audioSource in _instance.BgMusic.Values)
        {
            if (audioSource.gameObject.activeSelf)
            {
                audioSource.Pause();
            }
        }
        foreach (AudioSource audioSource in _instance.FXLoop.Values)
        {
            if (audioSource.gameObject.activeSelf)
            {
                audioSource.Pause();
            }
        }
        foreach (GameObject go in _instance._pool.poolDictionary.Values)
        {
            if (go.activeSelf)
            {
                go.GetComponent<AudioSource>().Pause();
            }
        }
    }

    /// <summary>
    /// Return true if can play music
    /// </summary>
    /// <returns></returns>
    public static bool ContinuePlayAllMusic()
    {
        bool check = false;
        if (Instance.bg > 0)
        {
            foreach (AudioSource audioSource in _instance.BgMusic.Values)
            {
                if (audioSource.gameObject.activeSelf)
                {
                    check = true;
                    if (!audioSource.isPlaying)
                        audioSource.Play();
                }
            }
        }
        else
        {
            foreach (AudioSource audioSource in _instance.BgMusic.Values)
            {
                if (audioSource.gameObject.activeSelf)
                {
                    audioSource.Stop();
                }
            }
        }
        if (Instance.fx > 0)
        {
            foreach (AudioSource audioSource in _instance.FXLoop.Values)
            {
                if (audioSource.gameObject.activeSelf)
                {
                    check = true;
                    if (!audioSource.isPlaying)
                        audioSource.Play();
                }
            }
            foreach (GameObject go in _instance._pool.poolDictionary.Values)
            {
                if (go.activeSelf)
                {
                    go.GetComponent<AudioSource>().Play();
                }
            }
        }
        else
        {
            foreach (AudioSource audioSource in _instance.FXLoop.Values)
            {
                if (audioSource.gameObject.activeSelf)
                {
                    audioSource.Stop();
                }
            }
            foreach (GameObject go in _instance._pool.poolDictionary.Values)
            {
                if (go.activeSelf)
                {
                    go.GetComponent<AudioSource>().Stop();
                }
            }
        }
        return check;
    }

    public static void DisableAllMusic()
    {
        foreach (AudioSource audioSource in _instance.BgMusic.Values)
        {
            if (audioSource.gameObject.activeSelf)
            {
                audioSource.Stop();
                audioSource.gameObject.SetActive(false);
            }
        }
        foreach (AudioSource audioSource in _instance.FXLoop.Values)
        {
            if (audioSource.gameObject.activeSelf)
            {
                audioSource.Stop();
                audioSource.gameObject.SetActive(false);
            }
        }
        foreach (GameObject go in _instance._pool.poolDictionary.Values)
        {
            if (go.activeSelf)
            {
                go.GetComponent<AudioSource>().Stop();
                go.gameObject.SetActive(false);
            }
        }
    }

    public static void DisableBGMusic()
    {
        foreach (AudioSource audioSource in _instance.BgMusic.Values)
        {
            if (audioSource.gameObject.activeSelf)
            {
                audioSource.Stop();
                audioSource.gameObject.SetActive(false);
            }
        }
    }

    public static void DisableFXSound()
    {
        _instance.fx = 0;
    }

    public static void EnableFXSound()
    {
        _instance.fx = 1;
    }

    public static void ChangeVolumeBGMusic(float value)
    {
        PlayerPrefs.SetFloat("bg", value);
        _instance.bg = value;
        foreach (AudioSource audioSource in _instance.BgMusic.Values)
        {
            audioSource.volume = value;
        }
    }

    public static void ChangeVolumeFXSound(float value)
    {
        PlayerPrefs.SetFloat("fx", value);
        _instance.fx = value;
        _instance.UI.volume = value;
        foreach (AudioSource audioSource in _instance.FXLoop.Values)
        {
            audioSource.volume = value;
        }
        foreach (GameObject go in _instance._pool.poolDictionary.Values)
        {
            go.GetComponent<AudioSource>().volume = value;
        }
    }
    #endregion


}

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AudioClass
{
    public string name = "Audio";
    public AudioClip clip;
    [Range(0, 1.0f)]
    public float volume = 0.5f;
    [HideInInspector]
    public AudioSource source;
    public bool loopAudio = false;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClass[] audioArr;


    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        foreach (AudioClass audio in audioArr)
        {
            audio.source = gameObject.AddComponent<AudioSource>();
            audio.source.clip = audio.clip;
            audio.source.volume = audio.volume;
            audio.source.loop = audio.loopAudio;
        }
        
    }

    public void PlayAudiobyName(string name)
    {
        AudioClass audio = Array.Find(audioArr, audio => audio.name == name);
        if (audio != null && !audio.source.isPlaying)
        {
            audio.source.Play();
        }
    }

    public void StopMusicbyName(string name)
    {
        AudioClass audio = Array.Find(audioArr, audio => audio.name == name);
        if (audio != null && audio.source.isPlaying)
        {
            audio.source.Stop();
        }
    }
    public void ToggleMusicbyName(string name)
    {
        AudioClass audio = Array.Find(audioArr, audio => audio.name == name);
        if (audio.source.isPlaying)
        {
            audio.source.Pause();
        }
        else
        {
            audio.source.Play();

        }
    }

    private void OnLevelWasLoaded()
    {
       
    }
}

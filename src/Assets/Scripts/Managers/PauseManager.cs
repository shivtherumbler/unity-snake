using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    //public AudioManager audioManager;

    public bool isPaused;
    public bool isMute;
    public GameObject pausemenu;
    public Snake.SoundManager soundManager;

    public void PauseGame()
    {
        isPaused = !isPaused;
    }
    // Start is called before the first frame update
    void Start()
    {
        //audioManager = AudioManager.instance;
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName: sceneName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            
        }

        if(isPaused)
        {
            Time.timeScale = 0;
            pausemenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pausemenu.SetActive(false);
        }

        if(isMute)
        {
            soundManager.m_sfxSource.mute = true;
        }
        else
        {
            soundManager.m_sfxSource.mute = false;
        }
    }

    private void OnLevelWasLoaded()
    {
        soundManager = FindObjectOfType<Snake.SoundManager>();
    }

    public void Mute()
    {

        isMute = !isMute;
    }
}

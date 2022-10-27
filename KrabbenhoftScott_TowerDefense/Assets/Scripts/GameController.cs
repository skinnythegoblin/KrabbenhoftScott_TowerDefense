using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static event Action OnPause;
    public static event Action OnUnpause;
    
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Unpause();
    }

    public static void Quit()
    {
        Application.Quit();
    }

    public static void Pause()
    {
        OnPause?.Invoke();
        Time.timeScale = 0;
    }

    public static void Unpause()
    {
        OnUnpause?.Invoke();
        Time.timeScale = 1;
    }
}

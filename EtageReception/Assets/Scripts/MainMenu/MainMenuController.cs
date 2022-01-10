using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    private void Start()
    {
    }
    public void Play()
    {
        GameStateManager.Instance.LaunchTransition(EGameState.LEVEL1);
    }

    public void HowToPlay()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

}

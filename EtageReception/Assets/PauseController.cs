using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Luminosity.IO;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{


    [SerializeField] private GameObject _menuMain = null;
    [SerializeField] private GameObject _howToPlayUI = null;
    [SerializeField] private Button _back = null;
    [SerializeField] private Button _howToPlayButton = null;
    private bool _pauseOpen = false;

    private void Update()
    {
        if(InputManager.GetButton("UI_Cancel") && _pauseOpen == false)
        {
            OpenPause();
        }
        else if(InputManager.GetButton("UI_Cancel") && _pauseOpen == true)
        {
            ClosePause();
        }
    }

    public void OpenPause()
    {
        _menuMain.SetActive(true);
        _howToPlayButton.Select();
        _pauseOpen = true;
    }

    public void ClosePause()
    {
        _menuMain.SetActive(false);
        _pauseOpen = false;

    }

    public void OpenHowToPlay()
    {
        _menuMain.SetActive(false);
        _howToPlayUI.SetActive(true);
        AudioManager.Instance.Start2DSound("S_UI");
        _back.Select();
    }

    public void CloseHowToPlay()
    {
        _menuMain.SetActive(true);
        _howToPlayUI.SetActive(false);
        AudioManager.Instance.Start2DSound("S_UI");
        _howToPlayButton.Select();

    }


    public void Quit()
    {
        AudioManager.Instance.Start2DSound("S_UI");
        StartCoroutine(Delay(2f));

    }

    private IEnumerator Delay(float timer)
    {
        yield return new WaitForSeconds(timer);
        GameStateManager.Instance.LaunchTransition(EGameState.MAINMENU);

    }

 
}



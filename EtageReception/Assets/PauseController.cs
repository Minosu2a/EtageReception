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
        if(InputManager.GetButtonDown("UI_Cancel") && _pauseOpen == false)
        {
            OpenPause();
        }
        else if(InputManager.GetButtonDown("UI_Cancel") && _pauseOpen == true)
        {
            ClosePause();
        }
    }

    public void OpenPause()
    {
        Time.timeScale = 0;
        _menuMain.SetActive(true);
        _pauseOpen = true;
        _howToPlayButton.Select();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePause()
    {
        Time.timeScale = 1;
        _menuMain.SetActive(false);
        _pauseOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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



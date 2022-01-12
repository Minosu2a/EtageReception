using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator _fade = null;
    [SerializeField] private GameObject _menuMain = null;
    [SerializeField] private GameObject _howToPlayUI = null;
    [SerializeField] private Button _back = null;
    [SerializeField] private Button _howToPlayButton = null;


    private void Start()
    {
    }
    public void Play()
    {
        AudioManager.Instance.Start2DSound("S_Button");
        _fade.SetTrigger("Transition");
        StartCoroutine(Loading(2.5f));
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

    public void Trump()
    {
        AudioManager.Instance.StartTrumpSound();
    }

    public void Quit()
    {
        AudioManager.Instance.Start2DSound("S_UI");
        StartCoroutine(Delay(2f));
        Application.Quit();
    }

    private IEnumerator Delay(float timer)
    {
        yield return new WaitForSeconds(timer);
    }

    private IEnumerator Loading(float timer)
    {
        yield return new WaitForSeconds(timer);
        GameStateManager.Instance.LaunchTransition(EGameState.GAME);

    }
}

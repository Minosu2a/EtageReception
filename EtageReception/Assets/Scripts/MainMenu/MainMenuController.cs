using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator _fade = null;
    private void Start()
    {
    }
    public void Play()
    {
        AudioManager.Instance.Start2DSound("S_Button");
        GameStateManager.Instance.LaunchTransition(EGameState.LEVEL1);
    }

    public void HowToPlay()
    {

    }

    public void Quit()
    {
        AudioManager.Instance.Start2DSound("S_Button");
        StartCoroutine(Delay(2f));
        Application.Quit();
    }

    private IEnumerator Delay(float timer)
    {
        yield return new WaitForSeconds(timer);
    }

  /*  private IEnumerator Delay(float timer)
    {
        yield return new WaitForSeconds(timer);
    }*/
}

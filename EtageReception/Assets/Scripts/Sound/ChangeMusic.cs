using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusic : MonoBehaviour
{

    int _currentMusic = 0;


    public void MusicChange()
    {
        switch(_currentMusic)
        {
            case 0:
                AudioManager.Instance.PlayMusic("M_Music1");
                break;
            case 1:
                AudioManager.Instance.PlayMusic("M_Music2");

                break;
            case 2:
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.Start2DSound("M_Music3");
                break;
            case 3:
                AudioManager.Instance.PlayMusic("M_Game");

                break;
            case 4:
                AudioManager.Instance.PlayMusic("M_Game2");

                break;
        }

        _currentMusic++;

        if (_currentMusic > 4)
        {
            _currentMusic = 0;
        }

    }

}

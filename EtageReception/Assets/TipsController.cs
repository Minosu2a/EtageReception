using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class TipsController : MonoBehaviour
{

    [SerializeField] private GameObject _mouseMoveTips = null;
    [SerializeField] private GameObject _padMoveTips = null;

    private bool _alreadyTriggered = false;

    void Start()
    {
        
    }

    void Update()
    {

        if (_alreadyTriggered == false)
        {
            if (InputManager.GetButton("AllController") || InputManager.GetAxis("AllController") != 0)
            {
                _alreadyTriggered = true;
                StartCoroutine(Tips(true));
            }
            if (InputManager.GetButton("AllMouse"))
            {
                _alreadyTriggered = true;
                StartCoroutine(Tips(false));

            }
        }

   
    }


    IEnumerator Tips(bool isPad)
    {
        if(isPad == true)
        {
            _padMoveTips.SetActive(true);
            yield return new WaitForSeconds(8f);
            _padMoveTips.SetActive(false);
        }
        else
        {
            _mouseMoveTips.SetActive(true);
            yield return new WaitForSeconds(8f);
            _mouseMoveTips.SetActive(false);
        }
     
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;

public class TipsController : MonoBehaviour
{

    [SerializeField] private GameObject _mouseMoveTips = null;
    [SerializeField] private GameObject _padMoveTips = null;

    private bool _tips;

    private bool _alreadyTriggered = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (InputManager.GetButton("AllController") || InputManager.GetAxis("AllController") != 0)
        {
            _tips = true;
            if (_alreadyTriggered == false)
            {
                _alreadyTriggered = true;
                StartCoroutine(Tips());
            }
        }
        if (InputManager.GetButton("AllMouse"))
        {
            _tips = false;
            if (_alreadyTriggered == false)
            {
                _alreadyTriggered = true;
                StartCoroutine(Tips());
            }
        }
        
    }


    IEnumerator Tips()
    {
        float duration = 8f;
        while(duration > 0)
        {
            if (_tips == true)
            {
                _padMoveTips.SetActive(true);
                _mouseMoveTips.SetActive(false);
            }
            else
            {
                _mouseMoveTips.SetActive(true);
                _padMoveTips.SetActive(false);
            }
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _padMoveTips.SetActive(false);
        _mouseMoveTips.SetActive(false);
    }
}

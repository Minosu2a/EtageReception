using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    #region Fields
    private float _tick = 1;
    private float _timeStamp = 0;
    private bool _isTimerLaunched = false;
    #endregion Fields

    #region Properties
    public bool IsTimerLaunched
    {
        get
        {
            return _isTimerLaunched;
        }
        set
        {
            _isTimerLaunched = value;
            if (_isTimerLaunched)
            {
                GameLoopManager.Instance.OnGameLoop += Loop;
                _timeStamp = 0;
            }
            else
            {
                GameLoopManager.Instance.OnGameLoop -= Loop;
            }
        }
    }
    #endregion Properties

    #region Events
    private event Action _onTick = null;
    public event Action OnTick
    {
        add
        {
            _onTick -= value;
            _onTick += value;
        }
        remove
        {
            _onTick -= value;
        }
    }
    #endregion Events

    #region Methods
    public void Loop()
    {
        _timeStamp += Time.deltaTime;
        if (_timeStamp >= _tick)
        {
            if (_onTick != null)
            {
                _onTick();
            }
            _timeStamp = 0;
        }
    }

    public void StartTimer(float tick)
    {
        _tick = tick;
        IsTimerLaunched = true;
    }

    public void StopTimer()
    {
        IsTimerLaunched = false;
    }

    public void ChangerTickValue(float tick)
    {
        _tick = tick;
    }
    #endregion Methods
}

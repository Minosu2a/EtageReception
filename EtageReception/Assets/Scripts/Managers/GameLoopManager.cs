using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopManager : Singleton<GameLoopManager>
{
    #region Events
    private event Action _onEarlyGameLoop = null;
    public event Action OnEarlyGameLoop
    {
        add
        {
            _onEarlyGameLoop -= value;
            _onEarlyGameLoop += value;
        }
        remove
        {
            _onEarlyGameLoop -= value;
        }
    }

    private event Action _onGameLoop = null;
    public event Action OnGameLoop
    {
        add
        {
            _onGameLoop -= value;
            _onGameLoop += value;
        }
        remove
        {
            _onGameLoop -= value;
        }
    }

    private event Action _onLateGameLoop = null;
    public event Action OnLateGameLoop
    {
        add
        {
            _onLateGameLoop -= value;
            _onLateGameLoop += value;
        }
        remove
        {
            _onLateGameLoop -= value;
        }
    }
    #endregion Events

    #region Methods
    public void Initialize()
    {

    }

    protected override void Update()
    {
        if (_onEarlyGameLoop != null)
        {
            _onEarlyGameLoop();
        }
        if (_onGameLoop != null)
        {
            _onGameLoop();
        }
        if(_onLateGameLoop != null)
        {
            _onLateGameLoop();
        }
    }
    #endregion Methods
}

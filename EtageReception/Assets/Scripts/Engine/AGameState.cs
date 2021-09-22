using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGameState
{
    #region Fields
    protected EGameState _state = EGameState.NONE;
    #endregion Fields

    #region Properties
    #endregion Properties

    #region Methods
    public void Initialize(EGameState state)
    {
        _state = state;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    #endregion Methods
}

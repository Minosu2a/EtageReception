using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : AGameState
{
    #region Fields
    #endregion Fields

    #region Properties
    #endregion Properties

    #region Methods
    public override void EnterState()
    {
        AudioManager.Instance.PlayMusic("M_Menu");
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }
    #endregion Methods
}

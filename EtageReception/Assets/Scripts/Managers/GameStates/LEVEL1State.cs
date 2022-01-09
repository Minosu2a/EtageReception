using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEVEL1State : AGameState
{
    #region Fields
    #endregion Fields

    #region Properties
    #endregion Properties

    #region Methods
    public override void EnterState()
    {
        AudioManager.Instance.PlayMusicWithFadeIn("M_Game", 1.5f);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }
    #endregion Methods
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    #region Fields
    private CharacterControllerE _characterController = null;
    #endregion Fields


    #region Property
    public CharacterControllerE CharacterController
    {

        get
        {
            return _characterController;
        }
        set
        {
            if (_characterController == null)
            {
                _characterController = value;
            }
        }

    }
    #endregion Property


    #region Methods
   public void Initialize()
   {

   }

    #endregion Methods

}

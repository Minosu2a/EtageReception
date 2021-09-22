using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    #region Fields
    private Vector3 _moveDir = Vector3.zero;
    private Vector3 _rot = Vector3.zero;
    #endregion Fields

    #region Properties
    public Vector3 MoveDir => _moveDir;
    public Vector3 Rot => _rot;
    #endregion Properties

    #region Events

    #endregion Events

    #region Methods
    public void Initialize()
    {
        
    }

    protected override void Update()
    {
        _moveDir.x = Input.GetAxis("Horizontal");
        _moveDir.z = Input.GetAxis("Vertical");

        _rot.y = Input.GetAxis("Mouse Y");
        _rot.x = Input.GetAxis("Mouse X");
    }
    #endregion Methods
}

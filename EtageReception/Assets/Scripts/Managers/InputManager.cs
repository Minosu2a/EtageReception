using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerE : Singleton<InputManagerE>
{
    #region Fields
    private Vector3 _moveDir = Vector3.zero;
    private Vector3 _rotDir = Vector3.zero;

    [Header("Mouse")]
    [SerializeField] private bool _mouseActivated = true;

    [Header("Action")]
    [SerializeField] private KeyCode _grabInput = KeyCode.Mouse0;
    [SerializeField] private KeyCode _dashInput = KeyCode.LeftAlt;
    [SerializeField] private KeyCode _jumpInput = KeyCode.Space;

    #endregion Fields

    #region Properties
    public Vector3 MoveDir => _moveDir;
    public Vector3 RotDir => _rotDir;

    public bool MouseActivated
    {
        get
        {
            return _mouseActivated;
        }
        set
        {
            _mouseActivated = value;
        }
    }
    #endregion Properties

    #region Events

    #endregion Events

    #region Methods
    public void Initialize()
    {
        
    }

    protected override void Update()
    {
        #region Movement & Rotation
        _moveDir.x = Input.GetAxis("Horizontal");
        _moveDir.z = Input.GetAxis("Vertical");

        #endregion Movement & Rotation

        if(CharacterManager.Instance.CharacterController != null)
        {
            if (Input.GetKeyDown(_grabInput))
            {
                CharacterManager.Instance.CharacterController.Grab();
            }

            if (Input.GetKey(_jumpInput))
            {

            }

            if (Input.GetKey(_dashInput))
            {

            }
        }

     

    }
    #endregion Methods
}

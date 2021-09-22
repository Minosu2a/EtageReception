using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    #region Fields
    [Header("Movement")]
    [SerializeField] private Rigidbody _rb = null;
    [SerializeField] private float _walkSpeed = 250f;
    [SerializeField] private float _sprintSpeed = 500f;
    private bool _isMoving = false;

    [Header("Look")]
    private Vector3 _posMouse = Vector3.zero;
    private Vector3 _mouseRotdir = Vector3.zero;


     [SerializeField] private GrabDetection _grabDetection = null;
     [SerializeField] private GameObject _centerGrabPoint = null;


    #endregion Fields


    #region Properties

    public Rigidbody Rb => _rb;

    public Vector3 PosMouse => _posMouse;

    
    #endregion Properties


    #region Methods

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        CharacterManager.Instance.CharacterController = this;    
    }

    private void Update()
    {
      


        Walk();

        if(_rb.velocity != Vector3.zero)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false; 
        }
    }


    private void Look()
    {
        transform.forward = _rb.velocity;
    }

    public void Walk()
    {
        _rb.velocity = InputManager.Instance.MoveDir * _walkSpeed;
    }

    public void Sprint()
    {
        _rb.velocity = InputManager.Instance.MoveDir * _sprintSpeed;
    }

    public void Grab()
    {
        if(_grabDetection.GrabRangeObject.Count >= 0)
        {
            for (int i =0; i < _grabDetection.GrabRangeObject.Count; i++ )
            {

                float smallerDistance = 0;
                int integerOfCloserObject = 0;

                float dist = Vector3.Distance(_grabDetection.GrabRangeObject[i].transform.position, _centerGrabPoint.transform.position);

                if(i == 0)
                {
                    smallerDistance = dist;
                    integerOfCloserObject = i;
                }
                else if(dist < smallerDistance)
                {
                    smallerDistance = dist;
                    integerOfCloserObject = i;
                }

                FixedJoint fj = _grabDetection.GrabRangeObject[i].AddComponent<FixedJoint>();
                fj.connectedBody = _rb;

                // _centerGrabPoint
                // _grabDetection.GrabRangeObject[i].transform.position;
            }

        }

    }

    public void Jump()
    {

    }



    #endregion Methods



}

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

    [Header("Grab")]
    private bool _isGrabbing = false;

    [SerializeField] private GrabDetection _grabDetection = null;
    [SerializeField] private GameObject _centerGrabPoint = null;

    private GameObject _objectGrabbed = null;

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

       // Look();

    }


    private void Look()
    {
        if(_rb.velocity.magnitude >= 0.4f)
        {
            transform.forward = _rb.velocity.normalized;
        }
    }

    public void Walk()
    {
        _rb.velocity = InputManager.Instance.MoveDir * _walkSpeed * Time.deltaTime;
    }

    public void Sprint()
    {
        _rb.velocity = InputManager.Instance.MoveDir * _sprintSpeed;
    }

    public GameObject DetectObject()
    {
        if (_grabDetection.GrabRangeObject.Count > 0)
        {

            float smallerDistance = 0;
            int integerOfCloserObject = 0;

            for (int i = 0; i < _grabDetection.GrabRangeObject.Count; i++)
            {

                float dist = Vector3.Distance(_grabDetection.GrabRangeObject[i].transform.position, _centerGrabPoint.transform.position);

                if (i == 0)
                {
                    smallerDistance = dist;
                    integerOfCloserObject = i;
                }
                else if (dist < smallerDistance)
                {
                    smallerDistance = dist;
                    integerOfCloserObject = i;
                }
            }

            GameObject closestObject = _grabDetection.GrabRangeObject[integerOfCloserObject];
            return closestObject;

        }

        return null;
    }

    public void Grab()
    {
        if (_isGrabbing == false)
        {
            if((_objectGrabbed = DetectObject()) != null)
            {
                FixedJoint fj = _objectGrabbed.AddComponent<FixedJoint>();
                fj.connectedBody = _rb;
                fj.breakForce = 9001;
                _isGrabbing = true;
            }
        }
        else if (_isGrabbing == true)
        {
            Destroy(_objectGrabbed.GetComponent<FixedJoint>());
            _isGrabbing = false;
        }

    }

    public void Jump()
    {

    }



    #endregion Methods



}

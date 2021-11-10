using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerE : MonoBehaviour
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

    [SerializeField] private Rigidbody _trompeRigidbody = null;

    #endregion Fields


    #region Properties

    public Rigidbody Rb => _rb;

    public Vector3 PosMouse => _posMouse;

    
    #endregion Properties


    #region Methods

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //CharacterManager.Instance.CharacterController = this;    
    }

    private void Update()
    {
      
       

        Walk();

        Look();

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
        _rb.velocity = InputManagerE.Instance.MoveDir * _walkSpeed * Time.deltaTime;
    }

    public void Sprint()
    {
        _rb.velocity = InputManagerE.Instance.MoveDir * _sprintSpeed;
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
                if(_objectGrabbed.GetComponent<ConfigurableJoint>() != null)
                {
                    ConfigurableJoint fj = _objectGrabbed.GetComponent<ConfigurableJoint>();

                    fj.connectedBody = _trompeRigidbody;

                    fj.xMotion = ConfigurableJointMotion.Locked;
                    fj.yMotion = ConfigurableJointMotion.Locked;
                    fj.zMotion = ConfigurableJointMotion.Locked;

                    float xrot = _objectGrabbed.transform.rotation.x;
                    float yrot = _objectGrabbed.transform.rotation.y;
                    float zrot = _objectGrabbed.transform.rotation.z;

                   Vector3 dirAnchor = _centerGrabPoint.transform.position -_objectGrabbed.transform.position;
                   Debug.Log(dirAnchor);
                    fj.anchor = dirAnchor;
                   // t.normalized

                    _isGrabbing = true;
                }
            }
        }
        else if (_isGrabbing == true)
        {
            ConfigurableJoint fj = _objectGrabbed.GetComponent<ConfigurableJoint>();
            fj.connectedBody = null;
            fj.xMotion = ConfigurableJointMotion.Free;
            fj.yMotion = ConfigurableJointMotion.Free;
            fj.zMotion = ConfigurableJointMotion.Free;
            _isGrabbing = false;
        }

    }

    public void Jump()
    {

    }



    #endregion Methods



}

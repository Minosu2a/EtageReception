using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ClemCAddons.Utilities;
using System;
using ClemCAddons;
using Luminosity.IO;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _jumpStrength = 1;
    [SerializeField] private AnimationInfo _animations;
    private Animator _animator;
    private List<GameObject> joints = new List<GameObject>();
    private List<KeyValuePair<int, KeyValuePair<float, AnimationInfo.InfoAnimation>>> _currentAnimations = new List<KeyValuePair<int, KeyValuePair<float, AnimationInfo.InfoAnimation>>>();
    private bool isRunningAnimation = true;

    [Header("Grab")]
    private bool _isGrabbing = false;

    [SerializeField] private GrabDetection _grabDetection = null;
    [SerializeField] private GameObject _centerGrabPoint = null;

    private GameObject _objectGrabbed = null;

    [SerializeField] private Rigidbody _trompeRigidbody = null;


    void Start()
    {
        CharacterManager.Instance.CharacterController = this;
        var r = FindObjectsOfType<CopyMotion>();
        joints = r.Select(t => t.gameObject).ToList();
        _animator = transform.FindDeep("AnimationBody").GetComponent<Animator>();
    }

    void Update()
    {
        bool input = false;
        if (InputManager.GetAxis("Forward") > 0)
        {
            input = true;
            AddForceOnJoint("Root",transform.right * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LeftLeg", transform.right * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("RightLeg", transform.right * _speed * Time.deltaTime * 1000);
        } else if (InputManager.GetAxis("Forward") < 0)
        {
            input = true;
            AddForceOnJoint("Root", transform.right * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LeftLeg", transform.right * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("RightLeg", transform.right * _speed * Time.deltaTime * -1000);
        }
        if (InputManager.GetAxis("Strafe") > 0)
        {
            input = true;
            AddForceOnJoint("Root", transform.forward * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LeftLeg", transform.forward * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("RightLeg", transform.forward * _speed * Time.deltaTime * 1000);
        } else if (InputManager.GetAxis("Strafe") < 0)
        {
            input = true;
            AddForceOnJoint("Root", transform.forward * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LeftLeg", transform.forward * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("RightLeg", transform.forward * _speed * Time.deltaTime * -1000);
        }
        if (InputManager.GetButtonDown("Jump"))
        {
            AddForceOnJoint("Root", transform.up * _jumpStrength * 1000);
            AddForceOnJoint("LeftArm", transform.up * _jumpStrength * 1000);
            AddForceOnJoint("RightArm", transform.up * _jumpStrength * 1000);
        }
        if (InputManager.GetButtonDown("Grab"))
        {
            Grab();
        }
        if (!isRunningAnimation && input)
        {
            var random = new System.Random();
            var t = random.Next(10);
            if(t < 5)
                PlayAnimationByName(t < 3 ? "NarutoRun" : "Dab");
            PlayAnimationByName("Walk");
            isRunningAnimation = true;
        }
        if (!input)
        {
            isRunningAnimation = false;
        }
    }
    #region Animation
    public void PlayAnimationByName(string name)
    {
        // standard behavior will ignore the second condition if the first is false, avoiding running into a potential error
        var r = _animations.Animations.Where(t => t.Name == name);
        if(r.Any())
        {
            StartAnimation(r.First());
        }
    }

    public void StartAnimation(AnimationInfo.InfoAnimation animation, bool stopAfterTime = false)
    {
        var r = joints.FindAll(t => animation.UsedJoints.Contains(t.GetComponent<ConfigurableJoint>())).Select(t => t.GetComponent<CopyMotion>()).ToArray();
        if (r != null)
            for(int i = 0; i < r.Length; i++)
                CopyJointAnimation(r[i]);
        var rand = new System.Random();
        int t = rand.Next();
        _animator.SetTrigger(animation.Name);
        for (int i = 0; i < _currentAnimations.Count; i++)
            _currentAnimations[i] = new KeyValuePair<int, KeyValuePair<float, AnimationInfo.InfoAnimation>>(_currentAnimations[i].Key, new KeyValuePair<float, AnimationInfo.InfoAnimation>(_currentAnimations[i].Value.Key - Mathf.RoundToInt(animation.Anim.length * 1000), _currentAnimations[i].Value.Value));
        _currentAnimations.Add(new KeyValuePair<int, KeyValuePair<float, AnimationInfo.InfoAnimation>>(t, new KeyValuePair<float, AnimationInfo.InfoAnimation>(Mathf.RoundToInt(animation.Anim.length * 1000), animation)));
        if (!stopAfterTime)
        {
            ClemCAddons.Utilities.Timer.StartTimer(t, Mathf.RoundToInt(animation.Anim.length * 1000), StopCurrentAnimation, false);
        }
    }

    public void StopCurrentAnimation()
    {
        if(_currentAnimations.Count() > 0)
        {
            var res = _currentAnimations.Select(t => t.Value.Key).Min();
            var v = _currentAnimations.Find(e => e.Value.Key == res);
            ClemCAddons.Utilities.Timer.StopTimer(v.Key);
            var r = joints.FindAll(t => _currentAnimations[0].Value.Value.UsedJoints.Contains(t.GetComponent<ConfigurableJoint>())).Select(t => t.GetComponent<CopyMotion>()).ToArray();
            if (r != null)
                for (int i = 0; i < r.Length; i++)
                    StopCopyingJointAnimation(r[i]);
            _animator.SetTrigger(_currentAnimations[0].Value.Value.Name+"End");
            _currentAnimations.RemoveAt(0);
        }
    }

    private void CopyJointAnimation(CopyMotion joint)
    {
        joint.enabled = true;
    }

    private void StopCopyingJointAnimation(CopyMotion joint)
    {
        joint.enabled = false;
    }
    #endregion Animation

    #region Joints

    public void AddForceOnJoint(string joint, Vector3 force)
    {
        transform.FindDeep(joint).GetComponent<Rigidbody>().AddForce(force);
    }
    public void AddForceOnJoint(CopyMotion joint, Vector3 force)
    {
        joint.gameObject.GetComponent<Rigidbody>().AddForce(force);
    }
    public void AddForceOnJoint(Rigidbody joint, Vector3 force)
    {
        joint.gameObject.GetComponent<Rigidbody>().AddForce(force);
    }
    #endregion Joints
    #region Grab
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
            if ((_objectGrabbed = DetectObject()) != null)
            {
                if (_objectGrabbed.GetComponent<ConfigurableJoint>() != null)
                {
                    ConfigurableJoint fj = _objectGrabbed.GetComponent<ConfigurableJoint>();

                    fj.connectedBody = _trompeRigidbody;

                    fj.xMotion = ConfigurableJointMotion.Locked;
                    fj.yMotion = ConfigurableJointMotion.Locked;
                    fj.zMotion = ConfigurableJointMotion.Locked;

                    float xrot = _objectGrabbed.transform.rotation.x;
                    float yrot = _objectGrabbed.transform.rotation.y;
                    float zrot = _objectGrabbed.transform.rotation.z;

                    //QUEL BORDEL ENVIE DE ME BATTRE LA
                    // WHY DOESNT IT WANT TO TURN FFS!
                    //WHY DOES APPLYING THE ROTATION NOT TURN A GLOBAL ROTATION TO A LOCAL ROTATION

                    fj.anchor = Vector3.forward;
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
#endregion Grab
}

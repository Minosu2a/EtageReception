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
    [SerializeField] private float _maxSpeed = 2;
    [SerializeField] private float _jumpStrength = 1;
    [SerializeField, LabelOverride("Turning Speed %", rangeMin: 0, rangeMax: 100)] private float _turningSpeed = 10f;
    [SerializeField] private AnimationInfo _animations;
    private Transform _root;
    private Animator _animator;
    private List<GameObject> joints = new List<GameObject>();
    private List<KeyValuePair<int, KeyValuePair<float, AnimationInfo.InfoAnimation>>> _currentAnimations = new List<KeyValuePair<int, KeyValuePair<float, AnimationInfo.InfoAnimation>>>();
    private bool isRunningAnimation = true;

    [Header("Grab")]
    private bool _isGrabbing = false;

    private GameObject _centerGrabPoint = null;

    private GameObject _objectGrabbed = null;

    public Transform Root
    {
        get
        {
            _root = transform.FindDeep("Root");
            return _root;
        }
        set
        {
            _root = value;
        }
    }

    public Animator Animator
    {
        get
        {
            _animator = transform.FindDeep("AnimationBody").GetComponent<Animator>();
            return _animator;
        }
        set
        {
            _animator = value;
        }
    }

    public GrabDetection GrabDetection
    {
        get
        {
            return GrabDetection.GetComponent<GrabDetection>();
        }
    }

    public GameObject CenterGrabPoint
    {
        get
        {
            if(_centerGrabPoint == null)
                _centerGrabPoint = transform.FindDeep("GrabPoint").gameObject;
            return _centerGrabPoint;
        }
        set
        {
            _centerGrabPoint = value;
        }
    }

    public Rigidbody TrompeRigidbody
    {
        get
        {
            return CenterGrabPoint.transform.parent.GetComponent<Rigidbody>();
        }
    }

    void Start()
    {
        CharacterManager.Instance.CharacterController = this;
        var r = FindObjectsOfType<CopyMotion>();
        joints = r.Select(t => t.gameObject).ToList();
    }

    void Update()
    {
        Vector3 camera = Camera.allCameras[0].transform.forward.SetY(0).normalized; // never look down
        // the currently active camera, always valid unlike .current
        bool input = false;
        Vector3 right = camera.Right();
        if (InputManager.GetAxis("Forward") > 0)
        {
            input = true;
            AddForceOnJoint("Root", right * _speed * Time.deltaTime * 1000, _maxSpeed);
            AddForceOnJoint("LeftLeg", right * _speed * Time.deltaTime * 1000, _maxSpeed);
            AddForceOnJoint("RightLeg", right * _speed * Time.deltaTime * 1000, _maxSpeed);
            TurnRootTowards(right);
        } else if (InputManager.GetAxis("Forward") < 0)
        {
            input = true;
            AddForceOnJoint("Root", right * _speed * Time.deltaTime * -1000, _maxSpeed);
            AddForceOnJoint("LeftLeg", right * _speed * Time.deltaTime * -1000, _maxSpeed);
            AddForceOnJoint("RightLeg", right * _speed * Time.deltaTime * -1000, _maxSpeed);
            TurnRootTowards(right * -1);
        }
        if (InputManager.GetAxis("Strafe") > 0)
        {
            input = true;
            AddForceOnJoint("Root", camera * _speed * Time.deltaTime * 1000, _maxSpeed);
            AddForceOnJoint("LeftLeg", camera * _speed * Time.deltaTime * 1000, _maxSpeed);
            AddForceOnJoint("RightLeg", camera * _speed * Time.deltaTime * 1000, _maxSpeed);
            TurnRootTowards(camera * -1);
        }
        else if (InputManager.GetAxis("Strafe") < 0)
        {
            input = true;
            AddForceOnJoint("Root", camera * _speed * Time.deltaTime * -1000, _maxSpeed);
            AddForceOnJoint("LeftLeg", camera * _speed * Time.deltaTime * -1000, _maxSpeed);
            AddForceOnJoint("RightLeg", camera * _speed * Time.deltaTime * -1000, _maxSpeed);
            TurnRootTowards(camera);
        }
        if (InputManager.GetButtonDown("Jump"))
        {
            AddForceOnJoint("Root", transform.up * _jumpStrength * 1000, _maxSpeed);
            AddForceOnJoint("LeftArm", transform.up * _jumpStrength * 1000, _maxSpeed);
            AddForceOnJoint("RightArm", transform.up * _jumpStrength * 1000, _maxSpeed);
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

    private void TurnRootTowards(Vector3 vector)
    {
        var dir = vector.SetY(0).normalized;
        Root.GetComponent<ConfigurableJoint>().targetRotation =
            Quaternion.Lerp(Root.GetComponent<ConfigurableJoint>().targetRotation, dir.DirectionToQuaternion(), _turningSpeed * 0.1f * Time.deltaTime);
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
        Animator.SetTrigger(animation.Name);
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
            Animator.SetTrigger(_currentAnimations[0].Value.Value.Name+"End");
            _currentAnimations.RemoveAt(0);
        }
    }

    private void CopyJointAnimation(CopyMotion joint)
    {
        joint.enabled = true;
    }

    private void StopCopyingJointAnimation(CopyMotion joint)
    {
        joint.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.identity;
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
    public void AddForceOnJoint(string joint, Vector3 force, float max)
    {
        force = Root.GetComponent<Rigidbody>().velocity.magnitude > max ? Vector3.zero : force;
        transform.FindDeep(joint).GetComponent<Rigidbody>().AddForce(force);
    }
    public void AddForceOnJoint(CopyMotion joint, Vector3 force, float max)
    {
        force = Root.GetComponent<Rigidbody>().velocity.magnitude > max ? Vector3.zero : force;
        joint.GetComponent<Rigidbody>().AddForce(force);
    }
    public void AddForceOnJoint(Rigidbody joint, Vector3 force, float max)
    {
        force = Root.GetComponent<Rigidbody>().velocity.magnitude > max ? Vector3.zero : force;
        joint.AddForce(force);
    }

    #endregion Joints
    #region Grab
    public GameObject DetectObject()
    {
        if (GrabDetection.GrabRangeObject.Count > 0)
        {
            float smallerDistance = 0;
            int integerOfCloserObject = 0;

            for (int i = 0; i < GrabDetection.GrabRangeObject.Count; i++)
            {

                float dist = Vector3.Distance(GrabDetection.GrabRangeObject[i].transform.position, CenterGrabPoint.transform.position);

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

            GameObject closestObject = GrabDetection.GrabRangeObject[integerOfCloserObject];
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
                    
                    fj.connectedBody = TrompeRigidbody;

                    fj.xMotion = ConfigurableJointMotion.Locked;
                    fj.yMotion = ConfigurableJointMotion.Locked;
                    fj.zMotion = ConfigurableJointMotion.Locked;

                    fj.anchor = _objectGrabbed.transform.GetLocal(_objectGrabbed.transform.position.Direction(TrompeRigidbody.position)).NormalizeTo(0.5f);
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

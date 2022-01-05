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
    private List<string> _currentAnimations = new List<string>();
    private bool isRunningWalkAnimation = true;
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
            return CenterGrabPoint.GetComponent<GrabDetection>();
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
            var l = transform.FindDeep("LeftLeg");
            var r = transform.FindDeep("RightLeg");
            var groundLeft = GameTools.FindGround(l.position, 0, l.lossyScale.y, LayerMask.GetMask("Obstacles"));
            var groundRight = GameTools.FindGround(r.position, 0, r.lossyScale.y, LayerMask.GetMask("Obstacles"));
            if(!groundLeft.Equals(l.lossyScale.y) || !groundRight.Equals(r.lossyScale.y))
            {
                AddForceOnJoint("Root", transform.up * _jumpStrength * 1000, _maxSpeed);
                AddForceOnJoint("LeftArm", transform.up * _jumpStrength * 1000, _maxSpeed);
                AddForceOnJoint("RightArm", transform.up * _jumpStrength * 1000, _maxSpeed);
            }
        }
        if (InputManager.GetButtonDown("Grab"))
        {
            Grab();
        }
        if(input == true)
        {
            if(ClemCAddons.Utilities.Timer.MinimumDelay("hellobitch".GetHashCode(), 1000))
            {
                var random = new System.Random();
                var t = random.Next(10);
                if (t < 5)
                {
                    var currentAnimations = GetCurrentAnimations();
                    if(!currentAnimations.Contains("NarutoRun") && !currentAnimations.Contains("Dab"))
                        PlayAnimationByName(t < 3 ? "NarutoRun" : "Dab", false);
                }
            }
        }
        if (!isRunningWalkAnimation && input)
        {
            var rand = new System.Random();
            PlayAnimationByName("Walk"+rand.Next(1,3), true);
            isRunningWalkAnimation = true;
        }
        if (isRunningWalkAnimation && !input && transform.FindDeep("Root").GetComponent<Rigidbody>().velocity.magnitude < _maxSpeed * 0.1f)
        {
            StopCurrentAnimation("Walk1");
            StopCurrentAnimation("Walk2");
            StopCurrentAnimation("Walk3");
            isRunningWalkAnimation = false;
        }
    }

    private void TurnRootTowards(Vector3 vector)
    {
        var dir = vector.SetY(0).normalized;
        Root.GetComponent<ConfigurableJoint>().targetRotation =
            Quaternion.Lerp(Root.GetComponent<ConfigurableJoint>().targetRotation, dir.DirectionToQuaternion(), _turningSpeed * 0.1f * Time.deltaTime);
    }

    #region Animation
    public void PlayAnimationByName(string name, bool loop = false)
    {
        // standard behavior will ignore the second condition if the first is false, avoiding running into a potential error
        var r = _animations.Animations.Where(t => t.Name == name);
        if(r.Any())
        {
            StartAnimation(r.First(), loop);
        }
    }

    public string[] GetCurrentAnimations()
    {
        var r = new List<string>();
        for(int i = 0; i < Animator.parameterCount; i++)
        {
            if(Animator.GetBool(Animator.GetParameter(i).name) == true)
            {
                r.Add(Animator.GetParameter(i).name);
            }
        }
        return r.ToArray();
    }

    public void StartAnimation(AnimationInfo.InfoAnimation animation, bool loop = false)
    {
        var r = joints.FindAll(t => animation.UsedJoints.Contains(t.GetComponent<ConfigurableJoint>())).Select(t => t.GetComponent<CopyMotion>()).ToArray();
        if (r != null)
            for(int i = 0; i < r.Length; i++)
                CopyJointAnimation(r[i]);
        Animator.SetBool(animation.Name,true);
        _currentAnimations.Add(animation.Name);
        if (!loop)
        {
            ClemCAddons.Utilities.Timer.StartTimer(animation.Name.GetHashCode(), Mathf.RoundToInt(animation.Anim.length * 100), (obj) => StopCurrentAnimation(animation.Name), animation.Name, false);
        }
    }

    public void StopCurrentAnimation(string animation)
    {
        ClemCAddons.Utilities.Timer.StopTimer(animation.GetHashCode());
        var r = _animations.Animations.Where(t => t.Name == animation);
        var res = joints.FindAll(t => r.First().UsedJoints.Contains(t.GetComponent<ConfigurableJoint>())).Select(t => t.GetComponent<CopyMotion>()).ToArray();
        if (res != null)
            for (int i = 0; i < res.Length; i++)
                StopCopyingJointAnimation(res[i]);
        Animator.SetBool(animation, false);
        _currentAnimations.Remove(animation);
    }

    private void CopyJointAnimation(CopyMotion joint)
    {
        joint.enabled = true;
    }

    private void StopCopyingJointAnimation(CopyMotion joint)
    {
        joint.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.identity;
        joint.GetComponent<ConfigurableJoint>().targetPosition = Vector3.zero;
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
                    StartCoroutine(GrabE());
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
            var r = transform.FindDeep("TrunkTrashCan");
            GameObject[] toRemove = new GameObject[] { };
            for (int i = 0; i < r.childCount; i++)
            {
                toRemove = toRemove.Add(r.GetChild(i).gameObject);
            }
            StartCoroutine(RemoveUnElongate(toRemove, 10));
            
        }

    }

    private IEnumerator GrabE()
    {
        var dist = (_objectGrabbed.transform.position + _objectGrabbed.transform.position.Direction(TrompeRigidbody.transform.position).Multiply(_objectGrabbed.transform.lossyScale/1.5f)).Distance(TrompeRigidbody.transform);
        //                                                                                                                                                          not divided by 2 to go slightly inside the object
        var r = Mathf.RoundToInt(dist / TrompeRigidbody.transform.lossyScale.y);
        yield return CreateElongate(TrompeRigidbody.gameObject, 10, r, transform.FindDeep("TrunkTrashCan"));
        ConfigurableJoint fj = _objectGrabbed.GetComponent<ConfigurableJoint>();

        fj.connectedBody = TrompeRigidbody;

        fj.xMotion = ConfigurableJointMotion.Locked;
        fj.yMotion = ConfigurableJointMotion.Locked;
        fj.zMotion = ConfigurableJointMotion.Locked;

        fj.anchor = _objectGrabbed.transform.GetLocal(_objectGrabbed.transform.position.Direction(TrompeRigidbody.position)).NormalizeTo(0.5f);
        _isGrabbing = true;
    }

    private IEnumerator CreateElongate(GameObject source, float speed, int chainLength = 1, Transform parent = null)
    {
        GameObject r;
        if(parent != null)
            r = Instantiate(source, source.transform.position + Vector3.zero.SetY(0.05f), source.transform.rotation, parent);
        else
            r = Instantiate(source, source.transform.position + Vector3.zero.SetY(0.05f), source.transform.rotation);
        r.transform.localScale = source.transform.lossyScale.SetY(0);
        r.GetComponent<ConfigurableJoint>().connectedBody = null;
        while(r.transform.localScale.y < source.transform.lossyScale.y)
        {
            yield return new WaitForEndOfFrame();
            r.transform.position = source.transform.position + (source.transform.up * -1 * source.transform.lossyScale.y) * (r.transform.localScale.y / source.transform.lossyScale.y);
            r.transform.localScale = r.transform.localScale.SetY(r.transform.localScale.y + source.transform.lossyScale.y * Time.smoothDeltaTime * speed).Min(source.transform.lossyScale.y);
            r.transform.rotation = source.transform.rotation;
        }
        r.GetComponent<ConfigurableJoint>().connectedBody = source.GetComponent<Rigidbody>();
        chainLength--;
        if(chainLength > 0)
        {
            StartCoroutine(CreateElongate(r, speed, chainLength, parent));
        }
    }
    private IEnumerator RemoveUnElongate(GameObject[] objects, float speed)
    {
        while(objects.Length > 0)
        {
            yield return new WaitForEndOfFrame();
            var source = objects.Length > 1 ? objects[objects.Length - 2].transform : TrompeRigidbody.transform;
            objects[objects.Length - 1].transform.position = source.position + (source.up * -1 * source.lossyScale.y) * (source.localScale.y / source.lossyScale.y);
            objects[objects.Length - 1].transform.localScale -= Vector3.zero.SetY(Time.smoothDeltaTime * speed);
            if (objects[objects.Length - 1].transform.localScale.y <= 0)
            {
                Destroy(objects[objects.Length - 1]);
                objects = objects.RemoveAt(objects.Length - 1);
            }
        }
    }


    #endregion Grab
}

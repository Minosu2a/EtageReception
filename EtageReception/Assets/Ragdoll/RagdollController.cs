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
    [SerializeField] private float _distanceFeetPerc = 1.5f;
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _maxSpeed = 2;
    [SerializeField] private float _jumpStrength = 1;
    [SerializeField, LabelOverride("Turning Speed %", rangeMin: 0, rangeMax: 100)] private float _turningSpeed = 10f;
    [SerializeField] private AnimationInfo _animations;
    [SerializeField] private float DivideBy = 10f;
    private Transform _root;
    private Animator _animator;
    private List<GameObject> joints = new List<GameObject>();
    private List<string> _currentAnimations = new List<string>();
    private bool isRunningWalkAnimation = true;
    [Header("Grab")]
    private bool _isGrabbing = false;

    private bool groundLeft;
    private bool groundRight;

    private GameObject _centerGrabPoint = null;

    private GameObject _objectGrabbed = null;

    private int _trumpstate = 0;

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

    void FixedUpdate()
    {
        var l = transform.FindDeep("LeftLeg");
        var r = transform.FindDeep("RightLeg");
        var lValue = l.GetComponent<Renderer>().bounds.extents.y * 2 * _distanceFeetPerc;
        var rValue = l.GetComponent<Renderer>().bounds.extents.y * 2 * _distanceFeetPerc;
        var topl = l.GetComponent<Renderer>().bounds.center + Vector3.zero.SetY(l.GetComponent<Renderer>().bounds.extents.y);
        var topr = l.GetComponent<Renderer>().bounds.center + Vector3.zero.SetY(r.GetComponent<Renderer>().bounds.extents.y);
        Debug.DrawLine(topl, topl + l.forward.Down() * lValue, Color.red, 0.1f);
        groundLeft = Physics.Linecast(topl, topl + l.forward.Down() * lValue, 1 << LayerMask.NameToLayer("Default"));
        groundRight = Physics.Linecast(topr, topr + r.forward.Down() * rValue, 1 << LayerMask.NameToLayer("Default"));
    }

    void Update()
    {
        Vector3 camera = Camera.allCameras[0].transform.forward.SetY(0).normalized; // never look down
        // the currently active camera, always valid unlike .current
        bool input = false;
        Vector3 right = camera.Right();
        if (InputManager.GetAxis("Strafe") > 0)
        {
            input = true;
            AddForceOnJoint("Root", right * _speed * Time.deltaTime * 1000, _maxSpeed);
            AddForceOnJoint("LeftLeg", right * _speed * Time.deltaTime * 1000, _maxSpeed);
            AddForceOnJoint("RightLeg", right * _speed * Time.deltaTime * 1000, _maxSpeed);
            TurnRootTowards(camera);
        } else if (InputManager.GetAxis("Strafe") < 0)
        {
            input = true;
            AddForceOnJoint("Root", right * _speed * Time.deltaTime * -1000, _maxSpeed);
            AddForceOnJoint("LeftLeg", right * _speed * Time.deltaTime * -1000, _maxSpeed);
            AddForceOnJoint("RightLeg", right * _speed * Time.deltaTime * -1000, _maxSpeed);
            TurnRootTowards(camera * -1);
        }
        if (InputManager.GetAxis("Forward") > 0)
        {
            input = true;
            AddForceOnJoint("Root", camera * _speed * Time.deltaTime * 1000, _maxSpeed);
            AddForceOnJoint("LeftLeg", camera * _speed * Time.deltaTime * 1000, _maxSpeed);
            AddForceOnJoint("RightLeg", camera * _speed * Time.deltaTime * 1000, _maxSpeed);
            TurnRootTowards(right * -1);
        }
        else if (InputManager.GetAxis("Forward") < 0)
        {
            input = true;
            AddForceOnJoint("Root", camera * _speed * Time.deltaTime * -1000, _maxSpeed);
            AddForceOnJoint("LeftLeg", camera * _speed * Time.deltaTime * -1000, _maxSpeed);
            AddForceOnJoint("RightLeg", camera * _speed * Time.deltaTime * -1000, _maxSpeed);
            TurnRootTowards(right);
        }
        if (InputManager.GetButtonDown("Jump"))
        {
            if ((groundLeft || groundRight) && ClemCAddons.Utilities.Timer.MinimumDelay(666,500, true))
            {
                var l = transform.FindDeep("LeftLeg");
                var r = transform.FindDeep("RightLeg");
                Vector3 orientation;
                if (groundLeft && groundRight)
                {
                    orientation = (l.up + r.up) / 2;
                }
                else if (groundLeft)
                {
                    orientation = l.up;
                }
                else
                {
                    orientation = r.up;
                }
                AddForceOnJoint("Root", orientation * _jumpStrength * 1000);
                AddForceOnJoint("LeftArm", orientation * _jumpStrength * 1000);
                AddForceOnJoint("RightArm", orientation * _jumpStrength * 1000);
            }
        }
        if (InputManager.GetButtonDown("Grab"))
        {
            Grab();
        }
        if (InputManager.GetButtonDown("TrunkUp") || InputManager.GetAxis("TrunkUp") > 0.5f)
        {
            Straighten();
        }
        else if (InputManager.GetButtonDown("TrunkDown") || InputManager.GetAxis("TrunkDown") > 0.5f)
            Flacidify();
        if ((InputManager.GetButtonUp("TrunkUp") && InputManager.GetAxis("TrunkUp") < 0.5f) || (InputManager.GetButtonUp("TrunkDown") && InputManager.GetAxis("TrunkDown") < 0.5f))
            UnStraighten();
        if (InputManager.GetButtonDown("Eat") && _isGrabbing == false)
        {

            AudioManager.Instance.StartTrumpSound();
                //Lancer une petite animation
        }
        if(InputManager.GetButtonDown("Eat") && _isGrabbing == true && _objectGrabbed.tag == "Food")
        {
            Debug.Log("AAAh");
            EatFood();
            AudioManager.Instance.Start2DSound("S_Eat"); //Son de Miam
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
        Root.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Lerp(Root.GetComponent<ConfigurableJoint>().targetRotation,
            dir.FullToQuaternion(),
            _turningSpeed * 0.1f * Time.deltaTime);
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
            int integerOfCloserObject = -1;

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

            GameObject closestObject = null;

            if (integerOfCloserObject != -1)
            {
                closestObject = GrabDetection.GrabRangeObject[integerOfCloserObject];
            }

            return closestObject;

        }

        return null;
    }

    public void Grab()
    {
        if (_isGrabbing == false)
        {
            _objectGrabbed = DetectObject();

            if (_objectGrabbed != null)
            {
                if (_objectGrabbed.GetComponent<ConfigurableJoint>() != null)
                {
                    if(_objectGrabbed.GetComponent<SpecialSound>() != null)
                    {
                        AudioManager.Instance.StartSpecialSound(_objectGrabbed.GetComponent<SpecialSound>().Sound);
                    }
                    else
                    {
                        AudioManager.Instance.StartGrabSound(_objectGrabbed.tag);
                    }
                    StartCoroutine(GrabE());
                }
            }
        }
        else if (_isGrabbing == true)
        {
            AudioManager.Instance.Start2DSound("S_UnGrab");
            ConfigurableJoint fj = _objectGrabbed.GetComponent<ConfigurableJoint>();
            fj.connectedBody = null;
            fj.xMotion = ConfigurableJointMotion.Free;
            fj.yMotion = ConfigurableJointMotion.Free;
            fj.zMotion = ConfigurableJointMotion.Free;
            fj.anchor = Vector3.zero;
            _isGrabbing = false;
            var r = transform.FindDeep("TrunkTrashCan");
            GameObject[] toRemove = new GameObject[] { };
            for (int i = 0; i < r.childCount; i++)
            {
                toRemove = toRemove.Add(r.GetChild(i).gameObject);
            }
            _objectGrabbed.GetComponent<Rigidbody>().mass *= DivideBy;
            StartCoroutine(RemoveUnElongate(toRemove, 10));
            
        }

    }

     public void EatFood()
     {


            ConfigurableJoint fj = _objectGrabbed.GetComponent<ConfigurableJoint>();
            fj.connectedBody = null;
            fj.xMotion = ConfigurableJointMotion.Free;
            fj.yMotion = ConfigurableJointMotion.Free;
            fj.zMotion = ConfigurableJointMotion.Free;
            fj.anchor = Vector3.zero;
            _isGrabbing = false;
            var r = transform.FindDeep("TrunkTrashCan");
            GameObject[] toRemove = new GameObject[] { };
            for (int i = 0; i < r.childCount; i++)
            {
                toRemove = toRemove.Add(r.GetChild(i).gameObject);
            }
            _objectGrabbed.GetComponent<Rigidbody>().mass *= DivideBy;
            StartCoroutine(RemoveUnElongate(toRemove, 10));
            _objectGrabbed.SetActive(false);
        
 
     }

    private IEnumerator GrabE()
    {
        _isGrabbing = true;

        ConfigurableJoint fj = _objectGrabbed.GetComponent<ConfigurableJoint>();
        _objectGrabbed.GetComponent<Rigidbody>().mass *= 1 / DivideBy;
        var closest = _objectGrabbed.GetComponent<Rigidbody>().ClosestPointOnBounds(TrompeRigidbody.transform.position);
        var dist = closest.Distance(TrompeRigidbody.transform.position);
        var r = Mathf.FloorToInt(dist / TrompeRigidbody.transform.lossyScale.y);
        var trashcan = transform.FindDeep("TrunkTrashCan");

        yield return StartCoroutine(CreateElongate(TrompeRigidbody.gameObject, 10, r, trashcan));
        fj.autoConfigureConnectedAnchor = false;
        fj.connectedAnchor = TrompeRigidbody.transform.position + (TrompeRigidbody.transform.position.Direction(closest) * r * TrompeRigidbody.transform.lossyScale.y);
        var x = fj.xDrive;
        x.positionSpring = 100;
        fj.xDrive = x;
        var y = fj.yDrive;
        y.positionSpring = 100;
        fj.yDrive = y;
        var z = fj.zDrive;
        z.positionSpring = 100;
        fj.zDrive = z;

        yield return new WaitForSeconds(0.5f);
        fj.autoConfigureConnectedAnchor = true;
        x = fj.xDrive;
        x.positionSpring = 0;
        fj.xDrive = x;
        y = fj.yDrive;
        y.positionSpring = 0;
        fj.yDrive = y;
        z = fj.zDrive;
        z.positionSpring = 0;
        fj.zDrive = z;

        fj.connectedBody = trashcan.GetChild(trashcan.childCount-1).GetComponent<Rigidbody>();
        fj.xMotion = ConfigurableJointMotion.Locked;
        fj.yMotion = ConfigurableJointMotion.Locked;
        fj.zMotion = ConfigurableJointMotion.Locked;

        fj.anchor = _objectGrabbed.transform.GetLocal(_objectGrabbed.transform.position.Direction(TrompeRigidbody.position)).normalized.NormalizeTo(_objectGrabbed.transform.lossyScale.Inverse() * 1.7f);
        //need to figure out WHY 1.7 in this particular project
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
    #region Straighten
    private void Straighten()
    {
        var list = new List<Transform>();
        var parent = TrompeRigidbody.transform.parent;
        for(int i = 0; i < parent.childCount; i++)
        {
            if(parent.GetChild(i).name != "TrunkTrashCan")
            {
                list.Add(parent.GetChild(i));
            }
            else
            {
                for(int f = 0; f < parent.GetChild(i).childCount; f++)
                {
                    list.Add(parent.GetChild(i).GetChild(f));
                }
            }
        }
        int num = list.Count;
        foreach(Transform t in list)
        {
            var joint = t.GetComponent<ConfigurableJoint>().yDrive;
            joint.positionSpring = 10000000;
            t.GetComponent<ConfigurableJoint>().xDrive = t.GetComponent<ConfigurableJoint>().zDrive = joint;

            t.GetComponent<ConfigurableJoint>().yDrive = joint;
            t.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Euler(-2f*num, 0, 0);
            var drive = t.GetComponent<ConfigurableJoint>().angularXDrive;
            drive.positionSpring = 10000000;
            t.GetComponent<ConfigurableJoint>().angularXDrive =
                t.GetComponent<ConfigurableJoint>().angularYZDrive =
                    drive;
            num--;
        }
    }
    private void UnStraighten()
    {
        var list = new List<Transform>();
        var parent = TrompeRigidbody.transform.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name != "TrunkTrashCan")
            {
                list.Add(parent.GetChild(i));
            }
            else
            {
                for (int f = 0; f < parent.GetChild(i).childCount; f++)
                {
                    list.Add(parent.GetChild(i).GetChild(f));
                }
            }
        }
        foreach (Transform t in list)
        {
            var joint = t.GetComponent<ConfigurableJoint>().yDrive;
            joint.positionSpring = 10000;
            t.GetComponent<ConfigurableJoint>().xDrive = t.GetComponent<ConfigurableJoint>().zDrive = new JointDrive();

            t.GetComponent<ConfigurableJoint>().yDrive = joint;
            t.GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0, 0, 0, 1);
            var drive = t.GetComponent<ConfigurableJoint>().angularXDrive;
            drive.positionSpring = 10000;
            t.GetComponent<ConfigurableJoint>().angularXDrive =
                t.GetComponent<ConfigurableJoint>().angularYZDrive =
                    drive;
        }
    }
    private void Flacidify()
    {
        var list = new List<Transform>();
        var parent = TrompeRigidbody.transform.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name != "TrunkTrashCan")
            {
                list.Add(parent.GetChild(i));
            }
            else
            {
                for (int f = 0; f < parent.GetChild(i).childCount; f++)
                {
                    list.Add(parent.GetChild(i).GetChild(f));
                }
            }
        }
        foreach (Transform t in list)
        {
            var joint = t.GetComponent<ConfigurableJoint>().yDrive;
            joint.positionSpring = 0;
            t.GetComponent<ConfigurableJoint>().xDrive = t.GetComponent<ConfigurableJoint>().zDrive = new JointDrive();

            t.GetComponent<ConfigurableJoint>().yDrive = joint;
            t.GetComponent<ConfigurableJoint>().targetRotation = new Quaternion(0, 0, 0, 1);
            var drive = t.GetComponent<ConfigurableJoint>().angularXDrive;
            drive.positionSpring = 0;
            t.GetComponent<ConfigurableJoint>().angularXDrive =
                t.GetComponent<ConfigurableJoint>().angularYZDrive =
                    drive;
        }
    }
    #endregion Straighten
}

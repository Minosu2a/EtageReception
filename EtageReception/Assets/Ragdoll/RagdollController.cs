using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ClemCAddons.Utilities;
using System;
using ClemCAddons;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _jumpStrength = 1;
    [SerializeField] private AnimationInfo _animations;
    private Animator _animator;
    private List<GameObject> joints = new List<GameObject>();
    private List<KeyValuePair<int, KeyValuePair<float, AnimationInfo.InfoAnimation>>> _currentAnimations = new List<KeyValuePair<int, KeyValuePair<float, AnimationInfo.InfoAnimation>>>();

    private bool isRunningAnimation = true;

    void Start()
    {
        var r = FindObjectsOfType<CopyMotion>();
        joints = r.Select(t => t.gameObject).ToList();
        _animator = transform.FindDeep("AnimationBody").GetComponent<Animator>();
    }

    void Update()
    {
        bool input = false;
        if (Input.GetKey(KeyCode.W))
        {
            input = true;
            AddForceOnJoint("Root",transform.right * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LeftLeg", transform.right * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("RightLeg", transform.right * _speed * Time.deltaTime * 1000);
        }
        if (Input.GetKey(KeyCode.S))
        {
            input = true;
            AddForceOnJoint("Root", transform.right * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LeftLeg", transform.right * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("RightLeg", transform.right * _speed * Time.deltaTime * -1000);
        }
        if (Input.GetKey(KeyCode.A))
        {
            input = true;
            AddForceOnJoint("Root", transform.forward * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LeftLeg", transform.forward * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("RightLeg", transform.forward * _speed * Time.deltaTime * 1000);
        }
        if (Input.GetKey(KeyCode.D))
        {
            input = true;
            AddForceOnJoint("Root", transform.forward * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LeftLeg", transform.forward * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("RightLeg", transform.forward * _speed * Time.deltaTime * -1000);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddForceOnJoint("Root", transform.up * _jumpStrength * 1000);
            AddForceOnJoint("LeftArm", transform.up * _jumpStrength * 1000);
            AddForceOnJoint("RightArm", transform.up * _jumpStrength * 1000);
        }
        if(!isRunningAnimation && input)
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
}

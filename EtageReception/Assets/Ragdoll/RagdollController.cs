using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ClemCAddons.Utilities;
using System;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _jumpStrength = 1;
    [SerializeField] private AnimationInfo _animations;
    private List<GameObject> joints = new List<GameObject>();
    private List<KeyValuePair<int, AnimationInfo.InfoAnimation>> _currentAnimations = new List<KeyValuePair<int, AnimationInfo.InfoAnimation>>();

    private bool isRunningAnimation = true;

    void Start()
    {
        var r = FindObjectsOfType<CopyMotion>();
        joints = r.Select(t => t.gameObject).ToList();
    }

    void Update()
    {
        bool input = false;
        if (Input.GetKey(KeyCode.W))
        {
            input = true;
            AddForceOnJoint("Root",transform.right * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LegL", transform.right * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LegR", transform.right * _speed * Time.deltaTime * 1000);
        }
        if (Input.GetKey(KeyCode.S))
        {
            input = true;
            AddForceOnJoint("Root", transform.right * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LegL", transform.right * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LegR", transform.right * _speed * Time.deltaTime * -1000);
        }
        if (Input.GetKey(KeyCode.A))
        {
            input = true;
            AddForceOnJoint("Root", transform.forward * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LegL", transform.forward * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LegR", transform.forward * _speed * Time.deltaTime * 1000);
        }
        if (Input.GetKey(KeyCode.D))
        {
            input = true;
            AddForceOnJoint("Root", transform.forward * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LegL", transform.forward * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LegR", transform.forward * _speed * Time.deltaTime * -1000);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddForceOnJoint("Root", transform.up * _jumpStrength * 1000);
            AddForceOnJoint("ArmL", transform.up * _jumpStrength * 1000);
            AddForceOnJoint("ArmR", transform.up * _jumpStrength * 1000);
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

    public void StartAnimation(AnimationInfo.InfoAnimation animation)
    {
        var r = joints.FindAll(t => animation.UsedJoints.Contains(t.GetComponent<ConfigurableJoint>())).Select(t => t.GetComponent<CopyMotion>()).ToArray();
        if (r != null)
            for(int i = 0; i < r.Length; i++)
                CopyJointAnimation(r[i]);
        var rand = new System.Random();
        int t = rand.Next();
        _currentAnimations.Add(new KeyValuePair<int,AnimationInfo.InfoAnimation>(t, animation));
        ClemCAddons.Utilities.Timer.StartTimer(t, Mathf.RoundToInt(animation.Anim.length * 1000), StopCurrentAnimation, false);
    }

    public void StopCurrentAnimation()
    {
        if(_currentAnimations.Count() > 0)
        {
            ClemCAddons.Utilities.Timer.StopTimer(_currentAnimations[0].Key);
            var r = joints.FindAll(t => _currentAnimations[0].Value.UsedJoints.Contains(t.GetComponent<ConfigurableJoint>())).Select(t => t.GetComponent<CopyMotion>()).ToArray();
            if (r != null)
                for (int i = 0; i < r.Length; i++)
                    StopCopyingJointAnimation(r[i]);
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
        transform.Find(joint).GetComponent<Rigidbody>().AddForce(force);
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

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
    private AnimationInfo.InfoAnimation? _currentAnimation = null;

    void Start()
    {
        var r = FindObjectsOfType<CopyMotion>();
        joints = r.Select(t => t.gameObject).ToList();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            AddForceOnJoint("Root",transform.right * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LegL", transform.right * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LegR", transform.right * _speed * Time.deltaTime * 1000);
        }
        if (Input.GetKey(KeyCode.S))
        {
            AddForceOnJoint("Root", transform.right * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LegL", transform.right * _speed * Time.deltaTime * -1000);
            AddForceOnJoint("LegR", transform.right * _speed * Time.deltaTime * -1000);
        }
        if (Input.GetKey(KeyCode.A))
        {
            AddForceOnJoint("Root", transform.forward * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LegL", transform.forward * _speed * Time.deltaTime * 1000);
            AddForceOnJoint("LegR", transform.forward * _speed * Time.deltaTime * 1000);
        }
        if (Input.GetKey(KeyCode.D))
        {
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
    }

    public void PlayAnimationByName(string name)
    {
        // standard behavior will ignore the second condition if the first is false, avoiding running into a potential error
        var r = _animations.Animations.Find(t => t.HasValue && t.Value.Name == name);
        if(r.HasValue)
        {
            StartAnimation(r.Value);
        }
    }

    public void StartAnimation(AnimationInfo.InfoAnimation animation)
    {
        var r = joints.FindAll(t => animation.UsedJoints.Contains(t.GetComponent<ConfigurableJoint>())).Select(t => t.GetComponent<CopyMotion>()).ToArray();
        if (r != null)
            for(int i = 0; i < r.Length; i++)
                CopyJointAnimation(r[i]);
        _currentAnimation = animation;
        ClemCAddons.Utilities.Timer.StartTimer(0, Mathf.RoundToInt(animation.Anim.clip.length * 1000), StopCurrentAnimation, false);
    }

    public void StopCurrentAnimation()
    {
        if(_currentAnimation.HasValue)
        {
            ClemCAddons.Utilities.Timer.StopTimer(0);
            var r = joints.FindAll(t => _currentAnimation.Value.UsedJoints.Contains(t.GetComponent<ConfigurableJoint>())).Select(t => t.GetComponent<CopyMotion>()).ToArray();
            if (r != null)
                for (int i = 0; i < r.Length; i++)
                    StopCopyingJointAnimation(r[i]);
            _currentAnimation = null;
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

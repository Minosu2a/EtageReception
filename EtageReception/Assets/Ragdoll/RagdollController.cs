using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _jumpStrength = 1;

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

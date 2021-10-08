using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private ConfigurableJoint _cj;
    [SerializeField] private bool _mirror = true;

    void Update()
    {
        if(_mirror)
            _cj.targetRotation = Quaternion.Inverse(_target.rotation);
        else
            _cj.targetRotation = _target.rotation;
    }
}

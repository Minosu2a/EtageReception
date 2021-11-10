using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClemCAddons;

public class CopyMotion : MonoBehaviour
{
    [SerializeField] private bool _mirror = true;
    [SerializeField] private int _howDeep = 2;
    private Transform _copyTarget;
    private ConfigurableJoint _cj;

    void Start()
    {
        _cj = GetComponent<ConfigurableJoint>();
        _copyTarget = transform.GetParent(_howDeep).Find("a" + gameObject.name);
        Debug.Log(_copyTarget);
    }

    void Update()
    {
        if(_mirror)
            _cj.targetRotation = Quaternion.Inverse(_copyTarget.rotation);
        else
            _cj.targetRotation = _copyTarget.rotation;
    }
}

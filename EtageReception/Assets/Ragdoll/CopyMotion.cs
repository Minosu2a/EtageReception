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
        _copyTarget = transform.FindParentDeep("Elephant").FindDeep("a" + gameObject.name);
    }

    void Update()
    {
        _cj.targetPosition = _copyTarget.localPosition;
        if (_mirror)
        {
            _cj.targetRotation = Quaternion.Inverse(_copyTarget.localRotation);
        }
        else
            _cj.targetRotation = _copyTarget.localRotation;
    }
}

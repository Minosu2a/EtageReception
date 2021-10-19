using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInfo : MonoBehaviour
{
    [SerializeField] private List<InfoAnimation?> _animations;

    public List<InfoAnimation?> Animations { get => _animations; set => _animations = value; }

    [Serializable]
    public struct InfoAnimation
    {
        public string Name;
        public Animation Anim;
        public List<ConfigurableJoint> UsedJoints;
    }

}

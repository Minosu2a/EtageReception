using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationInfo
{
    [SerializeField] private List<InfoAnimation> _animations;

    public List<InfoAnimation> Animations { get => _animations; set => _animations = value; }


    [Serializable]
    public struct InfoAnimation
    {
        public string Name;
        public AnimationClip Anim;
        public List<ConfigurableJoint> UsedJoints;
    }

}

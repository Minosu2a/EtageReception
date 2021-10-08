using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInfo : MonoBehaviour
{
    [SerializeField] private List<InfoAnimation> _animations;

    void Update()
    {
        
    }

    struct InfoAnimation
    {
        public string Name;
        public Animation Anim;
        public List<ConfigurableJoint> UsedJoints;
    }
}

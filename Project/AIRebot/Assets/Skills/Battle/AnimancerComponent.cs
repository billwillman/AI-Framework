using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillEditor.Data;
using Animancer;

namespace SkillEditor.Runtime
{
    public class AnimancerComponent : AnimationBaseComponent
    {
        Animancer.AnimancerComponent m_Component = null;

        protected override void Awake() {
            base.Awake();
            if (m_Component == null)
                m_Component = GetComponent<Animancer.AnimancerComponent>();
        }
    }
}

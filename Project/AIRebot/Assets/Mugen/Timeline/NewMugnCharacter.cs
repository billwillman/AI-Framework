using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMugnCharacter : PlatformCharacter
{
    private ImageAnimation m_ImageAnimation;
    private Animation m_Animation;

    public ImageAnimation MugenImageAnimation {
        get {
            if (m_ImageAnimation == null)
                m_ImageAnimation = GetComponentInChildren<ImageAnimation>();
            return m_ImageAnimation;
        }
    }

    public Animation UnityAnimation {
        get {
            if (m_Animation == null)
                m_Animation = GetComponentInChildren<Animation>();
            return m_Animation;
        }
    }
}

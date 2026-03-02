using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMugnCharacter : PlatformCharacter
{
    private ImageAnimation m_ImageAnimation;

    public ImageAnimation MugenImageAnimation {
        get {
            if (m_ImageAnimation == null)
                m_ImageAnimation = GetComponentInChildren<ImageAnimation>();
            return m_ImageAnimation;
        }
    }
}

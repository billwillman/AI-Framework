using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
// 站立类型
public enum TStandType
{
    S, // 站立
    C, // 下蹲
    A, // 空中
    L  // 倒地
}

// 招式类型
public enum TMoveType
{
    None,
    A, // 攻击
    I, // 非攻击
    H // 受击
}

// 物理类型
public enum TPhysicType
{
    S,  // 站立
    C,  // 蹲
    A  // 空中
}
*/

// 数值属性
public class FighterAttribute {
    // 能量(技能招式使用)
    public int Power;
}

[RequireComponent(typeof(CnsCommander))]
public class Fighter : MonoBehaviour {
    private int m_Hp = 100;
    
    private FighterAttribute m_Attribute = new FighterAttribute();

    public FighterAttribute Attribute {
        get {
            return m_Attribute;
        }
    }

    

    public int Hp {
        get {
            return m_Hp;
        }
    }

    public bool IsAlive {
        get {
            return Hp > 0;
        }
    }

    protected CnsCommander m_Cns = null;
    protected CnsCommander Cns {
        get {
            if (m_Cns == null)
                m_Cns = GetComponent<CnsCommander>();
            return m_Cns;
        }
    }

    public bool IsFlipX {
        get {
            return this.Cns.IsFlipX;
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using BehavicTree;

[System.Serializable]
public class WALK
{
    public float fwd = 1f;
    public float back = -1f;
}

[System.Serializable]
public class RUN
{
    public Vector2 fwd = new Vector2(2f, 0);
    public Vector2 back = new Vector2(-2f, -2f);
}

[System.Serializable]
public class JUMP
{
    public Vector2 neu = new Vector2(0, -3f); // 中跳
    public Vector2 fwd = new Vector2(3f, 0f); // 向前跳
    public Vector2 back = new Vector2(-3f, 0f); // 向后跳
}

[System.Serializable]
public class RUNJUMP
{
    public Vector2 fwd = new Vector2(6f, -6f);
    public Vector2 back = new Vector2(-6f, -6f);
}

[System.Serializable]
// 两段跳
public class AIRJUMP
{
    public Vector2 neu = new Vector2(0, -6f);
    public Vector2 fwd = new Vector2(3f, 0f);
    public Vector2 back = new Vector2(-3f, 0);
}

[System.Serializable]
public class Velocity
{
    public WALK walk = new WALK();
    public RUN run = new RUN();
    public JUMP jump = new JUMP();
    public RUNJUMP runjump = new RUNJUMP();
    public AIRJUMP airjump = new AIRJUMP();
}

[System.Serializable]
public class SIZE
{
    public float xscale = 1f;
    public float yscale = 1f;
    public float shadowoffset = 0f;
}

public class Movement : MonoBehaviour {

    public Velocity m_Velocity = new Velocity();
    public SIZE m_Size = new SIZE();

    protected bool IsFlipX {
        get {
            //var sp = this.SpRender;
            //return sp.flipX;
            return this.transform.localScale.x < 0;
        }
    }

    // 速度，Y跟UNITY的方向相反
    public Vector2 Vec;

    // 加速度
    public Vector2 Acc;

    // 重力加速度
    public float g = 9.8f;

    // 摩擦因子
    public static float U = 0.005f;

    public Vector2 velMul = new Vector2(1f, 1f);

    void FixedUpdate() {
        UpdateMove(Time.fixedDeltaTime);
    }

    /*
    void OnImageAnimationFrame(ImageAnimation target) {
       // UpdateMove(Time.deltaTime);
    }
    */

    private FightAgent m_Agent = null;
    private CnsCommander m_Cns = null;

    protected FightAgent Agent {
        get {
            if (m_Agent == null)
                m_Agent = GetComponent<FightAgent>();
            return m_Agent;
        }
    }

    protected CnsCommander Cns {
        get {
            if (m_Cns == null)
                m_Cns = GetComponent<CnsCommander>();
            return m_Cns;
        }
    }

    public TStandType StandType {
        get {
            /*
             * var agent = this.Agent;
                if (agent == null)
                    return TStandType.S;
                return agent._get_type();
             */
            var cns = this.Cns;
            if (cns != null) {
                return cns.type;
            } else
                return TStandType.S;
        }
    }

    private static readonly float _cGPer = 5f;
    private static readonly Vector2 _cVel = new Vector2(1f, 1f);

    void UpdateMove(float deltaTime) {
        if (Mathf.Abs(Vec.x) <= float.Epsilon && Mathf.Abs(Vec.y) <= float.Epsilon) {
            if (this.StandType != TStandType.A)
                return;
        }

        if (this.StandType == TStandType.A) {
            float gg = (g * _cGPer + Acc.y);

            Vec.y *= velMul.y;

            Vec.y += gg * deltaTime;
        }

        Vec.x = Vec.x * velMul.x;
        float Ax = IsFlipX ? -Acc.x : Acc.x;
        Vector2 vv = new Vector2(Vec.x * (IsFlipX ? -1 : 1), -Vec.y);
        if (this.StandType != TStandType.A && (Mathf.Abs(vv.x) > float.Epsilon || Mathf.Abs(Ax) > float.Epsilon)) {
            float oldVx = vv.x;
            float u = Movement.U * Movement.U/100f;
            float aX = u * g;
            if (vv.x > 0)
                aX = -aX;
            aX += Ax;

            // 加速度算速度
            vv.x = aX * deltaTime + vv.x;
            if (oldVx * vv.x < 0)
                vv.x = 0;

            Vec.x = IsFlipX ? -vv.x : vv.x;
        }

        vv *= _cVel;

        Vector2 org = this.transform.position;
        org += vv * deltaTime;
        if (org.y < 0)
            org.y = 0;

        this.transform.position = org;
    }

}

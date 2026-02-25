using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExplodCreator: System.ICloneable
{
    public int anim;
    public Vector2Int offset;
    public TProjPostype postype = TProjPostype.p1;
    public int bindID = ImageAnimation._cNoVaildState;
    public float bindtime = -2;
    public int removetime = -2;
    public int sprpriority = 0;
    public Vector2 vel = Vector2.zero;
    public Vector2 accel;

    public object Clone() {
        return this.MemberwiseClone();
    }
}

[RequireComponent(typeof(CnsCommander))]
[RequireComponent(typeof(ImageAnimation))]
public class Explod : MonoBehaviour
{
    public ExplodCreator creator = new ExplodCreator();
    private float m_RuntimeRemoveTime = 0f;
    private int m_RuntimeAnimFrame = ImageAnimation._cNoVaildState;
    private float m_RuntimeBindTime = ImageAnimation._cNoVaildState;

    private CnsCommander m_Cns = null;
    public CnsCommander Cns {
        get {
            if (m_Cns == null)
                m_Cns = GetComponent<CnsCommander>();
            return m_Cns;
        }
    }
    private ImageAnimation m_Anim = null;
    public ImageAnimation Anim {
        get {
            if (m_Anim == null)
                m_Anim = GetComponent<ImageAnimation>();
            return m_Anim;
        }
    }
    private Movement m_Mover = null;
    public Movement Mover {
        get {
            if (m_Mover == null)
                m_Mover = GetComponent<Movement>();
            return m_Mover;
        }
    }

    public void Attach(ExplodCreator creator) {
        if (creator != null) {
            this.creator =  creator.Clone() as ExplodCreator;

            this.Cns.sprpriority(this.creator.sprpriority);
            this.Mover.Vec = this.creator.vel;
            this.Mover.Acc = this.creator.accel;

            var binder = Common.GetFighter(creator.bindID);
            if (binder != null) {
                Common.ApplyPosOffset(this.creator.postype, this.gameObject, binder.Owner, null, creator.offset);
            }

            m_RuntimeBindTime = this.creator.bindtime;

            if (creator.removetime > 0)
                m_RuntimeRemoveTime = creator.removetime;
            else
                m_RuntimeRemoveTime = 0f;

            UpdateRemove();
        }
    }

    void UpdateBind() {
        if (creator.bindID == ImageAnimation._cNoVaildState || creator.bindID == -2)
            return;
        
        // -1
        bool isAllBinding = Mathf.Abs(creator.bindtime + 1) <= float.Epsilon;
        if (!isAllBinding) {
            if (m_RuntimeBindTime >= 0) {
                float deltaTime = Time.fixedDeltaTime;
                m_RuntimeBindTime -= deltaTime;
            } else
                return;
        }

        var binder = Common.GetFighter(creator.bindID);
        if (binder != null) {
            Common.ApplyPosOffset(this.creator.postype, this.gameObject, binder.Owner, null, creator.offset);
        }
    }

    void UpdateRemove() {
        if (creator.removetime == -1) {
            if (this.Anim.CurrActionNo != this.creator.anim)
                this.Anim.PlayerPlayerAni(this.creator.anim, true);
        } else if (creator.removetime == -2) {
            if (this.Anim.CurrActionNo != this.creator.anim)
                this.Anim.PlayerPlayerAni(this.creator.anim, false);
            if (this.Cns.AnimTime() == 0)
                this.Cns.DestroySelf();
        } else if (creator.removetime > 0) {
            if (this.Anim.CurrActionNo != this.creator.anim)
                this.Anim.PlayerPlayerAni(this.creator.anim, false);
        }
    }

    void OnImageAnimationFrame(ImageAnimation target) {

        if (target.CurFrame != ImageAnimation._cNoVaildState) {

            if (m_RuntimeAnimFrame == ImageAnimation._cNoVaildState) {
                m_RuntimeAnimFrame = target.CurFrame;
                return;
            }

            if (m_RuntimeAnimFrame == target.CurFrame)
                return;
            m_RuntimeAnimFrame = target.CurFrame;

            if (this.creator.removetime > 0) {
                --m_RuntimeRemoveTime;
                if (m_RuntimeRemoveTime <= 0)
                    this.Cns.DestroySelf();
            }

            //UpdateRemove();
        }
    }

    private void FixedUpdate() {
        UpdateBind();
        UpdateRemove();
    }
}

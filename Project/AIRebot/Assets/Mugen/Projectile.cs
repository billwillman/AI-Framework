using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TProjCollisionedType
{
    None = 0,
    Destroy = 1
};

public enum TProjPostype
{
    // 说明pos相对于p1的坐标轴。正的x坐标偏移量朝向p1的前方。该参数是postype的缺省值。
    p1,
    // 说明pos相对于p2的坐标轴。正的x坐标偏移量朝向p2的前方（1.0改为了p1的前方）。如果p2不存在，该位置会根据p1计算并且提示错误
    p2,
    // 说明xpos相对于p1面向的屏幕边缘。正的x坐标偏移量朝向p1的前方
    front,
    //  说明xpos相对于p1背向的屏幕边缘，正的x坐标偏移量朝向p1的前方。
    back,
    // 说明xpos相对于屏幕左边。正的x坐标偏移量朝向p1前方。
    left,
    // 说明xpos相对于屏幕右边。正的x坐标偏移量朝向p1前方。
    right
}

public enum ProjectileStatus
{
    None = 0,
    Fly,
    Hit,
    HitMiss,
    Destroying,
    DoDestroy,
    DoDestroyed,
}

[System.Serializable]
public struct ProjectileCreator
{
    public int projID;
    public int projAnim;
    public int projHitAnim;
    public Vector2 projScale;

}

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ImageAnimation))]
[RequireComponent(typeof(CnsCommander))]
public class Projectile : MonoBehaviour
{
    // 指定涉及的该飞行道具ID号。若指定应为正。
    public int ProjID = ImageAnimation._cNoVaildState;
    // 为使用飞行道具的动画而指定动画的动作号。
    public int projanim = ImageAnimation._cNoVaildState;
    // 当飞行道具击中对手时指定播放动画的动作号。
    public int projhitanim = ImageAnimation._cNoVaildState;
    // 当飞行道具被移除（由于时间到期或击到它的移除边缘，等等）时指定播放动画的动作号。若省略，将由projhitanim代替。
    public int projremanim = ImageAnimation._cNoVaildState;
    // 当飞行道具在击中另一个飞行道具时被抵消时指定播放动画的动作号。若省略，将由projremanim代替。
    public int projcancelanim = ImageAnimation._cNoVaildState;
    // 指定飞行道具的缩放比例因数。飞行道具的最终比例由该参数和p1常量文件内[Size]组的“proj.doscale”两个参数共同决定。若省略，缺省为1,1（普通尺寸）。
    public Vector2 projscale = new Vector2(1f, 1f);
    // 设置非0值使得飞行道具在击中后被移除，或0取消该行为。缺省为1
    public TProjCollisionedType collisionedType = TProjCollisionedType.Destroy;
    // 指定飞行道具应从屏幕中被移除时间帧（tick）数。缺省为-1（永不移除）。
    public int projremovetime;

    private Movement m_Mover = null;
    protected Movement mover {
        get {
            if (m_Mover == null)
                m_Mover = GetComponent<Movement>();
            return m_Mover;
        }
    }
    private CnsCommander m_Cns = null;
    public CnsCommander Cns {
        get {
            if (m_Cns == null)
                m_Cns = GetComponent<CnsCommander>();
            return m_Cns;
        }
    }

    protected ImageAnimation m_Anim = null;
    public ImageAnimation Anim {
        get {
            if (m_Anim == null)
                m_Anim = GetComponent<ImageAnimation>();
            return m_Anim;
        }
    }

    // 指定飞行道具行进的x和y的初速度。若省略，缺省为0,0
    public Vector2 velocity;

    // 指定飞行道具正被移除时行进的x和y的速度。若省略，缺省为0,0。
    public Vector2 remvelocity;
    // 指定应用于飞行道具在x和y方向的加速度。若省略，缺省为0,0。
    public Vector2 accel;

    // 指定x和y的速度乘数。飞行道具的速度在每一个时间（帧）都将乘以该乘数。若省略，乘数缺省为1。
    public Vector2 velmul;

    // 指定被移除前飞行道具能够产生的hit数。缺省为1。
    public int projhits = 1;
    // 若飞行道具产生多次hit，则miss_time指定能造成下个hit前必须经过的最少时间帧（tick）数。缺省为0，但你很可能需要非0值。
    public int projmisstime = 0;
    private int m_runtimeProjmisstime = 0;
    private int m_runtimeAnimFrameNo = ImageAnimation._cNoVaildState;
    // 指定飞行道具优先级。若飞行道具与另一个同等优先级的飞行道具碰撞，则它们都被删去。若碰撞上一个低于它优先级的飞行道具，则将删去低优先级的飞行道具，并将高优先级的那个飞行道具的优先级减去1。
    public int projpriority = 0;
    // 指定精灵（sprite，这里理解为图层，下略）的优先权。高优先级的图层绘制在低优先级的图层之上。缺省为3。
    public int projsprpriority = 3;
    // 这是飞行道具离开屏幕边缘后被自动删除的距离。缺省为40。（1.0的缺省分别为：在240p中为40，在480p中为80，在720p中为160
    public int projedgebound = 40;
    // 指定飞行道具被删除前可以在舞台上（不是屏幕）行进的最远距离。缺省为40。（1.0的缺省分别为：在240p中为40，在480p中为80，在720p中为160）
    public int projstagebound = 40;
    // 指定飞行道具被创建时的x和y坐标偏移量。若省略，两个参数都缺省为0。飞行道具被创建时总是与角色相同方向，off_x于飞行道具的朝向关联。该偏移量参数的准确行为依赖于postype（参数）。
    public Vector2Int offset;
    // type_string定义postype如何说明（解释）pos参数。任何情况下，正的y坐标偏移量表示向下的位移。在任何情况下，off_y于角色位置都有关联。
    public TProjPostype postype = TProjPostype.p1;

    private ProjectileStatus m_Status = ProjectileStatus.None;

    public void Attach(ProjectileCreator creator) {
        this.ProjID = creator.projID;
        this.projanim = creator.projAnim;
        this.projhitanim = creator.projHitAnim;
        this.projscale = creator.projScale;

        ProjectileMgr.Instance.AddProjectile(this.ProjID, this);
    }

    private void ApplyPosOffset(CnsCommander owner, CnsCommander target) {
        Common.ApplyPosOffset(this.postype, this.gameObject, owner, target, this.offset);
    }

    // 开始飞行
    public void StartFly(CnsCommander owner, CnsCommander target) {
        this.Anim.PlayerPlayerAni(projanim, true);
        this.mover.Vec = velocity;
        this.mover.velMul = velmul;
        this.mover.Acc = accel;
        this.Cns.sprpriority(this.projsprpriority);
        this.transform.localScale = new Vector3(projscale.x, projscale.y, 1f);
        this.m_Status = ProjectileStatus.Fly;

        ApplyPosOffset(owner, target);
    }

    private void FixedUpdate() {
        switch (m_Status) {
            case ProjectileStatus.Hit: {
                    --projhits;
                    if (projhits > 0) {
                        m_Status = ProjectileStatus.HitMiss;
                        m_runtimeProjmisstime = this.projmisstime;
                        m_runtimeAnimFrameNo = ImageAnimation._cNoVaildState;
                    } else {
                        m_Status = ProjectileStatus.Destroying;
                    }
                    this.Anim.PlayerPlayerAni(projhitanim, false);
                }
                break;

            case ProjectileStatus.Destroying:
                if (this.Cns.AnimTime() <= 0) {
                    m_Status = ProjectileStatus.DoDestroy;
                }
                break;
            case ProjectileStatus.DoDestroy: {
                    if (this.collisionedType == TProjCollisionedType.Destroy) {
                        m_Status = ProjectileStatus.DoDestroyed;
                        this.m_Cns.DestroySelf();
                    } else {
                        m_Status = ProjectileStatus.Fly;
                        this.Anim.PlayerPlayerAni(projanim, true);
                    }
                }
                break;
        }
    }

    void OnImageAnimationFrame(ImageAnimation target) {
        if (target.CurrActionNo != ImageAnimation._cNoVaildState && m_Status == ProjectileStatus.HitMiss) {
            if (m_runtimeAnimFrameNo == ImageAnimation._cNoVaildState) {
                m_runtimeAnimFrameNo = target.CurFrame;
                m_runtimeProjmisstime = this.projmisstime;
            } else {
                if (m_runtimeAnimFrameNo != target.CurFrame) {
                    m_runtimeAnimFrameNo = target.CurFrame;
                    --m_runtimeProjmisstime;
                    if (m_runtimeProjmisstime <= 0) {
                        m_Status = ProjectileStatus.Fly;
                        this.Anim.PlayerPlayerAni(projanim, true);
                    }
                }
            }
        }

        //UpdateBehavicTree(true);
    }

    // 发生碰撞
    public void OnCollision(CnsCommander owner, CnsCommander target) {
        if (target == null || m_Status != ProjectileStatus.Fly)
            return;
        m_Status = ProjectileStatus.Hit;
       
    }

    private void OnDestroy() {
        if (ResourceMgr.Instance.IsQuitApp)
            return;
        ProjectileMgr.Instance.RemoveProjectile(this.ProjID);
    }

}
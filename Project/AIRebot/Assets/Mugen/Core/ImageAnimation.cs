using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public enum ActionFlip
{
    afNone,
    afH,
    afV,
    afHV
}

public enum ActionDrawMode
{
    adNone,
    ad_A,
    ad_S,
    ad_A1
}

[System.Serializable]
public struct RectF
{
	private float m_XMin;

	private float m_YMin;

	private float m_Width;

	private float m_Height;

	public static RectF zero = new RectF(0f, 0f, 0f, 0f);

	public float x {
		get {
			return m_XMin;
		}
		set {
			m_XMin = value;
		}
	}

	public float y {
		get {
			return m_YMin;
		}
		set {
			m_YMin = value;
		}
	}

	public Vector2 position {
		get {
			return new Vector2(m_XMin, m_YMin);
		}
		set {
			m_XMin = value.x;
			m_YMin = value.y;
		}
	}

	public Vector2 center {
		get {
			return new Vector2(x + m_Width / 2f, y + m_Height / 2f);
		}
		set {
			m_XMin = value.x - m_Width / 2f;
			m_YMin = value.y - m_Height / 2f;
		}
	}

	public Vector2 min {
		get {
			return new Vector2(xMin, yMin);
		}
		set {
			xMin = value.x;
			yMin = value.y;
		}
	}

	public Vector2 max {
		get {
			return new Vector2(xMax, yMax);
		}
		set {
			xMax = value.x;
			yMax = value.y;
		}
	}
    public float width {
        get {
            return m_Width;
        }
        set {
            m_Width = value;
        }
    }

    public float height {
        get {
            return m_Height;
        }
        set {
            m_Height = value;
        }
    }

    public Vector2 size {
        get {
            return new Vector2(m_Width, m_Height);
        }
        set {
            m_Width = value.x;
            m_Height = value.y;
        }
    }

    public float xMin {
        get {
            return m_XMin;
        }
        set {
            float xMax = this.xMax;
            m_XMin = value;
            m_Width = xMax - m_XMin;
        }
    }

    public float yMin {
        get {
            return m_YMin;
        }
        set {
            float yMax = this.yMax;
            m_YMin = value;
            m_Height = yMax - m_YMin;
        }
    }

    public float xMax {
        get {
            return m_Width + m_XMin;
        }
        set {
            m_Width = value - m_XMin;
        }
    }

    public float yMax {
        get {
            return m_Height + m_YMin;
        }
        set {
            m_Height = value - m_YMin;
        }
    }
    public RectF(float x, float y, float width, float height) {
        m_XMin = x;
        m_YMin = y;
        m_Width = width;
        m_Height = height;
    }

    public RectF(Vector2 position, Vector2 size) {
        m_XMin = position.x;
        m_YMin = position.y;
        m_Width = size.x;
        m_Height = size.y;
    }

    public RectF(RectF source) {
        m_XMin = source.m_XMin;
        m_YMin = source.m_YMin;
        m_Width = source.m_Width;
        m_Height = source.m_Height;
    }

    public static RectF MinMaxRect(float xmin, float ymin, float xmax, float ymax) {
        return new RectF(xmin, ymin, xmax - xmin, ymax - ymin);
    }

    public void Set(float x, float y, float width, float height) {
        m_XMin = x;
        m_YMin = y;
        m_Width = width;
        m_Height = height;
    }

    public bool Contains(Vector2 point) {
        return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
    }

    public bool Contains(Vector3 point) {
        return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax;
    }

    public bool Contains(Vector3 point, bool allowInverse) {
        if (!allowInverse) {
            return Contains(point);
        }

        bool flag = false;
        if ((width < 0f && point.x <= xMin && point.x > xMax) || (width >= 0f && point.x >= xMin && point.x < xMax)) {
            flag = true;
        }

        if (flag && ((height < 0f && point.y <= yMin && point.y > yMax) || (height >= 0f && point.y >= yMin && point.y < yMax))) {
            return true;
        }

        return false;
    }

    private static RectF OrderMinMax(RectF rect) {
        if (rect.xMin > rect.xMax) {
            float xMin = rect.xMin;
            rect.xMin = rect.xMax;
            rect.xMax = xMin;
        }

        if (rect.yMin > rect.yMax) {
            float yMin = rect.yMin;
            rect.yMin = rect.yMax;
            rect.yMax = yMin;
        }

        return rect;
    }

    public bool Overlaps(RectF other) {
        return other.xMax > xMin && other.xMin < xMax && other.yMax > yMin && other.yMin < yMax;
    }

  
    public bool Overlaps(RectF other, bool allowInverse) {
        RectF rect = this;
        if (allowInverse) {
            rect = OrderMinMax(rect);
            other = OrderMinMax(other);
        }

        return rect.Overlaps(other);
    }

    public static Vector2 NormalizedToPoint(RectF rectangle, Vector2 normalizedRectCoordinates) {
        return new Vector2(Mathf.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), Mathf.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
    }

    public static Vector2 PointToNormalized(RectF rectangle, Vector2 point) {
        return new Vector2(Mathf.InverseLerp(rectangle.x, rectangle.xMax, point.x), Mathf.InverseLerp(rectangle.y, rectangle.yMax, point.y));
    }

    public static bool operator !=(RectF lhs, RectF rhs) {
        return !(lhs == rhs);
    }

    public static bool operator ==(RectF lhs, RectF rhs) {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
    }

    public override int GetHashCode() {
        return x.GetHashCode() ^ (width.GetHashCode() << 2) ^ (y.GetHashCode() >> 2) ^ (height.GetHashCode() >> 1);
    }

    public override bool Equals(object other) {
        if (!(other is RectF)) {
            return false;
        }

        RectF rect = (RectF)other;
        return x.Equals(rect.x) && y.Equals(rect.y) && width.Equals(rect.width) && height.Equals(rect.height);
    }


    public override string ToString() {
        return string.Format("(x:{0:F2}, y:{1:F2}, width:{2:F2}, height:{3:F2})", x, y, width, height);
    }

    public string ToString(string format) {
        return string.Format("(x:{0}, y:{1}, width:{2}, height:{3})", x.ToString(format), y.ToString(format), width.ToString(format), height.ToString(format));
    }
}

[System.Serializable]
public struct ImageAnimateNode
{
	public int actionNo;
	public int frameGroup;
	public int frameIndex;
	public float AniTick;
	public ActionFlip flipTag;
	public ActionDrawMode drawMode;
	public bool isLoopStart;
	public RectF[] localClsn2Arr;
	public RectF[] defaultClsn2Arr;
	public RectF[] localCls1Arr;
}

// 角色动画处理
//[RequireComponent(typeof(Unit))]
public class ImageAnimation 
    : MonoBehaviour
    //SkillEditor.Runtime.AnimationBaseComponent
    {

    public void Load(byte[] buffer) {
        if (mStateAniMap != null)
            mStateAniMap.Clear();
           
        BinaryFormatter formatter = new BinaryFormatter();

        MemoryStream stream = new MemoryStream(buffer);
        mStateAniMap = formatter.Deserialize(stream) as Dictionary<int, List<ImageAnimateNode>>;

    }

    public bool ContainsAnim(int animNo) {
        if (mStateAniMap == null || mStateAniMap.Count <= 0)
            return false;
        return mStateAniMap.ContainsKey(animNo);
    }

    public int CurrActionNo {
        get {
            return m_CurrActionNo;
        }
    }

    public List<ImageAnimateNode> CurrActionList {
        get {
            if (m_CurrActionNo == _cNoVaildState || mStateAniMap == null || mStateAniMap.Count <= 0)
                return null;
            List<ImageAnimateNode> ret;
            if (!mStateAniMap.TryGetValue(m_CurrActionNo, out ret))
                ret = null;
            return ret;
        }
    }

    public bool IsLoop {
        get {
            return m_IsLoop;
        }
    }

    public int CurFrame {
        get {
            return m_CurFrame;
        }
    }

    public bool NextFrame() {
        return NextFrame(true);
    }

    public void Pause(bool isPause) {
        bool isEnable = !isPause;
        var ani = this.CacheAnimation;
        if (ani != null && ani.enabled != isEnable)
            ani.enabled = isEnable;
        //var info = ani [_cPlayAnimationName];
    }

    public bool NextFrame(bool checkLimitStop) {
        bool ret = UpdateFrame(CurFrame + 1);
        if (ret && checkLimitStop) {
            int limitEnd = this.LimitEndFrame;
            if (limitEnd >= 0 && limitEnd == this.m_CurFrame) {
                Pause(true);
            }
        }
        return ret;
    }

    public bool PrevFrame() {
        return PrevFrame(true);
    }

    protected int LimitStartFrame {
        get {
            int limitStart = m_LimitStartFrame;
            if (limitStart < 0)
                return limitStart;
            int curFrameCount = this.AniNodeCount;
            if (limitStart >= curFrameCount)
                limitStart = curFrameCount - 1;
            return limitStart;
        }
    }

    public bool PrevFrame(bool checkLimitStop) {
        bool ret = UpdateFrame(m_CurFrame - 1);
        if (ret && checkLimitStop) {
            int limitStart = this.LimitStartFrame;
            if (limitStart >= 0 && limitStart == this.m_CurFrame) {
                Pause(true);
            }
        }
        return ret;
    }

    public void StartFrame() {
        m_LoopStart = -1;
        m_LoopStartAniTime = -1;
        //  if (m_AniUsedTime < 0)
        //      m_AniUsedTime = 0;
    }

    public void EndFrame() {
        var frameList = this.FrameList;
        if ((frameList == null) || (frameList.Count <= 0))
            return;

        DoEndFrame();
        if (IsLoop || m_LoopStart >= 0) {
            int loopStart = m_LoopStart;
            if (loopStart < 0)
                loopStart = 0;
            if (UpdateFrame(loopStart)) {
                // 移动Animation
                if (m_LoopStart >= 0 && m_LoopStartAniTime >= 0) {
                    var aniCtl = this.CacheAnimation;
                    if (aniCtl != null) {
                        var info = aniCtl[this.CurrActionNoName];
                        if (info != null)
                            info.time = m_LoopStartAniTime;
                    }
                }
            }
        }
    }

    public void StopAndReset(bool clearAnimMap = true) {
        Stop();
        ResetAnimation();

        if (clearAnimMap && mStateAniMap != null) {
            mStateAniMap.Clear();
        }
    }

    public void Stop() {
        var frameList = this.FrameList;
        if (frameList == null || frameList.Count <= 0)
            return;
        CacheAnimation.Stop();
    }

    public void SetLimitFrame(int startFrame = -1, int endFrame = -1, bool checkCurrentFrame = true) {
        m_LimitStartFrame = startFrame;
        m_LimitEndFrame = endFrame;
        if (checkCurrentFrame) {
            int oldFrame = m_CurFrame;
            bool isChanged = CheckCurrentFrameLimit();
            if (isChanged && oldFrame != m_CurFrame) {
                var ani = this.CacheAnimation;
                if (ani != null) {
                    Pause(true);
                    if (ani.clip != null) {
                        var info = ani[this.CurrActionNoName];
                        if (info != null) {
                            float t = CalcAnimationTime(m_CurFrame);
                            info.time = t;
                        }
                    }
                }
                DoChangeFrame();
            }
        }
    }

    public bool PlayerPlayerAni(int actionNo, bool isLoop = true) {
        return PlayerPlayerAni(actionNo, -1, -1, isLoop);
    }

    /*
    public override void PlayAnimation(string name, bool loop) {
        int actionNo;
        if (int.TryParse(name, out actionNo)) {
            PlayerPlayerAni(actionNo, loop);
        }
    }
    */

    public bool PlayerPlayerAni(int actionNo, int startFrame, int endFrame, bool isLoop = true) {

        if (!ContainsAnim(actionNo))
            return false;

        
        if (this.m_CurrActionNo == actionNo) {
            if (CacheAnimation.enabled && !CacheAnimation.isPlaying) {
                CacheAnimation.Play();
            }
            SetLimitFrame(startFrame, endFrame, false);
            return true;
        }
        
        ResetAnimation();

        bool ret = DoInitAnimation(actionNo);

        if (ret) {
            m_IsLoop = isLoop;
            //m_PrevState = m_State;
            m_CurrActionNo = actionNo;

            CacheAnimation.Stop();
            if (isLoop)
                CacheAnimation.wrapMode = WrapMode.Loop;
            else
                CacheAnimation.wrapMode = WrapMode.Once;
            CacheAnimation.Play();

            if (!CacheAnimation.enabled)
                CacheAnimation.enabled = true;

            SetLimitFrame(startFrame, endFrame, false);
        } else {
            m_IsLoop = isLoop;
            // m_PrevState = m_State;
            // m_State = state;
            m_CurrActionNo = actionNo;

            DoEndFrame();
        }

        return ret;
    }

    private void InitAnimationClip(int actionNo) {
        // 加载对应animation
        string evtName = "OnAnimationLoad";
        SendMessage(evtName, actionNo, SendMessageOptions.DontRequireReceiver);

        var frameList = this.FrameList;
        if (frameList == null || frameList.Count < 2) {
            Animation ctl = this.CacheAnimation;
            if (ctl.enabled)
                ctl.enabled = false;
        }
    }

    private bool DoInitAnimation(int actionNo) {
        m_CurrActionNo = actionNo;
        var frameList = this.FrameList;
        if (frameList == null || frameList.Count <= 0) {
            ResetAnimation();
            return false;
        }

        m_CurFrame = 0;
        InitAnimationClip(actionNo);

        // DoChangeFrame();
       // DoInitAnimationEvt();

        return true;
    }

    void DoEndFrame() {
      //  m_AniUsedTime = 100000;
        string evtName = "OnImageAnimationEndFrame";
        SendMessage(evtName, this, SendMessageOptions.DontRequireReceiver);
    }

    private void ResetAnimation() {
        m_CurFrame = -1;
        // m_AniTotalTime = -1;
        // ResetCns();

        var anim = this.CacheAnimation;
        var actionName = this.CurrActionNoName;
        if (anim != null && !string.IsNullOrEmpty(actionName))
            anim.RemoveClip(actionName);

        m_CurrActionName = string.Empty;
        m_CurrActionNo = _cNoVaildState;

        
    }

    private bool CheckFrameLimit(ref int frameIndex) {
        if (m_LimitStartFrame >= 0 && m_LimitEndFrame >= 0 && m_LimitStartFrame > m_LimitEndFrame)
            return false;
        int curFrameCount = this.AniNodeCount;
        int limitStart = m_LimitStartFrame;
        if (limitStart >= curFrameCount)
            limitStart = curFrameCount - 1;
        int limitEnd = m_LimitEndFrame;
        if (limitEnd >= curFrameCount)
            limitEnd = curFrameCount - 1;
        bool ret = false;
        if (limitStart >= 0) {
            if (m_CurFrame < limitStart) {
                m_CurFrame = limitStart;
                ret = true;
            }
        }

        if (limitEnd >= 0) {
            if (m_CurFrame > limitEnd) {
                m_CurFrame = limitEnd;
                ret = true;
            }
        }

        return ret;
    }

    private bool CheckCurrentFrameLimit() {
        return CheckFrameLimit(ref m_CurFrame);
    }

    private bool UpdateFrame(int frameIndex) {
        var frameList = this.FrameList;
        if (frameList == null || frameList.Count <= 0)
            return false;
        if (frameIndex < 0) {
            if (IsLoop)
                frameIndex = frameList.Count - 1;
            else
                frameIndex = 0;
        } else if (frameIndex >= frameList.Count) {
            if (IsLoop)
                frameIndex = 0;
            else
                frameIndex = frameList.Count - 1;
        }
        int oldFrame = m_CurFrame;
        m_CurFrame = frameIndex;
        bool isChanged = CheckCurrentFrameLimit();
        if (oldFrame != m_CurFrame) {
            if (m_CurFrame >= 0 && m_CurFrame < frameList.Count) {
                var node = frameList[m_CurFrame];

                if (isChanged) {
                    var aniCtl = this.CacheAnimation;
                    if (aniCtl != null && aniCtl.isPlaying && aniCtl.clip != null) {
                        var info = aniCtl[this.CurrActionNoName];
                        if (info != null) {
                            float t = CalcAnimationTime(m_CurFrame);
                            info.time = t;
                        }
                    }
                }

                if (node.isLoopStart) {
                    var aniCtl = this.CacheAnimation;
                    if (aniCtl != null && aniCtl.isPlaying && aniCtl.clip != null) {
                        if (m_LoopStart != m_CurFrame) {
                            var info = aniCtl[this.CurrActionNoName];
                            if (info != null) {
                                m_LoopStart = m_CurFrame;
                                m_LoopStartAniTime = info.time;
                                CheckLoopStartLimit();
                            }

                        }
                    }
                }
            }

            DoChangeFrame();
        } else {
            if (isChanged) {
                var aniCtl = this.CacheAnimation;
                if (aniCtl != null && aniCtl.isPlaying && aniCtl.clip != null) {
                    var info = aniCtl[this.CurrActionNoName];
                    if (info != null) {
                        float t = CalcAnimationTime(m_CurFrame);
                        info.time = t;
                    }
                }
            }
        }
        return true;
    }

    public bool IsPlaying {
        get {
            var frameList = this.FrameList;
            if (frameList == null || frameList.Count <= 0)
                return false;
            return CacheAnimation.isPlaying;
        }
    }

    void DoInitAnimationEvt() {
        string evtName = "OnImageAnimationInit";
        SendMessage(evtName, this, SendMessageOptions.DontRequireReceiver);
    }

    void DoChangeFrame() {
      //  m_AnimElemTime = 0;

        string evtName = "OnImageAnimationFrame";
        SendMessage(evtName, this, SendMessageOptions.DontRequireReceiver);
    }

    private void CheckLoopStartLimit() {
        if (m_LimitEndFrame >= 0 && m_LimitStartFrame >= 0 &&
            m_LimitEndFrame < m_LimitStartFrame)
            return;

        int curFrameCount = this.AniNodeCount;
        int limitStart = m_LimitStartFrame;
        if (limitStart >= curFrameCount)
            limitStart = curFrameCount - 1;
        int limitEnd = m_LimitEndFrame;
        if (limitEnd >= curFrameCount)
            limitEnd = curFrameCount - 1;

        bool isChanged = false;
        if (limitStart >= 0) {
            if (m_LoopStart < limitStart) {
                m_LoopStart = limitStart;
                isChanged = true;
            }
        }

        if (limitEnd >= 0) {
            if (m_LoopStart > limitEnd) {
                m_LoopStart = limitEnd;
                isChanged = true;
            }
        }

        if (isChanged) {
            // 重新计算时间
            m_LoopStartAniTime = CalcAnimationTime(m_LoopStart);
        }
    }

    private float CalcAnimationTime(int ff) {
        var frameList = this.FrameList;
        if (frameList == null || frameList.Count <= 0)
            return -1;
        int curFrame = ff;
        if (curFrame < 0)
            curFrame = 0;
        else if (curFrame >= frameList.Count)
            curFrame = frameList.Count - 1;
        float ret = 0;
        for (int i = 0; i <= curFrame; ++i) {
            ImageAnimateNode frame = frameList[i];
            float evtTime = frame.AniTick * _cImageAnimationScale;
            ret += evtTime;
        }
        return 0;
    }

    public List<ImageAnimateNode> FrameList {
        get {
            return this.CurrActionList;
        }
    }

    public int AniNodeCount {
        get {
            var frameList = this.FrameList;
            if (frameList != null)
                return frameList.Count;
            return 0;
        }
    }

    protected int LimitEndFrame {
        get {
            int limitEnd = m_LimitEndFrame;
            if (limitEnd < 0)
                return limitEnd;
            int curFrameCount = this.AniNodeCount;
            if (limitEnd >= curFrameCount)
                limitEnd = curFrameCount - 1;
            return limitEnd;
        }
    }

    public bool HasAniData {
        get {
            var frameList = this.FrameList;
            return (frameList != null) && (frameList.Count > 0);
        }
    }

    public Animation CacheAnimation {
        get {
            if (m_Animation == null)
                m_Animation = GetComponent<Animation>();
            return m_Animation;
        }
    }

    protected string CurrActionNoName {
        get {
            if (string.IsNullOrEmpty(m_CurrActionName)) {
                if (m_CurrActionNo == _cNoVaildState)
                    return string.Empty;
                m_CurrActionName = m_CurrActionNo.ToString();
            }
            return m_CurrActionName;
        }
    }

    public Dictionary<int, List<ImageAnimateNode>> ActionMap {
        get {
            return mStateAniMap;
        }
    }

    public bool CurrAnimNode(out ImageAnimateNode node) {
        if (m_CurFrame < 0) {
            node = new ImageAnimateNode();
            return false;
        }
        var lst = this.FrameList;
        if (lst == null || lst.Count <= 0 || m_CurFrame >= lst.Count) {
            node = new ImageAnimateNode();
            return false;
        }

        node = lst[m_CurFrame];

        return true;
    }

    public static float _cImageAnimationScale = 0.017f;

    public static readonly short _cNoVaildState = -9999;
    private Dictionary<int, List<ImageAnimateNode>> mStateAniMap = null;
    private int m_CurrActionNo = _cNoVaildState;
    private bool m_IsLoop = false;
    private int m_CurFrame = -1;
    // 限定动画帧范围
    private int m_LimitStartFrame = -1;
    private int m_LimitEndFrame = -1;
    private Animation m_Animation = null;
    private int m_LoopStart = -1;
    private float m_LoopStartAniTime = -1;
    private string m_CurrActionName = string.Empty;
}

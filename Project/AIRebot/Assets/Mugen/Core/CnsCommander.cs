using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using BehavicTree;

public struct VarValue
{
    public float fValue;
    public int iValue;
};

struct StateFrameInfo {
    public int stateRunFrameNum;
    private int curFrame;

    public void Init() {
        this.stateRunFrameNum = 0;
        this.curFrame = ImageAnimation._cNoVaildState;
    }

    public void InitAnim() {
        this.stateRunFrameNum = 0;
        this.curFrame = 0;
    }

    public void ChangeState() {
        this.curFrame = ImageAnimation._cNoVaildState;
        this.stateRunFrameNum = 0;
    }

    public void AttachCurFrame(int curFrame, int maxFrameNum) {
        if (maxFrameNum <= 0)
            return;

        if (curFrame == this.curFrame)
            return;

        if (this.curFrame == ImageAnimation._cNoVaildState) {
            this.stateRunFrameNum = 0;
            this.curFrame = curFrame;
            return;
        }

        ++this.stateRunFrameNum;

        this.curFrame = curFrame;
    }

    public void AttachCurFrame(ImageAnimation target) {
        if (target == null || target.CurrActionNo == ImageAnimation._cNoVaildState)
            return;
        var list = target.CurrActionList;
        if (list == null || list.Count <= 0)
            return;
        AttachCurFrame(target.CurFrame, list.Count);
    }
}

[RequireComponent(typeof(ImageAnimation))]
[RequireComponent(typeof(PlayerLoader))]
[RequireComponent(typeof(Fighter))]
[RequireComponent(typeof(Movement))]
public class CnsCommander : MonoBehaviour
{
    private Dictionary<int, VarValue> m_VarMap = new Dictionary<int, VarValue>();
    private ImageAnimation m_Anim = null;
    private PlayerLoader m_PlayerLoader = null;
    private int m_Ctrl = 1;
    private Fighter m_Fighter = null;
    private Movement m_Movement = null;
    private SpriteRenderer m_SpriteRenderer = null;
#if _USE_NGUI
    private UI2DSprite m_UI2DSprite = null;
#endif
    private int m_PrevStateNo = ImageAnimation._cNoVaildState;
    private int m_CurrStateNo = ImageAnimation._cNoVaildState;
   // private FightAgent m_Agent = null;
   // private behaviac.EBTStatus m_AgentStatus = behaviac.EBTStatus.BT_INVALID;
    private bool m_StatePlayAnim = false;

    public TStandType type = TStandType.S;
    public TMoveType movetype = TMoveType.None;
    public TPhysicType physics = TPhysicType.S;

    private Dictionary<string, LuaFunction> m_LuaObjMap = null;

    private LuaFunction GetCacheLuaObj(string funcName) {
        if (string.IsNullOrEmpty(funcName) || m_LuaObjMap == null)
            return null;
        LuaFunction ret;
        if (!m_LuaObjMap.TryGetValue(funcName, out ret))
            ret = null;
        return ret;
    }

    private R CallLuaFuncRet<T, R>(string funcName, T param) {
        if (string.IsNullOrEmpty(funcName))
            return default(R);

        R ret = default(R);
        LuaFunction func = GetCacheLuaObj(funcName) as LuaFunction;
        if (func == null) {
            /*
            LuaState lua = main.Instance.LuaEngine;
            if (lua == null)
                return ret;
            lua.GetFunction(funcName);*/
            func = SOC.GamePlay.GameStart.EnvLua.Global.Get<LuaFunction>(funcName);
            if (func == null)
                return ret;
            m_LuaObjMap[funcName] = func;
        }
        
        //func.BeginPCall();
        try {
            /*
            func.Push(this);
            func.PushGeneric<T>(param);
            func.PCall();
            ret = func.CheckValue<R>();
            */
            var result = func.Call(this, param)[0];
            ret = (R)result;
        } catch (Exception e) {
#if DEBUG
            Debug.LogError(e.ToString());
#endif
        }
        //func.EndPCall();

        return ret;
    }

    private void CallLuaFunc<T>(string funcName, T param) {

        LuaFunction func = GetCacheLuaObj(funcName) as LuaFunction;

        if (func == null) {
            /*
            LuaState lua = main.Instance.LuaEngine;
            if (lua == null)
                return;
            func = lua.GetFunction(funcName);
            */
            func = SOC.GamePlay.GameStart.EnvLua.Global.Get<LuaFunction>(funcName);
            if (func == null)
                return;
            m_LuaObjMap[funcName] = func;
        }

        //func.BeginPCall();
        try {
            /*
            func.Push(this);
            func.PushGeneric<T>(param);
            func.PCall();*/
            func.Call(this, param);
        } catch (Exception e) {
#if DEBUG
            Debug.LogError(e.ToString());
#endif
        }
        //func.EndPCall();
    }

    public void CallLuaFunc(string funcName, string json = null) {
        CallLuaFunc<string>(funcName, json);
    }

    public void CallLuaFunc(string funcName, long value) {
        CallLuaFunc<long>(funcName, value);
    }

    public void CallLuaFunc(string funcName, bool value) {
        CallLuaFunc<bool>(funcName, value);
    }

    public void CallLuaFunc(string funcName, double value) {
        CallLuaFunc<double>(funcName, value);
    }

    public bool CallLuaFuncRetBool(string funcName, object obj) {
        return CallLuaFuncRet<object, bool>(funcName, obj);
    }

    public long CallLuaFuncRetLong(string funcName, object obj) {
        return CallLuaFuncRet<object, long>(funcName, obj);
    }

    public double CallLuaFuncRetDouble(string funcName, object obj) {
        return CallLuaFuncRet<object, double>(funcName, obj);
    }

    public LuaTable CallLuaFuncRetTable(string funcName, string json = null) {
        return CallLuaFuncRet<string, LuaTable>(funcName, json);
    }

    public long CallLuaFuncRetLong(string funcName, string json = null) {
        return CallLuaFuncRet<string, long>(funcName, json);
    }

    public string CallLuaFuncRetStr(string funcName, string json = null) {
        return CallLuaFuncRet<string, string>(funcName, json);
    }

    // 计算当前状态走了多少多少帧了
    private StateFrameInfo m_StateFrameInfo = new StateFrameInfo();

    private static Dictionary<int, VarValue> m_GlobalVarMap = new Dictionary<int, VarValue>();

    private void Awake() {
        m_StateFrameInfo.Init();
    }

    void OnImageAnimationInit(ImageAnimation target) {
        m_StateFrameInfo.InitAnim();
    }

    void OnImageAnimationFrame(ImageAnimation target) {
        if (m_CurrStateNo != ImageAnimation._cNoVaildState && target.CurrActionNo != ImageAnimation._cNoVaildState && m_StatePlayAnim) {
            m_StateFrameInfo.AttachCurFrame(target);
        }

        //UpdateBehavicTree(true);
    }

    public void velset(float x, float y) {
        var mover = this.Mover;
        mover.Vec.x = x;
        mover.Vec.y = y;
    }

    public Vector2 Vec() {
        var mover = this.Mover;
        return mover.Vec;
    }

    public float VelX() {
        return this.VecX();
    }

    public float VelY() {
        return this.VecY();
    }

    public float VecX() {
        return this.Vec().x;
    }

    public float VecY() {
        return this.Vec().y;
    }

    public void xScaleSet(float scale) {
        var trans = this.transform;
        var localScale = trans.localScale;
        localScale.x = scale;
        trans.localScale = localScale;
    }

    public void yScaleSet(float scale) {
        var trans = this.transform;
        var localScale = trans.localScale;
        localScale.y = scale;
        trans.localScale = localScale;
    }

    public float SysFVar(int idx) {
        VarValue ret;
        if (m_GlobalVarMap.TryGetValue(idx, out ret))
            return ret.fValue;
        return 0f;
    }

    public int SysVar(int idx) {
        VarValue ret;
        if (m_GlobalVarMap.TryGetValue(idx, out ret))
            return ret.iValue;
        return 0;
    }

    public bool m_UI2DMode = false;

    public SpriteRenderer SpRender {
        get {
            if (m_SpriteRenderer == null)
                m_SpriteRenderer = GetComponent<SpriteRenderer>();
            return m_SpriteRenderer;
        }
    }

#if _USE_NGUI
    public UI2DSprite UI2DSprite {
        get {
            if (m_UI2DSprite == null)
                m_UI2DSprite = GetComponent<UI2DSprite>();
            return m_UI2DSprite;
        }
    }
#endif
    public Movement Mover {
        get {
            if (m_Movement == null)
                m_Movement = GetComponent<Movement>();
            return m_Movement;
        }
    }

    public AudioSource Audio {
        get {
            if (m_Audio == null)
                m_Audio = GetComponent<AudioSource>();
            return m_Audio;
        }
    }

    public Fighter Player {
        get {
            if (m_Fighter == null)
                m_Fighter = GetComponent<Fighter>();
            return m_Fighter;
        }
    }

    public PlayerLoader PlayerLoader {
        get {
            if (m_PlayerLoader == null)
                m_PlayerLoader = GetComponent<PlayerLoader>();
            return m_PlayerLoader;
        }
    }
    public ImageAnimation Anim {
        get {
            if (m_Anim == null)
                m_Anim = GetComponent<ImageAnimation>();
            return m_Anim;
        }
    }

    public void VarSet(int idx, float value) {
        VarValue vv;
        if (!m_VarMap.TryGetValue(idx, out vv))
            vv = new VarValue();
        vv.fValue = value;
        m_VarMap[idx] = vv;
    }

    public void VarSet(int idx, int value) {
        VarValue vv;
        if (!m_VarMap.TryGetValue(idx, out vv))
            vv = new VarValue();
        vv.iValue = value;
        m_VarMap[idx] = vv;
    }

    public void PlaySnd(int group, int image) {
       // Debug.LogErrorFormat("播放声音: {0:D}, {1:D}", group, image);
        var source = this.Audio;
        if (source != null) {
            var audioClip = SoundManager.LoadAudio(this.PlayerLoader.PlayerName, group, image);
            if (audioClip != null) {
                source.PlayOneShot(audioClip);
            }
        }
    }

    public int iVal(int idx) {
        VarValue vv;
        if (m_VarMap.TryGetValue(idx, out vv))
            return vv.iValue;
        return 0;
    }

    public float fVal(int idx) {
        VarValue vv;
        if (m_VarMap.TryGetValue(idx, out vv))
            return vv.fValue;
        return 0;
    }

    public void VarAdd(int idx, int add) {
        int v = iVal(idx) + add;
        VarSet(idx, v);
    }

    public void VarAdd(int idx, float add) {
        float v = fVal(idx) + add;
        VarSet(idx, v);
    }

    void OnDestroy() {
        if (m_LuaObjMap != null) {
            var iter = m_LuaObjMap.GetEnumerator();
            while (iter.MoveNext()) {
                var func = iter.Current.Value;
                if (func != null)
                    func.Dispose();
            }
            iter.Dispose();
            m_LuaObjMap.Clear();
        }
    }

    /*
    protected FightAgent Agent {
        get {
            if (m_Agent == null) {
                m_Agent = GetComponent<FightAgent>();
                m_Agent = this.gameObject.AddComponent<FightAgent>();
            }
            return m_Agent;
        }
    }
    */

    /*
    void OnDestroy() {
        if (m_Agent != null) {
            m_Agent.btunloadall();
            m_Agent = null;
        }
    }
    

    void UpdateBehavicTree(bool checkLoop = false) {
        if (m_AgentStatus == behaviac.EBTStatus.BT_RUNNING) {
            var agent = this.Agent;
            if (agent != null)
                m_AgentStatus = agent.btexec();

            return;
        } else if (checkLoop && (m_AgentStatus == behaviac.EBTStatus.BT_SUCCESS)) {
            var agent = this.Agent;
            if (agent != null) {
                //agent.bthotreloaded()
            }
        }
    }

    void InitBehavicTree() {
        if (Agent.CurrentTreeTask == null) {
            var loader = this.PlayerLoader;
            var agent = this.Agent;

            string playerName = loader.PlayerName;
            playerName = Common.GetBehavicPlayerName(playerName);
            bool bRet = agent.btload(playerName);
            if (bRet) {
                m_AgentStatus = behaviac.EBTStatus.BT_RUNNING;
                //agent.btsetcurrent(playerName);
                agent.btreferencetree(playerName);
            }

            UpdateBehavicTree();
        }
        
    }
   

    void PlayBehavicTree(int stateNo) {

        
        var loader = this.PlayerLoader;
        var agent = this.Agent;

        if (agent.CurrentTreeTask != null) {
            // 以后就是一个角色一个行为树，这是为了测试 单树 方便
            string unloadPath = agent.CurrentTreeTask.GetName();
            agent.btunload(unloadPath);

        }

        string playerName = loader.PlayerName;
        playerName = Common.GetBehavicPlayerName(playerName);
        string fileName = string.Format("{0}_Statedef_{1:D}", playerName, stateNo);
        bool bRet = agent.btload(fileName);
        if (bRet) {
            m_AgentStatus = behaviac.EBTStatus.BT_RUNNING;
            agent.btsetcurrent(fileName);
        } else {
            m_AgentStatus = behaviac.EBTStatus.BT_INVALID;
            agent.btunload(fileName);
        }
    }
     */

    private RegisterFighter m_RegisterFighter = null;
    public RegisterFighter registerFighter {
        get {
            if (m_RegisterFighter == null)
                m_RegisterFighter = GetComponent<RegisterFighter>();
            return m_RegisterFighter;
        }
    }

    public int GetFighterID() {
        var reg = this.registerFighter;
        if (reg == null)
            return ImageAnimation._cNoVaildState;
        return reg.ID;
    }


    public void ChangeState(int stateNo, bool resetCtrl = true) {
        // 并不是调用动画，后面改成状态击
        if (stateNo == ImageAnimation._cNoVaildState)
            return;

        m_StateFrameInfo.ChangeState();
        m_StatePlayAnim = false;

        m_PrevStateNo = m_CurrStateNo;
        m_CurrStateNo = stateNo;

        sprpriority(0);

        if (resetCtrl)
            this.m_Ctrl = 1;

        // 执行行为树
       //InitBehavicTree();
    }

    // 这才是播放动画的接口
    public void ChangeAnim(int animNo, bool isLoop = false) {
        if (animNo == ImageAnimation._cNoVaildState)
            return;

        m_StatePlayAnim = true;
        // 其实并不是动画，这里暂时这样，后面改掉。
        var anim = this.Anim;
        anim.PlayerPlayerAni(animNo, isLoop);
    }

    public int StateNo() {
        return m_CurrStateNo;
    }

    public int stateNo {
        get {
            return StateNo();
        }
    }

    public void CtrlSet(int v) {
        m_Ctrl = v;
    }

    public void Ctrl(int v) {
        CtrlSet(v);
    }

    public bool CanCtrl() {
        return m_Ctrl != 0;
    }

    public int ctrl {
        get {
            return m_Ctrl;
        }
    }

    public bool canCtrl {
        get {
            return CanCtrl();
        }
    }

    public float Abs(float v) {
        return Mathf.Abs(v);
    }

    public int Abs(int v) {
        return System.Math.Abs(v);
    }

    public int AnimNO() {
        var anim = this.Anim;
        return anim.CurrActionNo;
    }

    public int AnimElem() {
        var anim = this.Anim;
        int ret = anim.CurFrame + 1;
      //  Debug.LogErrorFormat("AnimElem: {0:D}", ret);
        return ret;
    }

    // 剩余最后一帧数量，0表示就是最后一帧
    public int AnimTime() {
        var anim = this.Anim;
        int ret = anim.AniNodeCount - AnimElem();
        //Debug.LogErrorFormat("AnimTime: {0:D}", ret);
        return ret;
    }

    /// <summary>
    /// 当前相当于参考帧 lookFrame 的差 
    /// </summary>
    /// <param name="lookFrame">参考帧号，从1开始</param>
    /// <returns></returns>
    public int AnimElemTime(int lookFrame) {
        return AnimElem() - lookFrame;
    }

    public float ACos(float v) {
        return Mathf.Acos(v);
    }

    public bool IsAlive() {
        return this.Player.IsAlive;
    }

    // 角度加
    public void AngleAdd(float angle) {

        angle = Mathf.PI / 180 * angle;

        var trans = this.transform;
        var angles = trans.localEulerAngles;
        angles.z += angle;
        trans.localEulerAngles = angles;
    }

    // 角度设置
    public void AngleSet(float angle) {
        angle = Mathf.PI / 180 * angle;

        var trans = this.transform;
        var angles = trans.localEulerAngles;
        angles.z = angle;

        trans.localEulerAngles = angles;
    }

    public void DestroySelf() {
        if (Application.isPlaying)
            Destroy(this.gameObject);
        else
            DestroyImmediate(this.gameObject);
    }

    public bool AnimExist(int actionNo) {
        if (actionNo == ImageAnimation._cNoVaildState)
            return false;
        var anim = this.Anim;
        return anim.ContainsAnim(actionNo);
    }

    public float Asin(float v) {
        return Mathf.Asin(v);
    }

    public float Sin(float angle) {
        return Mathf.Sin(angle);
    }

    public float Atan(float v) {
        return Mathf.Atan(v);
    }

    public int Ceil(float v) {
        return Mathf.CeilToInt(v);
    }

    public int Floor(float v) {
        return Mathf.FloorToInt(v);
    }

    public float Cos(float a) {
        return Mathf.Cos(a);
    }

    public int Facing() {
        return transform.localScale.x < 0? -1 : 1;
    }

    public int Life() {
        var player = this.Player;
        return player.Hp;
    }

    public float Exp(float v) {
        return Mathf.Exp(v);
    }

    public float Log(float exp1, float exp2) {
        return Mathf.Log(exp1, exp2);
    }

    public float Pi() {
        return Mathf.PI;
    }

    public int PrevStateNo() {
        return m_PrevStateNo;
    }

    public void VarRandom(int idx, int range) {
        int v = m_Random.Next(0, range + 1);
        VarSet(idx, v);
    }

    /// <summary>
    /// 在这个状态已经经历了多少帧
    /// </summary>
    /// <returns></returns>
    public int Time() {
        //Debug.LogError(m_StateFrameInfo.stateRunFrameNum);
        return m_StateFrameInfo.stateRunFrameNum;
    }

    public bool TimeMod(int divFrame, int modValue) {
        if (divFrame == 0)
            return false;

        int time = this.Time();
        return (time % divFrame) == modValue;
    }

    /// <summary>
    /// 返回一个包含在0到999之间的随机整数
    /// </summary>
    /// <returns></returns>
    public int Random() {
        return m_Random.Next(0, 1000);
    }

    public void sprpriority(int no) {
#if _USE_NGUI
        if (m_UI2DMode) {
            var uiSp = this.UI2DSprite;
            uiSp.depth = no;
        } else
#endif
        {
            var sp = this.SpRender;
            sp.sortingOrder = no;
        }
    }

    public Explod CreateExplod(ExplodCreator creator) {
        if (creator == null)
            return null;
        Explod ret = Common.CreateExplod(this.PlayerLoader.PlayerName, creator);
        if (creator.postype == TProjPostype.p1)
            creator.bindID = this.registerFighter.ID;
        return ret;
    }

    public bool IsFlipX {
        get {
            // var sp = this.SpRender;
            //  return sp.flipX;
            var scale = this.transform.localScale;
            return scale.x < 0;
        }
    }


    public static float _cPosPer = 100.0f;
    public float PosY() {
        var trans = this.transform;
        var pt = trans.position;
        return -pt.y * _cPosPer;
    }

    public float PosX() {
        var trans = this.transform;
        var pt = trans.position;
        float dir = IsFlipX ? -1 : 1;
        return pt.x * _cPosPer * dir;
    }

    public void PosAdd(float x, float y = 0) {
        var trans = this.transform;
        var pt = trans.position;
        float dir = IsFlipX ? -1 : 1;
        pt.x += x/ _cPosPer * dir; pt.y += -y/ _cPosPer;
        trans.position = pt;
    }

    public void PosSetY(float y) {
        var trans = this.transform;
        var pt = trans.position;
        pt.y = -y / _cPosPer;
        trans.position = pt;
    }

    public void StatetypeSet(TStandType statetype) {
        this.type = statetype;
    }

    public void StatetypeSet(TPhysicType physics) {
        this.physics = physics;
    }

    public void StatetypeSet(TMoveType moveType) {
        this.movetype = moveType;
    }

    public void CreateProjectitle(string name) {
        if (string.IsNullOrEmpty(name))
            return;
        string fileName = StringHelper.Format("resources/BehavicTree/{0}.prefab", name);
        ResourceMgr.Instance.CreateGameObject(fileName);
    }

    public void StatetypeSet(TStandType statetype, TPhysicType physics, TMoveType moveType) {
        StatetypeSet(statetype);
        StatetypeSet(physics);
        StatetypeSet(moveType);
    }

    public void StatetypeSet(TStandType statetype, TPhysicType physics) {
        StatetypeSet(statetype);
        StatetypeSet(physics);
    }

    public void StatetypeSet(TStandType statetype, TMoveType moveType) {
        StatetypeSet(statetype);
        StatetypeSet(moveType);
    }

    public void StatetypeSet(TPhysicType physics, TMoveType moveType) {
        StatetypeSet(physics);
        StatetypeSet(moveType);
    }

    public void VelXSet(float x) {
        float y = VelY();
        velset(x, y);
    }

    public void VelYSet(float y) {
        float x = VelX();
        velset(x, y);
    }

    public void VelMul(float x, float y) {
        var v = this.Mover.Vec;
        v.x *= x;
        v.y *= y;
        this.Mover.Vec = v;
    }

    public void VelXMul(float x) {
        VelMul(x, 1f);
    }

    public void VelYMul(float y) {
        VelMul(1f, y);
    }

    public bool KeyPress(KeyCode key) {
        return Input.GetKey(key);
    }

    public bool KeyDown(KeyCode key) {
        return Input.GetKeyDown(key);
    }

    public bool KeyUp(KeyCode key) {
        return Input.GetKeyUp(key);
    }

    private System.Random m_Random = new System.Random();
    private AudioSource m_Audio = null;
}

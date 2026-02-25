using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerAnimShow : EditorWindow {

    //private PreviewRenderUtility m_Preview = null;
    private SpriteRenderer m_Target = null;
    private ImageAnimation m_Anim = null;
    private bool m_UnityIsRun = false;
    private string[] m_PlayerNames = null;
    private int m_SelPlayerName = -1;
    private bool m_IsInitPlayerNames = false;
    private PlayerLoader m_PlayerLoader;
    private CnsCommander m_Cns = null;
    private int m_PalletSelect = -1;
    private string[] m_Anims = null;
    private int[] m_AnimIds = null;
    private int m_SelAnimIdx = -1;
    private int m_AnimNo = ImageAnimation._cNoVaildState;
    private string[] m_States = null;
    //private static PlayerAnimShow m_Instance = null;

    /*
    public static void Open(string animFileName, string playerName) {
        PlayerAnimShow wnd = EditorWindow.GetWindow<PlayerAnimShow>("动画查看器");
        wnd.Init(animFileName, playerName);
    }
    */

    [MenuItem("Tools/角色动画")]
    public static void Menu() {
        var editorAsm = typeof(Editor).Assembly;
        var inspWndType = editorAsm.GetType("UnityEditor.InspectorWindow");

        var wnd = EditorWindow.GetWindow<PlayerAnimShow>("动画查看器", inspWndType);
        wnd.Init();
    }

    void RefreshAnimIdx() {
        if (m_AnimIds == null || m_AnimIds.Length <= 0 || m_AnimNo == ImageAnimation._cNoVaildState)
            m_SelAnimIdx = -1;
        else {
            for (int i = 0; i < m_AnimIds.Length; ++i) {
                if (m_AnimNo == m_AnimIds[i]) {
                    m_SelAnimIdx = i;
                    break;
                }
            }
        }
    }

    void OnAppQuit() {
        // 查找界面
        var wnds = Resources.FindObjectsOfTypeAll<PlayerAnimShow>();
        if (wnds != null && wnds.Length > 0) {
            for (int i = 0; i < wnds.Length; ++i) {
                var wnd = wnds[i];
                if (wnd == null || wnd != this)
                    continue;
                wnd.Close();
                break;
            }
        }
    }

    void OnEditorUpdate() {
        CheckAppQuit();
    }

    void CheckAppQuit() {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            OnAppQuit();
    }

    void OnPlayerModeChged() {
        CheckAppQuit();
    }

    void Init() {
        m_UnityIsRun = EditorApplication.isPlaying;
        if (!EditorApplication.isPlaying) {
            EditorApplication.isPlaying = true;
#if UNITY_2019
            EditorApplication.playmodeStateChanged += OnPlayerModeChged;
#endif
            EditorApplication.update += OnEditorUpdate;
        }
    }

    void CreateTarget(string playerName) {
        if (string.IsNullOrEmpty(playerName))
            return;

        GameObject gameObj = Common.CreateBehaviourPlayer(playerName, true);
        m_Target = gameObj.GetComponent<SpriteRenderer>();
        m_Anim = gameObj.GetComponent<ImageAnimation>();
        m_PlayerLoader = gameObj.GetComponent<PlayerLoader>();
        m_Cns = gameObj.GetComponent<CnsCommander>();

        var actionMap = m_Anim.ActionMap;
        if (actionMap != null) {
            m_AnimIds = actionMap.Keys.ToArray();
            if (m_AnimIds != null && m_AnimIds.Length > 0) {
                m_Anims = new string[m_AnimIds.Length];
                for (int i = 0; i < m_AnimIds.Length; ++i) {
                    m_Anims[i] = m_AnimIds[i].ToString();
                }
            }
        }

        Selection.activeGameObject = gameObj;
        //Selection.activeTransform = gameObj.transform;

        NodeCanvas.BehaviourTrees.BehaviourTreeOwner bbOwer = gameObj.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>();
        NodeCanvas.Framework.Blackboard bOwner = gameObj.GetComponent<NodeCanvas.Framework.Blackboard>();

        if (bbOwer != null && bOwner != null) {
            m_States = null;
            List<int> bList;
            var bVar = bOwner.GetVariable<List<int>>("StateList");
            int varCnt = 0;
            if (bVar != null) {
                bList = bVar.GetValue();
                if (bList != null && bList.Count > 0) {
                    varCnt += bList.Count;
                }
            }

            if (bbOwer.graph != null && bbOwer.graph.blackboard != null && bbOwer.graph.blackboard.variables != null) {
                NodeCanvas.Framework.Variable vv;
                if (bbOwer.graph.blackboard.variables.TryGetValue("StateList", out vv) && vv != null) {
                    bList = vv.value as List<int>;
                    if (bList != null && bList.Count > 0)
                        varCnt += bList.Count;
                }
            }

            if (varCnt > 0) {
                m_States = new string[varCnt];
                int varIdx = 0;

                bVar = bOwner.GetVariable<List<int>>("StateList");
                if (bVar != null) {
                    bList = bVar.GetValue();
                    if (bList != null) {
                        for (int i = 0; i < bList.Count; ++i) {
                            m_States[varIdx++] = bList[i].ToString();
                        }
                    }
                }

                if (bbOwer.graph != null && bbOwer.graph.blackboard != null && bbOwer.graph.blackboard.variables != null) {
                    NodeCanvas.Framework.Variable vv;
                    if (bbOwer.graph.blackboard.variables.TryGetValue("StateList", out vv) && vv != null) {
                        bList = vv.value as List<int>;
                        if (bList != null && bList.Count > 0) {
                            for (int i = 0; i < bList.Count; ++i) {
                                m_States[varIdx++] = bList[i].ToString();
                            }
                        }
                    }
                }
            }

        } else {
            const string stateDir = "assets/resources/BehavicTree";
            if (Directory.Exists(stateDir)) {
                string statePlayerName = Common.GetBehavicPlayerName(playerName);
                m_States = Directory.GetFiles(stateDir, string.Format("{0}_*.xml", statePlayerName), SearchOption.TopDirectoryOnly);
            } else
                m_States = null;
        }
        
    }

    /*
    void Init(string animFileName, string playerName) {
        if (!File.Exists(animFileName))
            return;
        //   m_Preview = new PreviewRenderUtility();

       // m_UnityIsRun = EditorApplication.isPlaying;
        if (!EditorApplication.isPlaying) {
            EditorApplication.isPlaying = true;
        }

        GameObject gameObj = new GameObject("角色");
        m_Target = gameObj.AddComponent<SpriteRenderer>();
        m_Anim = gameObj.AddComponent<ImageAnimation>();
        gameObj.AddComponent<Animation>();
        var loader = gameObj.AddComponent<PlayerLoader>();
        loader.m_PlayerName = playerName;
        FileStream stream = new FileStream(animFileName, FileMode.Open, FileAccess.Read);
        try {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            m_Anim.Load(buffer);
        } finally {
            stream.Close();
            stream.Dispose();
        }
        string dir = Path.GetDirectoryName(animFileName);
        string[] mats = Directory.GetFiles(dir, "*.act.mat");
        if (mats != null && mats.Length > 0) {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(mats[0]);
            if (mat != null)
                m_Target.sharedMaterial = mat;
        }
    }
    */

    void CheckEditorAppUpdate() {
        var list = EditorApplication.update != null ? EditorApplication.update.GetInvocationList() : null;
        bool isFound = false;
        if (list != null) {
            for (int i = 0; i < list.Length; ++i) {
                if (list[i] != null && list[i].Method != null && list[i].Method.Name == "OnEditorUpdate") {
                    isFound = true;
                    break;
                }
            }
        }

        if (!isFound) {
            EditorApplication.update += OnEditorUpdate;
        }
#if UNITY_2019
        var stateEvt = EditorApplication.playmodeStateChanged;
        var ll = stateEvt != null ? stateEvt.GetInvocationList() : null;
        isFound = false;
        if (list != null) {
            for (int i = 0; i < ll.Length; ++i) {
                if (ll[i] != null && ll[i].Method != null && ll[i].Method.Name == "OnPlayerModeChged") {
                    isFound = true;
                    break;
                }
            }
        }

        if (!isFound) {
            EditorApplication.playmodeStateChanged += OnPlayerModeChged;
        }
#endif
    }

    void InitPlayerNames() {

        if (m_IsInitPlayerNames)
            return;

        m_PlayerNames = null;

        const string root = "assets/resources/character";
        string[] dirs = Directory.GetDirectories(root, "*.*", SearchOption.TopDirectoryOnly);
        if (dirs != null && dirs.Length > 0) {
            List<string> lst = null;
            for (int i = 0; i < dirs.Length; ++i) {
                string dir = dirs[i];
                if (string.IsNullOrEmpty(dir))
                    continue;
                string name = Path.GetFileNameWithoutExtension(dir);
                if (string.IsNullOrEmpty(name))
                    continue;
                if (lst == null)
                    lst = new List<string>();
                lst.Add(name);
            }
            if (lst != null && lst.Count > 0)
                m_PlayerNames = lst.ToArray();
        }

        m_IsInitPlayerNames = true;
    }

    void OnChgPlayer() {
        if (m_PlayerNames == null || m_SelPlayerName < 0 || m_SelPlayerName >= m_PlayerNames.Length)
            return;
        string playerName = m_PlayerNames[m_SelPlayerName];
        if (string.IsNullOrEmpty(playerName))
            return;
        DestroyTarget();
        CreateTarget(playerName);
    }

    void DrawSelectPlayer() {
        if (m_PlayerNames == null)
            return;
        var newIdx = EditorGUILayout.Popup("选择角色", m_SelPlayerName, m_PlayerNames);
        if (m_SelPlayerName != newIdx) {
            m_SelPlayerName = newIdx;
            OnChgPlayer();
        }
    }

    void DrawPlayerPallet() {
        if (m_PlayerLoader != null) {
            var playerData = m_PlayerLoader.PlayerData;
            if (playerData.palletLocalPaths != null && playerData.palletLocalPaths.Length > 0) {
                var newIdx = EditorGUILayout.Popup("选择调色板", m_PalletSelect, playerData.palletLocalPaths);
                if (newIdx != m_PalletSelect) {
                    m_PalletSelect = newIdx;
                    m_PlayerLoader.SwitchPallet(m_PalletSelect);
                }
            }
            
        }
    }

    void DrawPlayerAction() {
        if (m_Anim != null && m_Anims != null && m_Anims.Length > 0) {
           var newIdx = EditorGUILayout.Popup("选择动作", m_SelAnimIdx, m_Anims);
            if (m_SelAnimIdx != newIdx) {
                m_SelAnimIdx = newIdx;
                m_AnimNo = m_AnimIds[m_SelAnimIdx];

                m_Anim.PlayerPlayerAni(m_AnimNo);
            }

            newIdx = EditorGUILayout.IntSlider("动画序列", m_SelAnimIdx  + 1, 1, m_AnimIds.Length) - 1;
            newIdx = System.Math.Min(System.Math.Max(0, newIdx), m_AnimIds.Length - 1);
            if (newIdx != m_SelAnimIdx) {
                m_SelAnimIdx = newIdx;

                m_AnimNo = m_AnimIds[m_SelAnimIdx];
                m_Anim.PlayerPlayerAni(m_AnimNo);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("前一个动作")) {
                if (m_SelAnimIdx == 0) {
                    m_SelAnimIdx = m_AnimIds.Length - 1;

                } else
                    --m_SelAnimIdx;

                m_AnimNo = m_AnimIds[m_SelAnimIdx];
                m_Anim.PlayerPlayerAni(m_AnimNo);
            }
            if (GUILayout.Button("后一个动作")) {
                if (m_SelAnimIdx + 1 >= m_AnimIds.Length)
                    m_SelAnimIdx = 0;
                else
                    ++m_SelAnimIdx;
                m_AnimNo = m_AnimIds[m_SelAnimIdx];
                m_Anim.PlayerPlayerAni(m_AnimNo);
            }
            EditorGUILayout.EndHorizontal();

        }
    }

    void OnGUI() {

        CheckEditorAppUpdate();
        InitPlayerNames();

        DrawSelectPlayer();

        if (m_Target == null)
            return;

        if (m_Anim != null) {
            if (!m_Anim.HasAniData) {
                var animMap = m_Anim.ActionMap;
                if (animMap != null) {
                    if (animMap.ContainsKey(0)) {
                        m_Anim.PlayerPlayerAni(0);
                        m_AnimNo = 0;
                        RefreshAnimIdx();
                    }
                }
            }
        }

        if (m_PlayerLoader != null) {
            if (m_PalletSelect < 0) {
                m_PalletSelect = 0;
                m_PlayerLoader.SwitchPallet(m_PalletSelect);
            }
        }

        DrawPlayerPallet();
        DrawPlayerAction();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawMaterial();

        DrawStates();

        /*
        var sprite = m_Target.sprite;
        if (sprite == null || sprite.texture == null)
            return;

        Rect r = new Rect((Screen.width - sprite.rect.width)/2f, (Screen.height - sprite.rect.height)/2f, sprite.rect.width, sprite.rect.height);

        GUI.DrawTextureWithTexCoords(r, sprite.texture, new Rect(r.xMin / sprite.texture.width, r.yMin / sprite.texture.height, r.width / sprite.texture.width, r.height / sprite.texture.height));
        */
        //m_Preview.BeginPreview(new Rect(0, 100, Screen.width, Screen.height), GUIStyle.none);

        // m_Preview.EndPreview();
    }

    void DrawMaterial() {
        if (m_Target == null || m_Cns == null)
            return;


        bool isFlipX = m_Cns.IsFlipX;
        bool newFlipX = EditorGUILayout.Toggle("是否翻转", isFlipX);
        if (newFlipX != isFlipX) {
            var scale = m_Target.transform.localScale;
            scale.x = -scale.x;
            m_Target.transform.localScale = scale;
        }

        bool isMatRGBA = m_Target.sharedMaterial.IsKeywordEnabled("_RGB_A");
        bool newMatRGBA = EditorGUILayout.Toggle("材质选项 _RGB_A ", isMatRGBA);
        if (newMatRGBA != isMatRGBA) {
            if (newMatRGBA) {
                m_Target.sharedMaterial.EnableKeyword("_RGB_A");
                //m_Target.sharedMaterial.DisableKeyword("__");
            } else {
                m_Target.sharedMaterial.DisableKeyword("_RGB_A");
                //m_Target.sharedMaterial.EnableKeyword("__");
            }
        }
        EditorGUILayout.Space();
    }

    void DrawStates() {
        if (m_States == null || m_States.Length <= 0)
            return;

        if (m_Cns != null) {

            if (m_Anim  != null && m_Anim.FrameList != null) {
                EditorGUILayout.LabelField("type", m_Cns.type.ToString());
                EditorGUILayout.LabelField("physicType", m_Cns.physics.ToString());
                EditorGUILayout.LabelField("moveType", m_Cns.movetype.ToString());
                EditorGUILayout.LabelField("StateNo", m_Cns.stateNo.ToString());
                EditorGUILayout.LabelField("AnimNo", m_Cns.AnimNO().ToString());
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("位置：", m_Target.transform.position.ToString());
                EditorGUILayout.LabelField("速度：", string.Format("({0},{1})", m_Cns.VelX().ToString(), m_Cns.VelY().ToString()));
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("当前动画帧数", m_Anim.FrameList.Count.ToString());
                EditorGUILayout.LabelField("当前帧", m_Anim.CurFrame.ToString());
                EditorGUILayout.Space();
            }
            EditorGUILayout.LabelField("Time()", m_Cns.Time().ToString());
            EditorGUILayout.LabelField("AnimTime()", m_Cns.AnimTime().ToString());
            EditorGUILayout.LabelField("AnimElem()", m_Cns.AnimElem().ToString());
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            Repaint();
        }
       
        NodeCanvas.BehaviourTrees.BehaviourTreeOwner bOwner = m_Target.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>();
        if (bOwner  != null && GUILayout.Button("显示行为树")) {
            NodeCanvas.Editor.GraphEditor.OpenWindow(bOwner);
        }

        const int cColNum = 5;

        int rowCnt = Mathf.CeilToInt(((float)m_States.Length) / ((float)cColNum));
        for (int r = 0; r < rowCnt; ++r) {
            bool isEnd = false;
            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < cColNum; ++c) {
                int idx = r * cColNum + c;
                if (idx >= m_States.Length) {
                    isEnd = true;
                    break;
                }

                string name = Path.GetFileNameWithoutExtension(m_States[idx]);
                int j = name.LastIndexOf('_');
                if (j >= 0) {
                    name = name.Substring(j + 1);
                }

                if (GUILayout.Button(name)) {
                    int stateNo;
                    if (int.TryParse(name, out stateNo)) {
                        m_Cns.ChangeState(stateNo);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (isEnd)
                break;
        }
    }

    void DestroyTarget() {
        if (m_Target != null) {
            if (Application.isPlaying)
                GameObject.Destroy(m_Target.gameObject);
            else
                GameObject.DestroyImmediate(m_Target.gameObject);
            m_Target = null;
            m_Anim = null;
            m_PlayerLoader = null;
            m_Anims = null;
        }

        m_PalletSelect = -1;
        m_States = null;
    }

    void OnDestroy() {
        // if (m_Preview != null) {
        //     m_Preview.Cleanup();
        //     m_Preview = null;
        // }

        EditorApplication.update -= OnEditorUpdate;
#if UNITY_2019
        EditorApplication.playmodeStateChanged -= OnPlayerModeChged;
#endif


        DestroyTarget();

        if (!m_UnityIsRun && EditorApplication.isPlaying) {
            EditorApplication.isPlaying = false;
        }
    }
}

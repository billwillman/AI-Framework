using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using NsLib.ResMgr;
using Utils;

[System.Serializable]
public struct PlayerLoaderData
{
    public string aniLocalPath;
    public string[] palletLocalPaths;
   // public Dictionary<KeyValuePair<int, int>, string> extPalletPath; 
}

public static class SoundManager
{
    // 声音
    private static Dictionary<string, Dictionary<KeyValuePair<int, int>, AudioClip>> m_AudioMap = new Dictionary<string, Dictionary<KeyValuePair<int, int>, AudioClip>>();

    public static AudioClip LoadAudio(string playerName, int group, int image) {
        AudioClip ret = null;
        Dictionary<KeyValuePair<int, int>, AudioClip> sndMap;
        if (m_AudioMap.TryGetValue(playerName, out sndMap) && sndMap != null) {
            if (sndMap.TryGetValue(new KeyValuePair<int, int>(group, image), out ret) && ret != null)
                return ret;
        } else
            sndMap = null;

        string fileName = string.Format("resources/character/{0}/sound/{1:D}_{2:D}.wav", playerName, group, image);
        ret = ResourceMgr.Instance.LoadAudioClip(fileName, ResourceCacheType.rctRefAdd);
        if (ret != null) {
            if (sndMap == null) {
                sndMap = new Dictionary<KeyValuePair<int, int>, AudioClip>();
                m_AudioMap[playerName] = sndMap;
            }

            var key = new KeyValuePair<int, int>(group, image);
            sndMap[key] = ret;
        }

        return ret;
    }

    public static void ClearAll() {
        var iIter = m_AudioMap.GetEnumerator();
        while (iIter.MoveNext()) {
            var iiIter = iIter.Current.Value.GetEnumerator();
            while (iiIter.MoveNext()) {
                ResourceMgr.Instance.DestroyObject(iiIter.Current.Value);
            }
            iiIter.Dispose();
            iIter.Current.Value.Clear();
        }
        iIter.Dispose();

        m_AudioMap.Clear();
    }
}


//[RequireComponent(typeof(SpriteRenderer))]
public class PlayerLoader
#if _USE_NGUI
    : NGUIResLoader
#else
    : BaseResLoaderAsyncMono
#endif
{
    private struct PlayerImage
    {
        public int group;
        public Sprite[] sprites;

        public bool IsVaid {
            get {
                return (group != ImageAnimation._cNoVaildState) && (sprites != null) && (sprites.Length > 0);
            }
        }

        public void Reset() {
            if (sprites != null) {
                ResourceMgr.Instance.DestroySprites(sprites);
                sprites = null;
            }
            group = ImageAnimation._cNoVaildState;
        }

        public void Init() {
            group = ImageAnimation._cNoVaildState;
            sprites = null;
        }

        public bool Load(int group, string playerName) {
            if (group == ImageAnimation._cNoVaildState || string.IsNullOrEmpty(playerName))
                return false;
            if (this.group == group)
                return true;
            this.group = group;
            if (this.sprites != null) {
                ResourceMgr.Instance.DestroySprites(this.sprites);
                this.sprites = null;
            }
            string fileName = string.Format("resources/character/{0}/@{1:D}/{1:D}.png", playerName, group);
            this.sprites = ResourceMgr.Instance.LoadSprites(fileName);
            bool ret = this.IsVaid;
            if (!ret) {
                this.group = ImageAnimation._cNoVaildState;
            }
            return ret;
        }
    }

    // 切换剪切板
    public void SwitchPallet(int palIdx) {
        if (string.IsNullOrEmpty(m_PlayerName) || m_PlayerData.palletLocalPaths == null || m_PlayerData.palletLocalPaths.Length <= 0 || palIdx < 0 || palIdx >= m_PlayerData.palletLocalPaths.Length)
            return;
        if (m_PalIdx == palIdx)
            return;
        var pal = m_PlayerData.palletLocalPaths[palIdx];
        if (string.IsNullOrEmpty(pal))
            return;
#if _USE_NGUI
        if (!m_IsUIMode)
#endif
        {
            var target = this.SpriteRender;
            if (target != null) {

                string palFileName = string.Format("assets/resources/character/{0}/{1}", m_PlayerName, pal);

                if (LoadMaterial(target, palFileName))
                    m_PalIdx = palIdx;
            }
        }
#if _USE_NGUI
        else {
            var uiTarget = this.UISprite;
            if (uiTarget != null) {
                string palFileName = string.Format("assets/resources/character/{0}/{1}", m_PlayerName, pal);
                if (LoadMaterial(uiTarget, palFileName)) {
                    m_PalIdx = palIdx;
                }
            }
        }
#endif

    }

    public void AttachPlayer(string playerName) {
        if (string.Compare(playerName, m_PlayerName) == 0)
            return;

        if (m_Anim != null) {
            m_Anim.StopAndReset();
        }
        m_PlayerImage.Reset();
        if (m_SpriteRenderer != null) {
            m_SpriteRenderer.sprite = null;
        }
#if _USE_NGUI
        if (m_UI2DSprite != null) {
            m_UI2DSprite.sprite2D = null;
        }
#endif
       // m_Mat = null;
        m_PalIdx = -1;

        m_PlayerName = playerName;

        if (string.IsNullOrEmpty(m_PlayerName))
            return;

        LoadPlayerData();
        if (!string.IsNullOrEmpty(m_PlayerData.aniLocalPath)) {
            var anim = this.Amim;
            if (anim != null) {
                string animFileName = string.Format("assets/resources/character/{0}/{1}", m_PlayerName, m_PlayerData.aniLocalPath);
                byte[] buffer = ResourceMgr.Instance.LoadBytes(animFileName);
                anim.Load(buffer);
            }
        }
    }

    public PlayerLoaderData PlayerData {
        get {
            return m_PlayerData;
        }
    }

    public string PlayerName {
        get {
            return m_PlayerName;
        }
    }

    private void LoadPlayerData() {
        if (string.IsNullOrEmpty(m_PlayerName))
            return;
        string fileName = string.Format("assets/resources/character/{0}/{0}.bytes", m_PlayerName);
        var buffer = ResourceMgr.Instance.LoadBytes(fileName);
        if (buffer == null || buffer.Length <= 0)
            return;
        MemoryStream stream = new MemoryStream(buffer);
        try {
            BinaryFormatter reader = new BinaryFormatter();
            m_PlayerData = (PlayerLoaderData)reader.Deserialize(stream);
        } finally {
            stream.Dispose();
        }
    }

    public string DesginPlayerName = string.Empty;

    private ImageAnimation m_Anim = null;
    private string m_PlayerName = string.Empty;
    private AnimationClip m_Clip = null;
    private Animation m_Ctl = null;
    private SpriteRenderer m_SpriteRenderer = null;
#if _USE_NGUI
    private UI2DSprite m_UI2DSprite = null;
#endif
    private PlayerImage m_PlayerImage;
    private PlayerLoaderData m_PlayerData;
    private int m_PalIdx = -1;
    //private Material m_Mat = null;

    protected override void OnDestroy() {
        m_PlayerImage.Reset();
        base.OnDestroy();
    }

    void Awake() {
        m_PlayerImage.Init();
        if (!string.IsNullOrEmpty(DesginPlayerName))
            AttachPlayer(DesginPlayerName);
    }

    protected SpriteRenderer SpriteRender {
        get {
            if (m_SpriteRenderer == null)
                m_SpriteRenderer = GetComponent<SpriteRenderer>();
            return m_SpriteRenderer;
        }
    }
#if _USE_NGUI
    protected UI2DSprite UISprite {
        get {
            if (m_UI2DSprite == null)
                m_UI2DSprite = GetComponent<UI2DSprite>();
            return m_UI2DSprite;
        }
    }
#endif
    protected Animation Ctl {
        get {
            if (m_Ctl == null)
                m_Ctl = GetComponent<Animation>();
            return m_Ctl;
        }
    }

    protected ImageAnimation Amim {
        get {
            if (m_Anim == null)
                m_Anim = GetComponent<ImageAnimation>();
            return m_Anim;
        }
    }

    // 动画帧变化通知
    void OnImageAnimationFrame(ImageAnimation target) {
        if (target == null)
            return;
        RefreshCurFrame(target);
    }

    void OnImageAnimationInit(ImageAnimation target) {
        if (target == null)
            return;
        RefreshCurFrame(target);
    }

    void UpdateRenderer(ImageAnimateNode node, ImageAnimation imageAni) {
        SpriteRenderer r = null;
#if _USE_NGUI
        UI2DSprite uiR = null;
        if (m_IsUIMode) {
            uiR = this.UISprite;
            if (uiR == null)
                return;
        } else 
#endif
        {
            r = this.SpriteRender;
            if (r == null)
                return;
        }
        // 加载Sprite
        if (m_PlayerImage.Load(node.frameGroup, m_PlayerName)) {
            var sprites = m_PlayerImage.sprites;
            if (node.frameIndex >= sprites.Length)
                return;
            if (r != null) {
                r.sprite = sprites[node.frameIndex];
                if (r.sprite != null) {
                    var flip = node.flipTag;
                    switch (flip) {
                        case ActionFlip.afH:
                            r.flipX = true;
                            break;
                        case ActionFlip.afV:
                            r.flipY = true;
                            break;
                        case ActionFlip.afHV:
                            r.flipX = true;
                            r.flipY = true;
                            break;
                        default:
                            r.flipX = false;
                            r.flipY = false;
                            break;
                    }
                }
            } else {
#if _USE_NGUI
                if (uiR != null) {
                    uiR.sprite2D = sprites[node.frameIndex];
                    if (uiR.sprite2D != null) {
                        var flip = node.flipTag;
                        switch (flip) {
                            case ActionFlip.afH:
                                uiR.flip = UIBasicSprite.Flip.Horizontally;
                                break;
                            case ActionFlip.afV:
                                uiR.flip = UIBasicSprite.Flip.Vertically;
                                break;
                            case ActionFlip.afHV:
                                uiR.flip = UIBasicSprite.Flip.Both;
                                break;
                            default:
                                uiR.flip = UIBasicSprite.Flip.Nothing;
                                break;
                        }
                    }
                }
#endif
            }
        } else {
            if (r != null)
                r.sprite = null;
#if _USE_NGUI
            if (uiR != null)
                uiR.sprite2D = null;
#endif
        }
    }

    public bool m_IsUIMode = false;

    internal void InteralRefreshCurFrame(ImageAnimation target) {
#if _USE_NGUI
        if (m_IsUIMode) {
            var uiR = this.UISprite;
            if (uiR == null)
                return;
        } else
#endif
        {
            SpriteRenderer r = this.SpriteRender;
            if (r == null)
                return;
        }
        ImageAnimateNode node;
        if (!target.CurrAnimNode(out node))
            return;
        UpdateRenderer(node, target);
    }

    protected void RefreshCurFrame(ImageAnimation target) {
        InteralRefreshCurFrame(target);
    }

    // 动画帧结束通知
    void OnImageAnimationEndFrame(ImageAnimation target) {

    }

    void OnAnimationLoad(int actionNo) {
        if (string.IsNullOrEmpty(m_PlayerName))
            return;

        var anim = this.Amim;
        if (anim == null)
            return;
        var actionMap = anim.ActionMap;
        if (actionMap == null)
            return;

        string animFileName = StringHelper.Format("assets/resources/character/{0}/anim/{1:D}.anim", m_PlayerName, actionNo);
        if (LoadAnimationClip(ref m_Clip, animFileName)) {
            var ctl = this.Ctl;
            if (ctl != null) {
                bool isFound = false;
                var iter = ctl.GetEnumerator();
                while (iter.MoveNext()) {
                    AnimationClip c = iter.Current as AnimationClip;
                    if (c != null) {
                        if (c == m_Clip) {
                            isFound = true;
                            break;
                        }
                    }
                }

                if (!isFound) {
#if UNITY_EDITOR
                    if (Application.isPlaying) {
                        ctl.AddClip(m_Clip, m_Clip.name);
                    } else {
                        AnimationClip[] clips = new AnimationClip[1];
                        clips[0] = m_Clip;
                        UnityEditor.AnimationUtility.SetAnimationClips(ctl, clips);
                    }
#else
			       ctl.AddClip(m_Clip, clip.name);
#endif
                }

                ctl.clip = m_Clip;

            }
        }
    }
}

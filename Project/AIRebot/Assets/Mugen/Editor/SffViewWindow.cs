using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using UnityEngine;
using Mugen;

public class InputDialogWindow : EditorWindow
{
    private string m_Message = string.Empty;
    private string m_Input = string.Empty;
    private string m_Text = string.Empty;
    private Action<InputDialogWindow> m_OkEvt = null;

    void OnGUI() {
        m_Input = EditorGUILayout.TextField(m_Message, m_Input);
        if (GUILayout.Button("确定")) {
            m_Text = m_Input;
            if (m_OkEvt != null)
                m_OkEvt(this);
            this.Close();
        }
    }

    public string InputText {
        get {
            return m_Text;
        }
    }

    void Init(string title, string message, Action<InputDialogWindow> okEvt) {
        this.title = title;
        this.m_Message = message;
        this.m_OkEvt = okEvt;
    }

    public static InputDialogWindow CreateWindow(string title, string message, Action<InputDialogWindow> okEvt = null) {
        var wnd = EditorWindow.GetWindow<InputDialogWindow>();
        wnd.Init(title, message, okEvt);
        wnd.ShowModalUtility();
        return wnd;
    }
}

class ViewItem
{
    public string path;
    public string name;
	public string[] GlobalpalletLocalPaths;
    private string m_SffLocalFileName;
    public string snd;

    public ViewItem(string path)
    {
        this.path = path;
        this.name = Path.GetFileNameWithoutExtension(path);
        if (this.name.StartsWith("@"))
        {
            this.name = this.name.Substring(1);
        }

        InitGlobalPalletLocalPaths();
    }

    private void InitGlobalPalletLocalPaths() {
        string parentPath = System.IO.Path.GetDirectoryName(path);
        string[] defFiles = Directory.GetFiles(parentPath, "*.def.txt", SearchOption.AllDirectories);
        if (defFiles != null && defFiles.Length > 0) {
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(defFiles[0]);
            if (asset != null) {
                byte[] buffer = asset.bytes;
                string text = System.Text.Encoding.UTF8.GetString(buffer);
                PlayerCfgLoader loader = new PlayerCfgLoader(text);
                var cfgFiles = loader.files;
                if (cfgFiles != null) {
                    if (cfgFiles.HasPal)
                        this.GlobalpalletLocalPaths = cfgFiles.ToPalLocalPaths;
                    this.m_SffLocalFileName = cfgFiles.sprite;
                    this.snd = cfgFiles.sound;
                }

                Caching.ClearCache();
            }
        }
    }

    public List<SffTexture> GetTextures()
    {
        string parentPath = System.IO.Path.GetDirectoryName(path);
        string fileName = string.Format("{0}/{1}.bytes", parentPath, this.m_SffLocalFileName);
        FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        try
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            SffLoader loader = new SffLoader(buffer);
            return loader.GetTextures();
        }
        finally
        {
            stream.Close();
            stream.Dispose();
        }
    }
}

public class SffViewWindow : EditorWindow
{
    [MenuItem("Tools/Sff查看器")]
    public static void Open() {
        // Rect r = new Rect((Screen.width - 1920) / 2, (Screen.height - 800) / 2, 1920, 800);

        SffViewWindow wnd = EditorWindow.GetWindow<SffViewWindow>("Sff查看器", typeof(SceneView));

        // SffViewWindow wnd = EditorWindow.GetWindowWithRect<SffViewWindow>(r, false, "Sff查看器", true);
        // wnd.maxSize = new Vector2(Screen.width, Screen.height);
        wnd.Init();
    }

    private void OnDestroy() {
        ClearTextures();
    }

    void ClearTextures() {
        if (m_ItemSelectTexs != null) {
            for (int i = 0; i < m_ItemSelectTexs.Length; ++i) {
                var tex = m_ItemSelectTexs[i];
                if (tex != null)
                    GameObject.DestroyImmediate(tex);
            }
        }
        m_ItemSelectTexs = null;
    }

    private Vector2 m_ScrollViewPos = Vector2.zero;
    private List<ViewItem> m_Items = new List<ViewItem>();
    private int m_ItemSelect = -1;
    private Texture[] m_ItemSelectTexs = null;
    private int m_PalletSelIndex = -1;

    void OnItemSelectChanged() {

        List<SffTexture> list = null;
        if (m_ItemSelect >= 0) {
            var item = m_Items[m_ItemSelect];
            list = item.GetTextures();

            Dictionary<KeyValuePair<short, short>, PalletLoader> localLoaderMap = new Dictionary<KeyValuePair<short, short>, PalletLoader>();
            for (int i = 0; i < list.Count; ++i) {
                var sffTex = list[i];
                if (!sffTex.UseGlobalPallet) {
                    PalletLoader localPalletLoader;
                    if (sffTex.UseLinkPallet) {

                        if (localLoaderMap.TryGetValue(new KeyValuePair<short, short>(sffTex.linkPalletGroup, sffTex.linkPalletIndex), out localPalletLoader) && localPalletLoader != null) {
                            sffTex.indexTexture = localPalletLoader.TranslateIndexTexture(sffTex.indexTexture as Texture2D, true);
                            list[i] = sffTex;
                        } else {
                            Debug.LogError("LinkPallet没前置");
                        }
                    } else if (sffTex.localPalletData != null && sffTex.localPalletData.Length > 0) {
                        localPalletLoader = new PalletLoader(sffTex.localPalletData);
                        sffTex.indexTexture = localPalletLoader.TranslateIndexTexture(sffTex.indexTexture as Texture2D, true);
                        list[i] = sffTex;
                        localLoaderMap[new KeyValuePair<short, short>(sffTex.group, sffTex.image)] = localPalletLoader;
                    }
                }
            }


            if (m_PalletSelIndex < 0 && item.GlobalpalletLocalPaths != null) {
                m_PalletSelIndex = 0;
            }
            if (item.GlobalpalletLocalPaths != null && m_PalletSelIndex >= 0 && m_PalletSelIndex < item.GlobalpalletLocalPaths.Length) {
                string palletFileName = item.GlobalpalletLocalPaths[m_PalletSelIndex];
                string dir = System.IO.Path.GetDirectoryName(item.path);
                palletFileName = string.Format("{0}/{1}.bytes", dir, palletFileName);
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(palletFileName);
                if (textAsset != null) {
                    PalletLoader palletLoader = new PalletLoader(textAsset.bytes);
                    for (int i = 0; i < list.Count; ++i) {
                        var sffTex = list[i];

                        if (sffTex.UseGlobalPallet) {
                            sffTex.indexTexture = palletLoader.TranslateIndexTexture(sffTex.indexTexture as Texture2D, true);
                            list[i] = sffTex;
                        }
                    }
                    Caching.ClearCache();
                }
            } else {
                m_PalletSelIndex = -1;
            }
        }
        ClearTextures();
        if (list != null && list.Count > 0) {
            m_ItemSelectTexs = new Texture2D[list.Count];
            for (int i = 0; i < list.Count; ++i) {
                var sffTex = list[i];
                m_ItemSelectTexs[i] = sffTex.indexTexture;
            }
        }

        m_Material_RGB_A = true;
    }

    bool m_Material_RGB_A = true;

    private void DrawIdxMapSelects() {

    }

    private void DestroyTargetObject(UnityEngine.Object target) {
        if (target == null)
            return;
        if (Application.isPlaying)
            GameObject.Destroy(target);
        else
            GameObject.DestroyImmediate(target);
    }

    private void ExportCurrentGlobalPallet(string fileName, string rootDir, bool isMaterial_RGB_A = true) {
        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(fileName);
        if (textAsset == null)
            return;
        PalletLoader loader = new PalletLoader(textAsset.bytes);
        var palletTex = loader.PalletTexture(true);
        if (palletTex == null)
            return;
        string dirName = Path.GetFileName(rootDir);
        string name = Path.GetFileNameWithoutExtension(fileName);
        string targetDir = string.Format("assets/resources/character/{0}", dirName);
        if (!Directory.Exists(targetDir))
            Directory.CreateDirectory(targetDir);
        string saveFileName = string.Format("{0}/{1}.png", targetDir, name);
        byte[] buffer = palletTex.EncodeToPNG();
        FileStream stream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
        stream.Write(buffer, 0, buffer.Length);
        stream.Close();
        stream.Dispose();

        DestroyTargetObject(palletTex);


        SaveTextureImportSetting(saveFileName, TextureImporterFormat.RGBA32);

        // 创建Material
        palletTex = AssetDatabase.LoadAssetAtPath<Texture2D>(saveFileName);

        string matFileName = string.Format("{0}/{1}.mat", targetDir, name);
        Material mat = new Material(Shader.Find("Mugen/PalletShader"));
        mat.SetTexture("_PalletTex", palletTex);
        if (isMaterial_RGB_A) {
            mat.EnableKeyword("_RGB_A");
        } else {
            mat.DisableKeyword("_RGB_A");
        }

        AssetDatabase.CreateAsset(mat, matFileName);
        //DestroyTargetObject(mat);
    }

    TextureImporter SaveTextureImportSetting(string saveFileName, TextureImporterFormat format) {
        saveFileName = AssetBundleMgr.GetAssetRelativePath(saveFileName);

        AssetDatabase.Refresh();

        TextureImporter ti = AssetImporter.GetAtPath(saveFileName) as TextureImporter;
        ti.wrapMode = TextureWrapMode.Clamp;
        ti.mipmapEnabled = false;
        ti.filterMode = FilterMode.Point;
        ti.alphaIsTransparency = true;

        TextureImporterPlatformSettings platformSettings = ti.GetDefaultPlatformTextureSettings();
        platformSettings.format = format;
        platformSettings.textureCompression = TextureImporterCompression.Uncompressed;
        ti.SetPlatformTextureSettings(platformSettings);

        ti.SaveAndReimport();

        return ti;
    }

    private SffLoader ExportCurrentImages(string fileName, bool exportImage = true) {
        var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(fileName);
        if (textAsset == null)
            return null;
        string dirName = Path.GetDirectoryName(fileName);
        dirName = Path.GetFileName(dirName);
        string targetDir = string.Format("assets/resources/character/{0}", dirName);
        if (!Directory.Exists(targetDir))
            Directory.CreateDirectory(targetDir);


        SffLoader loader = new SffLoader(textAsset.bytes);

        if (exportImage) {
            List<Texture2D> texs = new List<Texture2D>();
            List<KeyValuePair<float, float>> offsetList = new List<KeyValuePair<float, float>>();
            List<short> imageNoList = new List<short>();
            Action<short, List<Texture2D>, List<KeyValuePair<float, float>>, List<short>> combineFunc = (short group, List<Texture2D> texList, List<KeyValuePair<float, float>> offList, List<short> noList) =>
          {
             // 合并
             Texture2D combineTex = new Texture2D(1, 1, TextureFormat.RGBA32, false, false);
              var rects = combineTex.PackTextures(texList.ToArray(), 2, 2048);
              if (rects.Length < texList.Count) {
                  Debug.LogErrorFormat("Group: {0:D} is not Combine All [count: {1:D}] combineCount: {2:D}", group, texList.Count, rects.Length);
              }

              for (int i = 0; i < texList.Count; ++i) {
                  DestroyTargetObject(texList[i]);
              }
              texList.Clear();

              string spriteTargetDir = string.Format("{0}/@{1:D}", targetDir, group);
              if (!Directory.Exists(spriteTargetDir))
                  Directory.CreateDirectory(spriteTargetDir);

              byte[] buffer = combineTex.EncodeToPNG();
              string saveFileName = string.Format("{0}/{1:D}.png", spriteTargetDir, group);
              FileStream stream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
              stream.Write(buffer, 0, buffer.Length);
              stream.Close();
              stream.Dispose();

              float w = combineTex.width;
              float h = combineTex.height;

              DestroyTargetObject(combineTex);

              AssetDatabase.Refresh();

              saveFileName = AssetBundleMgr.GetAssetRelativePath(saveFileName);

              var import = SaveTextureImportSetting(saveFileName, TextureImporterFormat.Alpha8);
              import.textureType = TextureImporterType.Sprite;
              import.spriteImportMode = SpriteImportMode.Multiple;
              var spreetSheet = new SpriteMetaData[rects.Length];
              for (int i = 0; i < rects.Length; ++i) {
                  var r = rects[i];
                  var sheet = spreetSheet[i];
                  sheet.name = noList[i].ToString();
                  sheet.alignment = (int)SpriteAlignment.Custom;
                  sheet.rect = new Rect(r.x * w, r.y * h, r.width * w, r.height * h);
                 //sheet.border = new Vector4(r.x, r.y, r.width, r.height);

                 float offsetX = offList[i].Key;
                  float offsetY = offList[i].Value;
                 // Vector2 center = new Vector2(0.5f, 0.5f);
                 // var off = new Vector2(center.x * r.width - offsetX * r.width, center.y * r.height - offsetY * r.height);
                 Vector2 off = new Vector2(offsetX, offsetY);

                  sheet.pivot = off;

                  spreetSheet[i] = sheet;
              }

              import.spritesheet = spreetSheet;

              offList.Clear();
              noList.Clear();

              import.SaveAndReimport();
          };

            short lastGroup = -1;
            loader.ForEachPcx(
                (SffTexture sffTex) =>
                {
                    if (sffTex.group != lastGroup && lastGroup >= 0) {

                        combineFunc(lastGroup, texs, offsetList, imageNoList);
                        lastGroup = sffTex.group;
                    }

                    if (lastGroup < 0)
                        lastGroup = sffTex.group;

                    texs.Add(sffTex.indexTexture as Texture2D);
                    offsetList.Add(new KeyValuePair<float, float>(sffTex.offsetX, sffTex.offsetY));
                    imageNoList.Add(sffTex.image);
                }, true
                );


            combineFunc(lastGroup, texs, offsetList, imageNoList);
        }


        return loader;
    }

    private enum TExportPlayerType
    {
        indexTex,
        pallet,
        Anim,
        PlayerData,
        Sound
    };

    private static readonly int cExportIdxTex = (1 << (int)TExportPlayerType.indexTex);
    private static readonly int cExportPallet = (1 << (int)TExportPlayerType.pallet);
    private static readonly int cExportAnim = (1 << (int)TExportPlayerType.Anim);
    private static readonly int cPlayerData = (1<< (int)TExportPlayerType.PlayerData);
    private static readonly int cSound = (1 << (int)TExportPlayerType.Sound);

    private static readonly int cExportPlayerAll = cExportIdxTex | cExportPallet | cExportAnim | cPlayerData | cSound;

    private void ExportSelectPlayer(int exportType) {

        if (m_ItemSelect < 0 || m_ItemSelect >= m_Items.Count)
            return;
        var item = m_Items[m_ItemSelect];
        if (item == null)
            return;
        var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(item.path);
        if (textAsset == null)
            return;
        byte[] buffer = textAsset.bytes;
        string text = System.Text.Encoding.UTF8.GetString(buffer);
        PlayerCfgLoader loader = new PlayerCfgLoader(text);
        var playerFiles = loader.files;
        if (playerFiles == null)
            return;

        string parentDir = Path.GetDirectoryName(item.path);

        bool isExportImg = (exportType & cExportIdxTex) != 0;
        bool isExportAir = (exportType & cExportAnim) != 0;

        SffLoader sffLoader = null;
        if (isExportImg || isExportAir) {
            string spriteFileName = string.Format("{0}/{1}.bytes", parentDir, playerFiles.sprite);
            // 导出所有动画图片
            sffLoader = ExportCurrentImages(spriteFileName, isExportImg);
        }

        string[] pals = null;
        if ((exportType & cExportPallet) != 0) {
            pals = playerFiles.ToPalLocalPaths;
            for (int i = 0; i < pals.Length; ++i) {
                string palletFileName = string.Format("{0}/{1}.bytes", parentDir, pals[i]);
                ExportCurrentGlobalPallet(palletFileName, parentDir, m_Material_RGB_A);
            }
        }

        string animFileName = string.Empty;
        if (isExportAir) {
            animFileName = string.Format("{0}/{1}.txt", parentDir, playerFiles.anim);
            AirAnimExporter.Export(animFileName, parentDir, sffLoader);
        }

        if ((exportType & cPlayerData) != 0) {
            PlayerLoaderData playerData = new PlayerLoaderData();
            if (pals != null && pals.Length > 0) {
                playerData.palletLocalPaths = new string[pals.Length];
                for (int i = 0; i < pals.Length; ++i) {
                    string pal = pals[i];
                    if (string.IsNullOrEmpty(pal))
                        continue;
                    pal = Path.GetFileName(pal);
                    if (string.IsNullOrEmpty(pal))
                        continue;
                    playerData.palletLocalPaths[i] = (pal + ".mat").Replace('\\', '/');
                }
            }
            if (!string.IsNullOrEmpty(animFileName)) {
                playerData.aniLocalPath = (Path.GetFileNameWithoutExtension(animFileName) + ".bytes").Replace('\\', '/');
            }

            string dirName = Path.GetFileName(parentDir);
            string playerDataFileName = string.Format("assets/resources/character/{0}/{0}.bytes", dirName);
            FileStream playerDataStream = new FileStream(playerDataFileName, FileMode.Create, FileAccess.Write);
            try {
                BinaryFormatter writer = new BinaryFormatter();
                writer.Serialize(playerDataStream, playerData);
            } finally {
                playerDataStream.Close();
                playerDataStream.Dispose();
            }
        }

        if ((exportType & cSound) != 0) {
            string soundFileName = string.Format("{0}/{1}.bytes", parentDir, item.snd);
            FileStream sndStream = new FileStream(soundFileName, FileMode.Open, FileAccess.Read);
            SndLoader sndLoader = null;
            try {
                buffer = new byte[sndStream.Length];
                sndStream.Read(buffer, 0, buffer.Length);
                sndLoader = new SndLoader(buffer);
            } finally {
                sndStream.Close();
                sndStream.Dispose();
            }
            if (sndLoader != null && sndLoader.SoundCount > 0) {
                string dirName = Path.GetFileName(parentDir);
                string sndRootPath = string.Format("assets/resources/character/{0}/sound", dirName);
                if (!Directory.Exists(sndRootPath))
                    Directory.CreateDirectory(sndRootPath);
                var sndIter = sndLoader.GetSoundIter();
                while (sndIter.MoveNext()) {

                    if (sndIter.Current.Value == null || sndIter.Current.Value.Length <= 0)
                        continue;
                    
                    // 测试是否有效果
                    try {
                        Utils.WAV testWav = new Utils.WAV(sndIter.Current.Value);
                    } catch {
                        Debug.LogErrorFormat("【Sound Error】group {0:D} Image {1:D}", sndIter.Current.Key.Key, sndIter.Current.Key.Value);
                        continue;
                    }
                    soundFileName = string.Format("{0}/{1:D}_{2:D}.wav", sndRootPath, sndIter.Current.Key.Key, sndIter.Current.Key.Value);
                    FileStream sndItemStream = new FileStream(soundFileName, FileMode.Create, FileAccess.Write);
                    try {
                        sndItemStream.Write(sndIter.Current.Value, 0, sndIter.Current.Value.Length);
                    } finally {
                        sndItemStream.Close();
                        sndItemStream.Dispose();
                    }

                    AssetDatabase.Refresh();

                    // 处理声音格式
                    AudioImporter audioImporter = AssetImporter.GetAtPath(AssetBundleMgr.GetAssetRelativePath(soundFileName)) as AudioImporter;
                    if (audioImporter != null) {
                        audioImporter.forceToMono = true;
                        audioImporter.loadInBackground = false;
                       // audioImporter.preloadAudioData = false;

                        AudioImporterSampleSettings audioSettings = new AudioImporterSampleSettings();
                        audioSettings.loadType = AudioClipLoadType.DecompressOnLoad;
                        audioSettings.quality = 100;
                        audioSettings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
                        audioSettings.sampleRateOverride = 22050;
                        audioSettings.compressionFormat = AudioCompressionFormat.Vorbis;

                        audioImporter.SetOverrideSampleSettings("Android", audioSettings);
                        audioImporter.SetOverrideSampleSettings("iOS", audioSettings);
                        audioImporter.SetOverrideSampleSettings("Standalone", audioSettings);

                        audioImporter.SaveAndReimport();
                    }
                }
                sndIter.Dispose();
            }
        }

        Caching.ClearCache();

        AssetDatabase.Refresh();
    }

    /*
    void DoBtnAnim() {
        if (m_ItemSelect < 0 || m_ItemSelect >= m_Items.Count)
            return;
        var item = m_Items[m_ItemSelect];
        if (item == null)
            return;

        string parentDir = Path.GetDirectoryName(item.path);
        string dirName = Path.GetFileName(parentDir);
        string name = Path.GetFileNameWithoutExtension(parentDir);
        string targetDir = string.Format("assets/resources/character/{0}", dirName);
        if (!Directory.Exists(targetDir))
            return;
        string[] amimFiles = Directory.GetFiles(targetDir, "*.air.bytes", SearchOption.AllDirectories);
        if (amimFiles == null || amimFiles.Length <= 0)
            return;
        string animFile = amimFiles[0];
        PlayerAnimShow.Open(animFile, dirName);
    }
    */

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("全部收缩", GUILayout.Width(100))) {
            m_ItemSelect = -1;
            OnItemSelectChanged();
        }

        if (m_ItemSelect >= 0) {

            if (m_Items != null && m_ItemSelect < m_Items.Count) {
                var item = m_Items[m_ItemSelect];
                if (item != null) {
                    int newPalletIdx = EditorGUILayout.Popup(m_PalletSelIndex, item.GlobalpalletLocalPaths);
                    if (newPalletIdx != m_PalletSelIndex) {
                        m_PalletSelIndex = newPalletIdx;
                        OnItemSelectChanged();
                    }
                }
            }

            bool newMatRGBA = GUILayout.Toggle(m_Material_RGB_A, "调色版选项");
            if (m_Material_RGB_A != newMatRGBA) {
                m_Material_RGB_A = newMatRGBA;
            }

            if (GUILayout.Button("导出当前角色", GUILayout.Width(100))) {
                ExportSelectPlayer(cExportPlayerAll);
            }

            if (GUILayout.Button("导出当前索引图片", GUILayout.Width(100))) {
                ExportSelectPlayer(cExportIdxTex);
            }

            if (GUILayout.Button("导出当前调色版", GUILayout.Width(100))) {
                ExportSelectPlayer(cExportPallet);
            }

            if (GUILayout.Button("导出所有动画", GUILayout.Width(100))) {
                ExportSelectPlayer(cExportAnim);
            }

            if (GUILayout.Button("导出所有声音", GUILayout.Width(100))) {
                ExportSelectPlayer(cSound);
            }

            /*
            if (GUILayout.Button("查看角色动画组", GUILayout.Width(100))) {
                DoBtnAnim();
            }
            */

        }
        EditorGUILayout.EndHorizontal();

        m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos);


        for (int i = 0; i < m_Items.Count; ++i) {
            var item = m_Items[i];
            if (item == null)
                continue;
#if UNITY_5_6 || UNITY_2017
			if (EditorGUILayout.Foldout(m_ItemSelect == i, item.name))
#else
            if (EditorGUILayout.BeginFoldoutHeaderGroup(m_ItemSelect == i, item.name))
#endif
            {
                if (m_ItemSelect != i) {
                    m_ItemSelect = i;
                    OnItemSelectChanged();
                }

                if (m_ItemSelect == i && m_ItemSelectTexs != null && m_ItemSelectTexs.Length > 0) {
                    DrawIdxMapSelects();

                    var oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.green;
                    GUILayout.SelectionGrid(-1, m_ItemSelectTexs, 10, GUILayout.Width(Screen.width)); // , 10, GUILayout.Width(Screen.width)
                    GUI.backgroundColor = oldColor;
                }
            }
#if UNITY_5_6 || UNITY_2017
            //EditorGUILayout.EndFadeGroup();
#else
            EditorGUILayout.EndFoldoutHeaderGroup();
#endif
        }
        EditorGUILayout.EndScrollView();
    }

    void Init() {
        m_Items.Clear();
        string[] sffs = Directory.GetFiles("assets/ExResources/Character", "*.def.txt", SearchOption.AllDirectories);
        if (sffs != null && sffs.Length > 0) {
            for (int i = 0; i < sffs.Length; ++i) {
                string path = sffs[i];
                if (string.IsNullOrEmpty(path))
                    continue;
                path = path.Replace("\\", "/");
                ViewItem item = new ViewItem(path);
                m_Items.Add(item);

               
            }
        }
    }

    [MenuItem("Assets/Mugen/Mugen资源转Unity资源", true)]
    [MenuItem("Assets/Mugen/Unity资源转Mugen资源", true)]
    public static bool IsProcessMugenFilesToUnityResFiles() {
        return Selection.activeGameObject == null && Selection.activeObject != null;
    }

    // 选中文件夹处理
    [MenuItem("Assets/Mugen/Mugen资源转Unity资源")]
    public static void ProcessMugenFilesToUnityResFiles() {
        var selObj = Selection.activeObject;
        if (selObj != null) {
            string path = AssetDatabase.GetAssetPath(selObj);
            if (string.IsNullOrEmpty(path))
                return;
            path = System.IO.Path.GetDirectoryName(path);
            string[] files = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
            if (files != null && files.Length > 0) {
                for (int i = 0; i < files.Length; ++i) {
                    string srcPath = files[i];
                    if (string.IsNullOrEmpty(srcPath))
                        continue;
                    string ext = System.IO.Path.GetExtension(srcPath);
                    if (string.IsNullOrEmpty(ext))
                        continue;
                    string chgExt = string.Empty;
                    if (string.Compare(ext, ".def", true) == 0)
                        chgExt = ".def.txt";
                    else if (string.Compare(ext, ".sff", true) == 0)
                        chgExt = ".sff.bytes";
                    else if (string.Compare(ext, ".snd", true) == 0)
                        chgExt = ".snd.bytes";
                    else if (string.Compare(ext, ".air", true) == 0)
                        chgExt = ".air.txt";
                    else if (string.Compare(ext, ".act", true) == 0)
                        chgExt = ".act.bytes";
                    else if (string.Compare(ext, ".ai", true) == 0)
                        chgExt = ".ai.bytes";
                    else if (string.Compare(ext, ".cmd", true) == 0)
                        chgExt = ".cmd.txt";
                    else if (string.Compare(ext, ".cns", true) == 0)
                        chgExt = ".cns.txt";
                    if (string.IsNullOrEmpty(chgExt))
                        continue;
                    string dstPath = System.IO.Path.ChangeExtension(srcPath, chgExt);
                    //  FileUtil.ReplaceFile(srcPath, dstPath);
                    System.IO.File.Move(srcPath, dstPath);
                }

                AssetDatabase.Refresh();
            }
        }
    }

    [MenuItem("Assets/Mugen/Unity资源转Mugen资源")]
    // 选中文件夹处理
    public static void ProcessUnityResFilesToMugenFiles() {
        var selObj = Selection.activeObject;
        if (selObj != null) {
            string path = AssetDatabase.GetAssetPath(selObj);
            if (string.IsNullOrEmpty(path))
                return;
            path = System.IO.Path.GetDirectoryName(path);
            string[] files = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
            if (files != null && files.Length > 0) {
                for (int i = 0; i < files.Length; ++i) {
                    string srcPath = files[i];
                    if (string.IsNullOrEmpty(srcPath))
                        continue;
                    string ext = System.IO.Path.GetExtension(srcPath);
                    if (string.IsNullOrEmpty(ext))
                        continue;
                    string chgExt = string.Empty;
                    if (srcPath.IndexOf(".def.txt", System.StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        chgExt = ".def";
                        ext = ".def.txt";
                    } else if (srcPath.IndexOf(".sff.bytes", System.StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        chgExt = ".sff";
                        ext = ".sff.bytes";
                    } else if (srcPath.IndexOf(".snd.bytes", System.StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        chgExt = ".snd";
                        ext = ".snd.bytes";
                    } else if (srcPath.IndexOf(".air.txt", System.StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        chgExt = ".air";
                        ext = ".air.txt";
                    } else if (srcPath.IndexOf(".act.bytes", System.StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        chgExt = ".act";
                        ext = ".act.bytes";
                    } else if (srcPath.IndexOf(".ai.bytes", System.StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        chgExt = ".ai";
                        ext = ".ai.bytes";
                    } else if (srcPath.IndexOf(".cmd.txt", System.StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        chgExt = ".cmd";
                        ext = ".cmd.txt";
                    } else if (srcPath.IndexOf(".cns.txt", System.StringComparison.CurrentCultureIgnoreCase) >= 0) {
                        chgExt = ".cns";
                        ext = ".cns.txt";
                    }

                    if (string.IsNullOrEmpty(chgExt))
                        continue;
                    string dstPath = srcPath.Replace(ext, chgExt);
                    System.IO.File.Move(srcPath, dstPath);
                }

                AssetDatabase.Refresh();
            }

        }
    }

    [MenuItem("Assets/创建角色")]
    [MenuItem("Tools/创建角色")]
    public static void CreateCharacter() {
        InputDialogWindow.CreateWindow("请输入角色名", "角色目录名",
            (InputDialogWindow wnd) =>
            {
                string playerName = wnd.InputText;
                if (!string.IsNullOrEmpty(playerName)) {
                    playerName = playerName.Trim();
                    if (!string.IsNullOrEmpty(playerName)) {
                        GameObject obj = Common.CreatePlayer(playerName);
                        /*
                        var bOwner = obj.AddComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>();
                        bOwner.repeat = true;
                        */
                    }
                }
            }
            );
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

class ViewItem
{
    public string path;
    public string name;
	public string[] palletPaths;

    public ViewItem(string path)
    {
        this.path = path;
        this.name = Path.GetFileName(Path.GetDirectoryName(path));
        if (this.name.StartsWith("@"))
        {
            this.name = this.name.Substring(1);
        }
    }

    public List<Texture> GetTextures()
    {
        FileStream stream = new FileStream(this.path, FileMode.Open, FileAccess.Read);
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
    public static void Open()
    {
        // Rect r = new Rect((Screen.width - 1920) / 2, (Screen.height - 800) / 2, 1920, 800);
       
        SffViewWindow wnd = EditorWindow.GetWindow<SffViewWindow>("Sff查看器", typeof(SceneView));
        
       // SffViewWindow wnd = EditorWindow.GetWindowWithRect<SffViewWindow>(r, false, "Sff查看器", true);
       // wnd.maxSize = new Vector2(Screen.width, Screen.height);
        wnd.Init();
    }

    private void OnDestroy()
    {
        ClearTextures();
	}

    void ClearTextures()
    {
        if (m_ItemSelectTexs != null)
        {
            for (int i = 0; i < m_ItemSelectTexs.Length; ++i)
            {
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

    void OnItemSelectChanged()
    {
        List<Texture> list = null;
        if (m_ItemSelect >= 0)
        {
            var item = m_Items[m_ItemSelect];
            list = item.GetTextures();
        }
        ClearTextures();
        if (list != null)
            m_ItemSelectTexs = list.ToArray();
    }

	private void DrawIdxMapSelects()
	{
		
	}

    private void OnGUI()
    {
		EditorGUILayout.BeginHorizontal ();
        if (GUILayout.Button("全部收缩", GUILayout.Width(100)))
        {
            m_ItemSelect = -1;
            OnItemSelectChanged();
        }

		if (m_ItemSelect >= 0) {
			if (GUILayout.Button ("导出当前Sff", GUILayout.Width (100))) {
			}
				
		}
		EditorGUILayout.EndHorizontal ();

        m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos);


        for (int i = 0; i < m_Items.Count; ++i)
        {
            var item = m_Items[i];
            if (item == null)
                continue;
			#if UNITY_5_6
			if (EditorGUILayout.Foldout(m_ItemSelect == i, item.name))
			#else
            if (EditorGUILayout.BeginFoldoutHeaderGroup(m_ItemSelect == i, item.name))
			#endif
            {
                if (m_ItemSelect != i)
                {
                    m_ItemSelect = i;
                    OnItemSelectChanged();
                }

                if (m_ItemSelect == i && m_ItemSelectTexs != null && m_ItemSelectTexs.Length > 0)
                {
					DrawIdxMapSelects ();

                    var oldColor = GUI.backgroundColor;
					GUI.backgroundColor = Color.black;
                    GUILayout.SelectionGrid(-1, m_ItemSelectTexs, 5, GUILayout.Width(Screen.width));
                    GUI.backgroundColor = oldColor;
                }
            }
			#if UNITY_5_6
			//EditorGUILayout.EndFadeGroup();
			#else
            EditorGUILayout.EndFoldoutHeaderGroup();
			#endif
        }
        EditorGUILayout.EndScrollView();
    }

    void Init()
    {
        m_Items.Clear();
        string[] sffs = Directory.GetFiles("assets/resources/chars", "*.sff.bytes", SearchOption.AllDirectories);
        if (sffs != null && sffs.Length > 0)
        {
            for (int i = 0; i < sffs.Length; ++i)
            {
                string path = sffs[i];
                if (string.IsNullOrEmpty(path))
                    continue;
                path = path.Replace("\\", "/");
                ViewItem item = new ViewItem(path);
                m_Items.Add(item);
            }
        }
    }
}

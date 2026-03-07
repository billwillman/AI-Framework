using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using Mugen;


public enum AirAnimExportMode
{
    Json,
    CSharpBinary
}

// 导出动画功能
public static class AirAnimExporter {

    public static string GenAnimConfigFileName(string airFileName, string rootDir) {
        string name = Path.GetFileNameWithoutExtension(airFileName);
        string targetDir = GenAnimConfigTargetDir(airFileName, rootDir);
        string outFileName = string.Format("{0}/{1}.bytes", targetDir, name);
        return outFileName;
    }

    private static string GenAnimConfigTargetDir(string airFileName, string rootDir) {
        string dirName = Path.GetFileName(rootDir);
        string name = Path.GetFileNameWithoutExtension(airFileName);
        string targetDir = string.Format("assets/resources/character/{0}", dirName);
        if (!Directory.Exists(targetDir))
            Directory.CreateDirectory(targetDir);
        return targetDir;
    }

    private static string GenAnimationClipDir(string airFileName, string rootDir) {
        string dirName = Path.GetFileName(rootDir);
        string name = Path.GetFileNameWithoutExtension(airFileName);
        string targetDir = string.Format("assets/resources/character/{0}/anim", dirName);
        if (!Directory.Exists(targetDir))
            Directory.CreateDirectory(targetDir);
        return targetDir;
    }

    public static float _cImageAnimationScale = ImageAnimation._cImageAnimationScale;

    public static void Export(string airFileName, string rootDir, SffLoader sffLoader) {
        if (sffLoader == null)
            return;
        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(airFileName);
        if (textAsset == null)
            return;
        string text = System.Text.Encoding.UTF8.GetString(textAsset.bytes);

        PlayerAnimation animation = new PlayerAnimation(text);
        if (!animation.IsVaild)
            return;

        var animMap = animation.AnimMap;
        if (animMap == null)
            return;

        // 处理图片链接问题
        var aniIter = animMap.GetEnumerator();
        while (aniIter.MoveNext()) {
            var list = aniIter.Current.Value;
            if (list != null) {
                for (int i = 0; i < list.Count; ++i) {
                    var frame = list[i];
                    SFFSUBHEADER subHeader;
                    if (sffLoader.GetSubHeader(frame.frameGroup, frame.frameIndex, out subHeader)) {
                        if (frame.frameGroup != subHeader.GroubNumber || frame.frameIndex != subHeader.ImageNumber) {
                            Debug.LogFormat("Frame Replace: {0:D}, {1:D} to {2:D}, {3:D}", 
                                frame.frameGroup, frame.frameIndex, subHeader.GroubNumber, subHeader.ImageNumber);
                            
                            frame.frameGroup = subHeader.GroubNumber;
                            frame.frameIndex = subHeader.ImageNumber;

                            list[i] = frame;
                        }
                    }
                }
            }
        }
        aniIter.Dispose();
        // ------------------------

        string outFileName = GenAnimConfigFileName(airFileName, rootDir);

        BinaryFormatter binaryWriter = new BinaryFormatter();


        FileStream stream = new FileStream(outFileName, FileMode.Create, FileAccess.Write);
        try {
            binaryWriter.Serialize(stream, animMap);
        } finally {
            stream.Dispose();
        }

        // 生成animationClip

        string nextFrameStr = "NextFrame";
        string endFrameStr = "EndFrame";
        string firstFrameStr = "StartFrame";

        string animClipDir = GenAnimationClipDir(airFileName, rootDir);
        var iter = animMap.GetEnumerator();
        while (iter.MoveNext()) {
            int actionNo = iter.Current.Key;
            string actionNoStr = actionNo.ToString();
            var clip = new AnimationClip();
            clip.frameRate = 30;
            clip.legacy = true;
            clip.name = actionNoStr;
            List<AnimationEvent> evtList = new List<AnimationEvent>();
            if (iter.Current.Value != null && iter.Current.Value.Count > 0) {

                if (iter.Current.Value.Count >= 2) {
                    float sumTime = 0;

                    AnimationEvent evt = new AnimationEvent();
                    evt.functionName = firstFrameStr;
                    evt.messageOptions = SendMessageOptions.DontRequireReceiver;
                    evt.time = 0;
                    evtList.Add(evt);
                    for (int i = 0; i < iter.Current.Value.Count; ++i) {
                        ImageAnimateNode frame = iter.Current.Value[i];
                        float evtTime = frame.AniTick * _cImageAnimationScale;
                        sumTime += evtTime;
                        AnimationEvent aniEvt = new AnimationEvent();
                        if (i == iter.Current.Value.Count - 1)
                            aniEvt.functionName = endFrameStr;
                        else
                            aniEvt.functionName = nextFrameStr;
                        aniEvt.messageOptions = SendMessageOptions.DontRequireReceiver;
                        aniEvt.time = sumTime;
                        evtList.Add(aniEvt);
                    }

                } else {
                    // 直接針添加StartFrame EndFrame
                    AnimationEvent evt = new AnimationEvent();
                    evt.functionName = firstFrameStr;
                    evt.messageOptions = SendMessageOptions.DontRequireReceiver;
                    evt.time = 0;
                    evtList.Add(evt);

                    evt.functionName = endFrameStr;
                    evt.messageOptions = SendMessageOptions.DontRequireReceiver;
                    //evt.time = 1.0f/clip.frameRate;
                    evt.time = Time.fixedDeltaTime;
                    //	evt.time = _cLimitFrameDeltaTime;
                    evtList.Add(evt);
                }

                if (Application.isPlaying)
                    clip.events = evtList.ToArray();
                else
                    UnityEditor.AnimationUtility.SetAnimationEvents(clip, evtList.ToArray());

                outFileName = string.Format("{0}/{1:D}.anim", animClipDir, actionNo);
                AssetDatabase.CreateAsset(clip, outFileName);
                // 创建Animancer Translate Asset
                /*
                Animancer.ClipTransitionAsset animancerAsset = ScriptableObject.CreateInstance<Animancer.ClipTransitionAsset>();
                animancerAsset.Transition.Clip = clip;
                outFileName = Path.ChangeExtension(outFileName, ".asset");
                AssetDatabase.CreateAsset(animancerAsset, outFileName);
                */
            }
        }
        
        iter.Dispose();
        // --------------------------


        AssetDatabase.Refresh();
    }
}

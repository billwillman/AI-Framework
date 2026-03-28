using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class UnityTimelinePlayableBehaviour : PlayableBehaviour
{
    [System.NonSerialized]
    public UnityTimeline.UnityTimelineTree RuntimeTree = null;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        if (RuntimeTree != null) {
            if (RuntimeTree.DirectorController == null) {
                if (playerData != null) {
                    PlayableDirector director = playerData as PlayableDirector;
                    if (director != null) {
                        RuntimeTree.SetDirectorController(new UnityTimeline.PlayableDirectorController(director));
                    }
                }
            }
            if (RuntimeTree.DirectorController != null) {
                RuntimeTree.UpdateTree(info.deltaTime);
            }
        }
    }


    public void DestroyRuntimeTree(bool isCallCallBack = false) {
        if (RuntimeTree != null) {
            if (isCallCallBack)
                RuntimeTree.OnTreeDestroy();
            RuntimeTree.ResetTree();
            RuntimeTree.DisposeTree();
            if (Application.isPlaying)
                GameObject.Destroy(RuntimeTree);
            else
                GameObject.DestroyImmediate(RuntimeTree);
            RuntimeTree = null;
        }
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info) {
        if (RuntimeTree != null) {
            RuntimeTree.OnTreeEnable();
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info) {
        if (RuntimeTree != null) {
            RuntimeTree.OnTreeDisable();
        }
    }

    public override void OnPlayableDestroy(Playable playable) {
        DestroyRuntimeTree(true);
    }
}

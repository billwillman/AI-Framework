using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Animancer;
using UnityEngine.Timeline;

public static class AnimancerUnityTimelineExtend
{
    private static void ApplyPlayableAssetState(PlayableAssetState state) {
        if (state == null)
            return;
        var graph = state.Root._Graph;
        for (int i = 0; i < graph.GetOutputCountByType<ScriptPlayableOutput>(); ++i) {
            var PlayableOutput = graph.GetOutputByType<ScriptPlayableOutput>(i);
            var refObj = PlayableOutput.GetReferenceObject();
            if (refObj is UnityTimelinePlayableTrack) {
              //  PlayableOutput.SetUserData(state);
            }
        }
    }
    
    public static AnimancerState PlayTimeline(this AnimancerComponent component, PlayableAssetTransitionAsset asset, float fadeDuration, FadeMode mode = default) {
        if (asset = null)
            return null;
        AnimancerState temp;
        if (component.States.TryGet(asset, out temp)) {
            return component.Play(temp, fadeDuration, mode);
        }
        PlayableAssetState state = component.Play(asset, fadeDuration, mode) as PlayableAssetState;
        ApplyPlayableAssetState(state);
        return state;
    }

    public static AnimancerState PlayTimeline(this AnimancerComponent component, PlayableAssetTransitionAsset asset) {
        if (asset == null)
            return null;
        AnimancerState temp;
        if (component.States.TryGet(asset, out temp)) {
            return component.Play(temp, asset.FadeDuration, asset.FadeMode);
        }
        PlayableAssetState state = component.Play(asset) as PlayableAssetState;
        ApplyPlayableAssetState(state);
        return state;
    }
}

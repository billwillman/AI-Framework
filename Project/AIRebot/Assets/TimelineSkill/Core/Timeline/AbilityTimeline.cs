using System;
using UnityEngine;
using Taco.Gameplay;
using Taco.Timeline;

[AcceptableTrackGroups("Base", "Character", "Ability")]
public partial class AbilityTimeline : CharacterTimeline
{
    public Ability Ability { get; set; }

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Assets/Create/Taco/Timeline/AbilityTimeline")]
    public static void CreateAbilityTimeline()
    {
        AbilityTimeline timeline = CreateInstance<AbilityTimeline>();
        string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
        string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New Timeline.asset");
        UnityEditor.AssetDatabase.CreateAsset(timeline, assetPathAndName);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.Selection.activeObject = timeline;
    }
#endif
}

public abstract class AbilityClip : Clip
{
    public Ability Ability { get; private set; }

    public override void Bind()
    {
        base.Bind();
        if (Timeline is AbilityTimeline abilityTimeline)
            Ability = abilityTimeline.Ability;
    }

#if UNITY_EDITOR

    protected AbilityClip(Track track, int frame) : base(track, frame) { }
#endif
}

public abstract class AbilitySignalClip : SignalClip
{
    public Ability Ability { get; private set; }

    public override void Bind()
    {
        base.Bind();
        if (Timeline is AbilityTimeline abilityTimeline)
            Ability = abilityTimeline.Ability;
    }

#if UNITY_EDITOR

    protected AbilitySignalClip(Track track, int frame) : base(track, frame) { }
#endif
}



[TrackGroup("Ability"), ScriptGuid("f6e6bb52a7c30ce4789f78dcd9059c85"), IconGuid("e28acf5dc5b2e3d4a97920bf4e831c87"), Ordered(5), Color(201, 060, 032)]
public class ModifyRuntimeBlockTagTrack : Track
{

#if UNITY_EDITOR

    public override Type ClipType => typeof(ModifyRuntimeBlockTagClip);
#endif
}

[ScriptGuid("f6e6bb52a7c30ce4789f78dcd9059c85"), Color(201, 060, 032)]
public class ModifyRuntimeBlockTagClip : AbilityClip
{
    [ShowInInspector]
    public GameplayTagContainer Tag;

    public override void OnEnable()
    {
        base.OnEnable();
        if (Ability)
        {
            foreach (var tag in Tag.Tags)
            {
                Ability.AbilityRunner.BlockAbilitiesWithTag.Add(tag);
            }
        }
    }
    public override void OnDisable()
    {
        base.OnDisable();
        if (Ability)
        {
            foreach (var tag in Tag.Tags)
            {
                Ability.AbilityRunner.BlockAbilitiesWithTag.Remove(tag);
            }
        }
    }

#if UNITY_EDITOR

    public override string Name => "RuntimeBlockTag";
    public override ClipCapabilities Capabilities => ClipCapabilities.Resizable;
    public ModifyRuntimeBlockTagClip(Track track, int frame) : base(track, frame) { }
#endif
}

[TrackGroup("Ability"), ScriptGuid("f6e6bb52a7c30ce4789f78dcd9059c85"), IconGuid("e28acf5dc5b2e3d4a97920bf4e831c87"), Ordered(5), Color(201, 060, 032)]
public class ModifyCanBuffAbilitiesTagTrack : Track
{

#if UNITY_EDITOR

    public override Type ClipType => typeof(ModifyCanBuffAbilitiesTagClip);
#endif
}

[ScriptGuid("f6e6bb52a7c30ce4789f78dcd9059c85"), Color(201, 060, 032)]
public class ModifyCanBuffAbilitiesTagClip : AbilityClip
{
    [ShowInInspector]
    public GameplayTagContainer Tag;

    public override void OnEnable()
    {
        base.OnEnable();
        if (Ability)
        {
            foreach (var tag in Tag.Tags)
            {
                Ability.AbilityRunner.CanBufferAbilitiesTag.Add(tag);
            }
        }
    }
    public override void OnDisable()
    {
        base.OnDisable();
        if (Ability)
        {
            foreach (var tag in Tag.Tags)
            {
                Ability.AbilityRunner.CanBufferAbilitiesTag.Remove(tag);
            }
        }
    }

#if UNITY_EDITOR

    public override string Name => "CanBuffAbilitiesTag";
    public override ClipCapabilities Capabilities => ClipCapabilities.Resizable;
    public ModifyCanBuffAbilitiesTagClip(Track track, int frame) : base(track, frame) { }
#endif
}
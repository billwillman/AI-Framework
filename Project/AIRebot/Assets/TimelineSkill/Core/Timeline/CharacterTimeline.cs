using System;
using UnityEngine;
using Taco.Timeline;


[AcceptableTrackGroups("Base", "Character")]
public class CharacterTimeline : Timeline
{
    public PlatformCharacter Character { get; set; }

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Assets/Create/Taco/Timeline/CharacterTimeline")]
    public static void CreateCharacterTimeline()
    {
        CharacterTimeline timeline = CreateInstance<CharacterTimeline>();
        string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
        string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New Timeline.asset");
        UnityEditor.AssetDatabase.CreateAsset(timeline, assetPathAndName);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.Selection.activeObject = timeline;
    }
#endif
}
public abstract class CharacterTrack : Track
{
    public PlatformCharacter Character { get; set; }

    public override void Bind()
    {
        base.Bind();
        if (Timeline is CharacterTimeline characterTimeline)
            Character = characterTimeline.Character;
    }
}
public abstract class CharacterClip : Clip
{
    public PlatformCharacter Character { get; set; }

    public override void Bind()
    {
        base.Bind();
        if (Timeline is CharacterTimeline characterTimeline)
            Character = characterTimeline.Character;
    }

#if UNITY_EDITOR

    protected CharacterClip(Track track, int frame) : base(track, frame) { }
#endif
}



[TrackGroup("Character"), ScriptGuid("828a8311b89bab843b655b0181bf41e4"), IconGuid("e6435fa591ae4414eb0f26dc6410086e"), Ordered(4), Color(127, 253, 228)]
public class RootMotionTrack : CharacterTrack
{
    public override void Bind()
    {
        base.Bind();
        Character?.EnableRootmotion(true);
    }
    public override void Unbind()
    {
        base.Unbind();
        Character?.EnableRootmotion(false);
    }


#if UNITY_EDITOR

    public override Type ClipType => typeof(RootMotionClip);
#endif
}

[ScriptGuid("828a8311b89bab843b655b0181bf41e4"), Color(127, 253, 228)]
public class RootMotionClip : CharacterClip
{
    float m_ChangedValue;

    public override void Bind()
    {
        base.Bind();
        m_ChangedValue = 0;
    }
    public override void Unbind()
    {
        base.Unbind();
        if (Character)
            Character.RootMotionWeight -= m_ChangedValue;
    }

    public override void Evaluate(float deltaTime)
    {
        float targetWeight = 0;
        TargetTime = Time += deltaTime;

        if (!Character) return;

        if (Time < StartTime)
        {
            targetWeight = 0;

        }
        else if (StartTime <= Time && Time <= EndTime)
        {
            float selfTime = Time - StartTime;
            float remainTime = EndTime - Time;

            if (selfTime < EaseInTime)
            {
                targetWeight = selfTime / EaseInTime;
            }
            else if (remainTime < EaseOutTime)
            {
                targetWeight = remainTime / EaseOutTime;
            }
            else
            {
                targetWeight = 1;
            }
        }
        else if (Time > EndTime)
        {
            targetWeight = 0;
        }

        float deltaWeight = targetWeight - m_ChangedValue;
        Character.RootMotionWeight += deltaWeight;
        m_ChangedValue += deltaWeight;
    }


#if UNITY_EDITOR

    public override string Name => "RootMotion";
    public override ClipCapabilities Capabilities => ClipCapabilities.Resizable | ClipCapabilities.Mixable;
    public RootMotionClip(Track track, int frame) : base(track, frame) { }
#endif
}


[TrackGroup("Character"), ScriptGuid("828a8311b89bab843b655b0181bf41e4"), IconGuid("e28acf5dc5b2e3d4a97920bf4e831c87"), Ordered(4), Color(201, 060, 032)]
public class DrawAfterImageTrack : Track
{

#if UNITY_EDITOR

    public override Type ClipType => typeof(DrawAfterImageClip);
#endif
}

[ScriptGuid("828a8311b89bab843b655b0181bf41e4"), Color(201, 060, 032)]
public class DrawAfterImageClip : Clip
{
    [ShowInInspector]
    public string meshName;
    [ShowInInspector]
    public Material meshMat;
    [ShowInInspector]
    public LayerMask meshLayer;

    AfterImageController controller;
    AfterImageController.AfterImage afterImage;

    public override void Bind()
    {
        base.Bind();
        controller = Timeline.TimelinePlayer.GetComponent<AfterImageController>();
    }
    public override void OnEnable()
    {
        base.OnEnable();
        if (!controller) return;

        afterImage = controller.CreateAfterImage(meshName, meshMat, meshLayer);
        afterImage?.Start();
    }
    public override void OnDisable()
    {
        base.OnDisable();
        if (!controller) return;

        afterImage?.End();
        afterImage = null;
    }

#if UNITY_EDITOR

    public override ClipCapabilities Capabilities => base.Capabilities | ClipCapabilities.Resizable;
    public DrawAfterImageClip(Track track, int frame) : base(track, frame) { }
#endif
}


[TrackGroup("Character"), ScriptGuid("828a8311b89bab843b655b0181bf41e4"), IconGuid("76c9ab52db448f24ba098ad7d930fd96"), Ordered(4), Color(165, 032, 025)]
public class DamageZoneTrack : CharacterTrack
{

#if UNITY_EDITOR

    public override Type ClipType => typeof(DamageZoneClip);
    public override Clip AddClip(UnityEngine.Object referenceObject, int frame)
    {
        DamageZoneClip clip = new DamageZoneClip((referenceObject as GameObject).GetComponent<DamageZone>(), this, frame);
        m_Clips.Add(clip);
        return clip;
    }
    public override bool DragValid()
    {
        return UnityEditor.DragAndDrop.objectReferences.Length == 1 && UnityEditor.DragAndDrop.objectReferences[0] is GameObject gameObject && UnityEditor.EditorUtility.IsPersistent(gameObject) && gameObject.GetComponent<DamageZone>();
    }
#endif
}

[ScriptGuid("828a8311b89bab843b655b0181bf41e4"), Color(165, 032, 025)]
public class DamageZoneClip : CharacterClip
{
    [ShowInInspector]
    public int DamageValue;
    [ShowInInspector]
    public Vector2 DNVelocity;
    [ShowInInspector, OnValueChanged("OnClipChanged", "RebindTimeline")]
    public DamageZone DamageZonePrefab;
    [ShowInInspector, OnValueChanged("RebindTimeline")]
    public string SocketName;
    [ShowInInspector, HideIf("UseSelfTransform"), HorizontalGroup("Position")]
    public Vector3 PositionOffset;
    [ShowInInspector, HideIf("UseSelfTransform"), HorizontalGroup("Rotation")]
    public Vector3 RotationOffset;
    [ShowInInspector, HideIf("UseSelfTransform"), HorizontalGroup("Scale")]
    public Vector3 ScaleOffset;
    [ShowInInspector(1), OnValueChanged("RebindTimeline", "RepaintInspector")]
    public bool UseSelfTransform = true;
    [ShowInInspector(1), OnValueChanged("RebindTimeline")]
    public bool ShowDebug;

    [SplitLine(5)]
    [ShowInInspector(2), ShowIf("UseTimeScale")]
    public float TimeScaleDelay;
    [ShowInInspector(2), ShowIf("UseTimeScale")]
    public float TimeScale;
    [ShowInInspector(2), ShowIf("UseTimeScale")]
    public float TimeScaleDuration;
    [ShowInInspector(2), ShowIf("UseTimeScale")]
    public float TimeScaleBlendIn;
    [ShowInInspector(2), ShowIf("UseTimeScale")]
    public float TimeScaleBlendOut;
    [ShowInInspector(2), OnValueChanged("RebindTimeline", "RepaintInspector")]
    public bool UseTimeScale;

    [SplitLine(5)]
    [ShowInInspector(3), ShowIf("UseCameraImpulse")]
    public float ImpulseDelay;
    [ShowInInspector(3), ShowIf("UseCameraImpulse")]
    public Cinemachine.CinemachineImpulseDefinition.ImpulseTypes ImpulseType = Cinemachine.CinemachineImpulseDefinition.ImpulseTypes.Uniform;
    [ShowInInspector(3), ShowIf("UseCameraImpulse"), OnValueChanged("RepaintInspector")]
    public Cinemachine.CinemachineImpulseDefinition.ImpulseShapes ImpulseShape = Cinemachine.CinemachineImpulseDefinition.ImpulseShapes.Bump;
    [ShowInInspector(3), ShowIf("ShowImpulseCurve")]
    public AnimationCurve ImpulseCurve;
    [ShowInInspector(3), ShowIf("UseCameraImpulse")]
    public float ImpulseDuration = 0.1f;
    [ShowInInspector(3), ShowIf("UseCameraImpulse")]
    public Vector3 ImpulseVelocity;
    [ShowInInspector(3), OnValueChanged("RebindTimeline", "RepaintInspector")]
    public bool UseCameraImpulse;

    [SplitLine(5)]
    [ShowInInspector(4), ShowIf("UseAudio")]
    public float AudioDelay;
    [ShowInInspector(4), ShowIf("UseAudio")]
    public UnityEngine.AudioClip AudioClip;
    [ShowInInspector(4), ShowIf("UseAudio")]
    public float AudioVolume;
    [ShowInInspector(4), ShowIf("UseAudio")]
    public float AudioSpeed;
    [ShowInInspector(4), ShowIf("UseAudio")]
    public float AudioStartTime;
    [ShowInInspector(5), OnValueChanged("RebindTimeline", "RepaintInspector")]
    public bool UseAudio;

    [SplitLine(5)]
    [ShowInInspector(5), ShowIf("UseForce")]
    public float ForceDelay;
    [ShowInInspector(5), ShowIf("UseForce")]
    public AddForceTrigger.ForceDirectionType ForceDirectionType;
    [ShowInInspector(5), ShowIf("UseForce")]
    public Vector3 Force;
    [ShowInInspector(5), OnValueChanged("RebindTimeline", "RepaintInspector")]
    public bool UseForce;

    public DamageZone DamageZoneInstance { get; private set; }

    public override void OnEnable()
    {
        Instantiate();
        if (Character && DamageZoneInstance)
            DamageZoneInstance.Init(Character);
    }
    public override void OnDisable()
    {
        Destroy();
    }

    void Instantiate()
    {
        if (DamageZonePrefab)
        {
            Transform socketTransform = Timeline.TimelinePlayer.transform;
            var childTransforms = Timeline.TimelinePlayer.GetComponentsInChildren<Transform>();
            foreach (var childTransform in childTransforms)
            {
                if (childTransform.name == SocketName)
                {
                    socketTransform = childTransform;
                    break;
                }
            }

            DamageZoneInstance = UnityEngine.Object.Instantiate(DamageZonePrefab, socketTransform, false);
            DamageZoneInstance.Value = DamageValue;
            DamageZoneInstance.DNVelocity = DNVelocity;

            if (!UseSelfTransform)
            {
                DamageZoneInstance.transform.localScale = ScaleOffset;
                DamageZoneInstance.transform.localPosition = PositionOffset;
                DamageZoneInstance.transform.localEulerAngles = RotationOffset;
            }

            if (ShowDebug)
                DamageZoneInstance.Debug();

            if (UseTimeScale && DamageZoneInstance.TryGetComponent(out TimeScalerTrigger timeScalerTrigger))
            {
                timeScalerTrigger.Init(Character);
                timeScalerTrigger.Delay = TimeScaleDelay;
                timeScalerTrigger.Scale = TimeScale;
                timeScalerTrigger.Duration = TimeScaleDuration;
                timeScalerTrigger.BlendIn = TimeScaleBlendIn;
                timeScalerTrigger.BlendOut = TimeScaleBlendOut;
            }
            if (UseCameraImpulse && DamageZoneInstance.TryGetComponent(out CameraImpluseTrigger cameraImpluseTrigger))
            {
                cameraImpluseTrigger.Init(Character);
                cameraImpluseTrigger.Delay = ImpulseDelay;
                cameraImpluseTrigger.ImpulseType = ImpulseType;
                cameraImpluseTrigger.ImpulseShape = ImpulseShape;
                cameraImpluseTrigger.ImpulseCurve = ImpulseCurve;
                cameraImpluseTrigger.ImpulseDuration = ImpulseDuration;
                cameraImpluseTrigger.ImpulseVelocity = ImpulseVelocity;
            }
            if (UseAudio && DamageZoneInstance.TryGetComponent(out PlayAudioTrigger playAudioTrigger))
            {
                playAudioTrigger.Init(Character);
                playAudioTrigger.Delay = AudioDelay;
                playAudioTrigger.Clip = AudioClip;
                playAudioTrigger.Volume = AudioVolume;
                playAudioTrigger.Speed = AudioSpeed;
                playAudioTrigger.StartTime = AudioStartTime;
            }
            if (UseForce && DamageZoneInstance.TryGetComponent(out AddForceTrigger addForceTrigger))
            {
                addForceTrigger.Init(Character);
                addForceTrigger.Delay = ForceDelay;
                addForceTrigger.DirectionType = ForceDirectionType;
                addForceTrigger.Force = Force;
            }
        }
    }
    void Destroy()
    {
        if (DamageZoneInstance)
        {
            UnityEngine.Object.DestroyImmediate(DamageZoneInstance.gameObject);
            DamageZoneInstance = null;
        }
    }

#if UNITY_EDITOR

    public override string Name => DamageZonePrefab ? DamageZonePrefab.name : base.Name;
    public override ClipCapabilities Capabilities => ClipCapabilities.Resizable;
    public DamageZoneClip(Track track, int frame) : base(track, frame) { }
    public DamageZoneClip(DamageZone damageZone, Track track, int frame) : base(track, frame)
    {
        DamageZonePrefab = damageZone;
    }

    [Button("Record"), HideIf("UseSelfTransform"), ShowIf("ShowRecord"), HorizontalGroup("Position")]
    void RecordPosition()
    {
        if (DamageZoneInstance)
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(Track.Timeline, $"Timeline: RecordPosition");
            PositionOffset = DamageZoneInstance.transform.localPosition;
            UnityEditor.EditorUtility.SetDirty(Track.Timeline);
        }
    }
    [Button("Record"), HideIf("UseSelfTransform"), ShowIf("ShowRecord"), HorizontalGroup("Rotation")]
    void RecordRotation()
    {
        if (DamageZoneInstance)
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(Track.Timeline, $"Timeline: RecordRotation");
            RotationOffset = DamageZoneInstance.transform.localEulerAngles;
            UnityEditor.EditorUtility.SetDirty(Track.Timeline);
        }
    }
    [Button("Record"), HideIf("UseSelfTransform"), ShowIf("ShowRecord"), HorizontalGroup("Scale")]
    void RecordScale()
    {
        if (DamageZoneInstance)
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(Track.Timeline, $"Timeline: RecordScale");
            ScaleOffset = DamageZoneInstance.transform.localScale;
            UnityEditor.EditorUtility.SetDirty(Track.Timeline);
        }
    }

    bool ShowRecord()
    {
        return DamageZoneInstance;
    }

    bool ShowImpulseCurve()
    {
        return UseCameraImpulse && ImpulseShape == Cinemachine.CinemachineImpulseDefinition.ImpulseShapes.Custom;
    }

    void OnClipChanged()
    {
        OnNameChanged?.Invoke();
    }
#endif
}
using System.Collections.Generic;
using Taco.Timeline;
using UnityEngine;
using UnityEngine.Events;

public class PlatformTimelinePlayer : TimelinePlayer
{
    #region Stuck Frame
    [Header("===== Stuck Frame =====")]
    public int FrameRate;

    public Transform[] BoneRoots;
    public List<Transform> Bones;

    public Transform[] DummyBoneRoots;
    public List<Transform> DummyBones;

    [ContextMenu("FindBones")]
    public void FindBones()
    {
        Bones = new List<Transform> ();
        DummyBones = new List<Transform> ();

        foreach (Transform b in BoneRoots)
        {
            Bones.AddRange (b.GetComponentsInChildren<Transform>());
        }
        foreach (Transform b in DummyBoneRoots)
        {
            DummyBones.AddRange (b.GetComponentsInChildren<Transform>());
        }
    }

    [Space(10)]
    public UnityEvent HalfFrameEvent;
    #endregion

    public uint Frame { get; private set; }

    public List<(Transform, Transform)> BonePairs;

    public PlatformCharacter PlatformCharacter;

    protected override void OnEnable()
    {
        base.OnEnable();
        Application.targetFrameRate = TimelineUtility.FrameRate;
    }
    protected override void OnDisable()
    {
        base.OnDisable();

    }

    public override void Init()
    {
        base.Init();

        BonePairs = new List<(Transform, Transform)>();
        for (int i = 0; i < Bones.Count; i++)
        {
            BonePairs.Add((Bones[i], DummyBones[i]));
        }
    }
    public override void Evaluate(float deltaTime)
    {
        base.Evaluate(deltaTime);

        if (FrameRate == 0)
        {
            SyncBones();
        }
        else
        {
            int targetInterval = Mathf.RoundToInt((float)TimelineUtility.FrameRate / FrameRate);
            int halfInterval = Mathf.RoundToInt((float)TimelineUtility.FrameRate / FrameRate / 2);

            if (halfInterval == 0 || Frame % halfInterval == 0)
            {
                HalfFrameEvent?.Invoke();
            }

            if (Frame % targetInterval == 0)
            {
                SyncBones();
            }

            Frame++;
            if (Frame == uint.MaxValue)
                Frame = 0;
        }
    }
    public override void AddTimeline(Timeline timeline)
    {
        base.AddTimeline(timeline);
        Frame = 0;
    }

    protected override void OnRootMotion()
    {

    }

    void SyncBones()
    {
        foreach (var bonePair in BonePairs)
        {
            bonePair.Item2.rotation = bonePair.Item1.rotation;
            bonePair.Item2.position = bonePair.Item1.position;
        }
    }

    public void ResetFrame()
    {
        Frame = 0;
    }
}

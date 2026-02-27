using System;
using System.Collections.Generic;
using UnityEngine;
using Taco.Gameplay;

public class AbilityRunner
{
    public HashSet<Ability> Abilities = new HashSet<Ability>();
    public Dictionary<string, Ability> AbilityMap = new Dictionary<string, Ability>();
    public PlatformCharacter Owner;

    public event Action<Ability> OnAbilityStart;
    public event Action<Ability> OnAbilitySop;

    bool m_Starting;
    public bool Starting 
    {
        get => m_Starting;
        set
        {
            m_Starting = value;
            if (StartingBuffer.Count > 0)
                StartingBuffer.Dequeue().Invoke();
        }
    }
    public Queue<Action> StartingBuffer = new Queue<Action>();

    bool m_Stoping;
    public bool Stoping
    {
        get => m_Stoping;
        set
        {
            m_Stoping = value;
            if (StopingBuffer.Count > 0)
                StopingBuffer.Dequeue().Invoke();
        }
    }
    public Queue<Action> StopingBuffer = new Queue<Action>();


    public List<string> ActiveTags = new List<string>();
    public List<string> BlockAbilitiesWithTag = new List<string>();
    public List<string> CanBufferAbilitiesTag = new List<string>();

    public List<Ability> BufferedAbilities = new List<Ability>();

    public virtual void Init(PlatformCharacter owner)
    {
        Abilities.Clear();
        AbilityMap.Clear();
        Owner = owner;
    }
    public virtual void Dispose()
    {
        foreach (var ability in Abilities)
        {
            TryStopAbility(ability);
            ability.DisposeTree();
        }
        Abilities.Clear();
        AbilityMap.Clear();
        Owner = null;
    }

    public virtual void AddAbility(Ability ability)
    {
        if (!Abilities.Contains(ability))
        {
            ability.InitTree(this);
            Abilities.Add(ability);
            AbilityMap.Add(ability.name, ability);
        }
    }
    public virtual void RemoveAbility(Ability ability)
    {
        if (Abilities.Contains(ability))
        {
            ability.DisposeTree();
            Abilities.Remove(ability);
            AbilityMap.Remove(ability.name);
        }
    }


    public void AddToBuffer(Ability abilityToBuffer)
    {
        foreach (var tag in CanBufferAbilitiesTag)
        {
            if (abilityToBuffer.AbilityTags.IsChildOf(tag))
            {
                if(!BufferedAbilities.Contains(abilityToBuffer))
                    BufferedAbilities.Add(abilityToBuffer);
                break;
            }
        }
    }

    public virtual bool TryStartAbility(string name)
    {
        if(AbilityMap.TryGetValue(name, out Ability abilityToStart))
        {
            return TryStartAbility(abilityToStart);
        }
        return false;
    }
    public virtual bool TryStartAbility(Ability abilityToStart)
    {
        if (Starting)
        {
            StartingBuffer.Enqueue(() => TryStartAbility(abilityToStart));
            return false;
        }

        Starting = true;

        foreach (var requiredTag in abilityToStart.RequiredTags.Tags)
        {
            bool isChild = false;
            foreach (var activeTag in ActiveTags)
            {
                if (activeTag.StartTagIs(requiredTag))
                {
                    isChild = true;
                    break;
                }
            }
            if (!isChild)
            {
                Starting = false;
                AddToBuffer(abilityToStart);
                Debug.Log($"{abilityToStart} required tag {requiredTag}");
                return false;
            }
        }

        foreach (var blockTag in BlockAbilitiesWithTag)
        {
            if (abilityToStart.AbilityTags.IsChildOf(blockTag))
            {
                Starting = false;
                AddToBuffer(abilityToStart);
                Debug.Log($"{abilityToStart} is blocked by tag {blockTag}");
                return false;
            }
        }

        foreach (var ability in Abilities)
        {
            if (ability.Active && abilityToStart.AbilityTags.PartChildOf(ability.BlockAbilitiesWithTag))
            {
                Starting = false;
                AddToBuffer(abilityToStart);
                Debug.Log($"{abilityToStart} is blocked by {ability}");
                return false;
            }
        }

        if (!abilityToStart.CanStart())
        {
            Starting = false;
            AddToBuffer(abilityToStart);
            Debug.Log($"{abilityToStart} can't start");
            return false;
        }

        foreach (var ability in Abilities)
        {
            if (ability.Active)
            {
                if (ability.AbilityTags.PartChildOf(abilityToStart.CancelAbilitiesWithTag))
                {
                    ability.CancelAbility(abilityToStart);
                    TryStopAbility(ability);

                    Debug.Log($"{ability} is canceld by {abilityToStart}");
                    break;
                }
            }
        }

        BufferedAbilities.Clear();
        abilityToStart.StartAbility();
        OnAbilityStart?.Invoke(abilityToStart);

        Starting = false;

        return true;
    }


    public virtual void TryStopAbility(string name)
    {
        if (AbilityMap.TryGetValue(name, out Ability abilityToStop))
        {
            TryStopAbility(abilityToStop);
        }
    }
    public virtual void TryStopAbility(Ability abilityToStop)
    {
        if (Stoping)
        {
            StopingBuffer.Enqueue(() => TryStopAbility(abilityToStop));
            return;
        }

        Stoping = true;
        if (abilityToStop.Active)
        {
            abilityToStop.StopAbility();
            OnAbilitySop?.Invoke(abilityToStop);
        }
        Stoping = false;
    }

    public virtual void Update(float deltaTime)
    {
        for (int i = BufferedAbilities.Count - 1; i >= 0; i--)
        {
            Ability ability = BufferedAbilities[i];
            if (TryStartAbility(ability))
                break;
        }

        foreach (var ability in Abilities)
        {
            if (ability.Active)
            {
                ability.UpdateAbility(deltaTime);
            }
            else
            {
                ability.InactiveUpdate();
            }
        }
    }
}

public interface IAbilityRunnerOwner
{
    public AbilityRunner AbilityRunner { get; set; }
}
using UnityEngine;
using TreeDesigner;

public class AddFlowTrigger : TriggerBase
{
    [Space(10), Header("===== Custom =====")]
    public float Duration;

    protected override void DoAction(HitBox hitBox)
    {
        if (Duration >0 && hitBox.Source.AbilityRunner.AbilityMap.TryGetValue("Ability_Flow", out Ability abilityFlow))
        {
            abilityFlow.GetExposedProperty<FloatExposedProperty>("ContinousDuration")?.SetValue(Duration);
            hitBox.Source.AbilityRunner.TryStartAbility(abilityFlow);
        }
    }
}

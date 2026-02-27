using UnityEngine;
using EasyCharacterMovement;

public class AbilityRunnerComponent : MonoBehaviour
{
    public IAbilityRunnerOwner AbilityRunnerOwner;
    public AbilityRunner AbilityRunner => AbilityRunnerOwner.AbilityRunner;

    public void Start()
    {
        AbilityRunnerOwner = GetComponent<Character>() as IAbilityRunnerOwner;
    }
}

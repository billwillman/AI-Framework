using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HitBox : MonoBehaviour
{
    public PlatformCharacter Source;

    public UnityEvent OnTriggered;

    public void OnHit(int value, Vector2 dnVelocity)
    {
        OnTriggered?.Invoke();
        PopupTextManager.Instance.SpawnPopup(value, Source.transform.position + Vector3.up, Source.transform, dnVelocity);
    }

    public virtual void AddExternalForce(Vector3 force)
    {
        if (Source.TryGetComponent(out PlatformCharacter character))
        {
            character.AddExternalForce(force);
        }
    }
}
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class DamageZone : MonoBehaviour
{
    public int Value;
    public Vector2 DNVelocity;

    [Space(20), Header("===== Events ====="), Space(20)]
    public UnityEvent<HitBox> OnTriggered;

    public PlatformCharacter Source { get; private set; }
    public int Direction => transform.forward == Vector3.right ? 1 : -1;

    private void OnTriggerEnter(Collider other)
    {
        HitBox hitBox = other.GetComponent<HitBox>();
        if (hitBox == null) return;
        if (hitBox.Source == Source) return;

        hitBox.OnHit(Value, new Vector2(DNVelocity.x * Direction, DNVelocity.y));
        OnTriggered.Invoke(hitBox);
    }

    public void Init(PlatformCharacter source)
    {
        Source = source;
    }
    public void Debug()
    {
        if (GetComponent<MeshRenderer>() is MeshRenderer meshRenderer)
            meshRenderer.enabled = true;
    }
}
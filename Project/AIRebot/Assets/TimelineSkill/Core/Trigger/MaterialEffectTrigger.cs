using DG.Tweening;
using UnityEngine;

public class MaterialEffectTrigger : MonoBehaviour
{
    public Renderer Renderer;
    [ColorUsage(true,true)]
    public Color TargetColor;
    public float TweenDuration;
    public float KeepDuration;

    Color originalColor;
    Sequence sequence;

    private void Start()
    {
        originalColor = Renderer.material.GetColor("_Emissive_Color");
    }
    public void Tween()
    {
        sequence?.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(Renderer.material.DOColor(TargetColor, "_Emissive_Color", TweenDuration));
        sequence.Append(Renderer.material.DOColor(originalColor, "_Emissive_Color", TweenDuration).SetDelay(KeepDuration));
    }
}

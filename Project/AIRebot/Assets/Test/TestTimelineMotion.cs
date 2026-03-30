using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTimelineMotion : MonoBehaviour
{
    public Animancer.PlayableAssetTransitionAsset Asset = null;

    private void Start() {
        if (Asset != null) {
            var controller = GetComponent<Animancer.AnimancerComponent>();
            if (controller != null) {
                controller.PlayTimeline(Asset);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityTimelineTreeTempPlayableBehaviourMgr : SingetonMono<UnityTimelineTreeTempPlayableBehaviourMgr>
{
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private Dictionary<int, UnityTimelineTreeTempPlayableBehaviour> TempMap = new Dictionary<int, UnityTimelineTreeTempPlayableBehaviour>();

    public void Register(UnityTimelineTreeTempPlayableBehaviour tempBehaviour) {
        if (tempBehaviour == null)
            return;
        int gameObjectInstanceID = tempBehaviour.CachedGameObject.GetInstanceID();
        TempMap[gameObjectInstanceID] = tempBehaviour;
    }

    public void UnRegister(UnityTimelineTreeTempPlayableBehaviour tempBehaviour) {
        if (tempBehaviour == null)
            return;
        int gameObjectInstanceID = tempBehaviour.CachedGameObject.GetInstanceID();
        if (TempMap.ContainsKey(gameObjectInstanceID))
            TempMap.Remove(gameObjectInstanceID);
    }

    public UnityTimelineTreeTempPlayableBehaviour GetTempPlayableBehaviour(GameObject gameObject) {
        if (gameObject == null || TempMap == null)
            return null;
        int gameObjectInstanceID = gameObject.GetInstanceID();
        UnityTimelineTreeTempPlayableBehaviour ret;
        if (TempMap.TryGetValue(gameObjectInstanceID, out ret))
            return ret;
        return null;
    }
}

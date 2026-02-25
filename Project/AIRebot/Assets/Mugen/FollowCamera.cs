using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Fighter m_Target = null;

    public Vector2 m_CenterOffset = Vector2.zero;

    private static FollowCamera m_FollowCamera = null;

    public static FollowCamera Instance {
        get {
            return m_FollowCamera;
        }
    }

    private void Awake() {
        m_FollowCamera = this;
    }

    void OnFree() {
        if (m_FollowCamera == this)
            m_FollowCamera = null;
    }

    private void OnDestroy() {
        OnFree();
    }

    public Vector2 m_FollowSpeed = Vector2.zero;
    public float m_SmoothTime = 0.2f;

    private void Update() {
        if (m_Target != null) {
            var targetPt = m_Target.transform.position;
            targetPt.y = 0f;

            var trans = this.transform;
            var pt = trans.position;

            var newPt = Vector2.SmoothDamp(pt, targetPt + new Vector3(m_CenterOffset.x, m_CenterOffset.y, 0f), ref m_FollowSpeed, m_SmoothTime);
            pt.x = newPt.x;
            pt.y = newPt.y;

            trans.position = pt;


        }
    }

    private void OnApplicationQuit() {
        OnFree();
    }
}

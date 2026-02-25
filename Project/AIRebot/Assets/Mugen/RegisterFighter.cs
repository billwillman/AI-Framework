using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamType
{
    MyTeam,
    EmeryTeam
}

[RequireComponent(typeof(CnsCommander))]
public class RegisterFighter : MonoBehaviour
{
    public int ID = ImageAnimation._cNoVaildState;
    public TeamType Team = TeamType.MyTeam;
    private CnsCommander m_Owner = null;

    public CnsCommander Owner {
        get {
            if (m_Owner == null)
                m_Owner = GetComponent<CnsCommander>();
            return m_Owner;
        }
    }

    private void Awake() {
        RegisterFighterMgr.Instance.Register(this);
    }

    private void OnDestroy() {
        if (ResourceMgr.Instance.IsQuitApp || RegisterFighterMgr.IsDestroy)
            return;
        RegisterFighterMgr.Instance.UnRegister(this);
    }
}

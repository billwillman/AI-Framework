using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterFighterMgr : SingetonMono<RegisterFighterMgr>
{
    private static int _cFigherID = 0;
    private Dictionary<int, RegisterFighter> m_FightMap = new Dictionary<int, RegisterFighter>();

    private static int GenFighterID() {
        return ++_cFigherID;
    }

    public RegisterFighter GetFighter(int ID) {
        if (ID == ImageAnimation._cNoVaildState)
            return null;
        RegisterFighter ret;
        if (!m_FightMap.TryGetValue(ID, out ret))
            ret = null;
        return ret;
    }

    public void Register(RegisterFighter fighter) {
        if (fighter == null)
            return;
        if (fighter.ID == ImageAnimation._cNoVaildState) {
            fighter.ID = GenFighterID();
        }
        m_FightMap[fighter.ID] = fighter;
    }

    public void UnRegister(RegisterFighter fighter) {
        if (fighter == null || fighter.ID == ImageAnimation._cNoVaildState)
            return;
        if (m_FightMap.ContainsKey(fighter.ID))
            m_FightMap.Remove(fighter.ID);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMgr : SingetonMono<ProjectileMgr>
{
    private Dictionary<int, Projectile> m_ProjectileMap = new Dictionary<int, Projectile>();
    public List<Projectile> m_ProjectileList = new List<Projectile>();

    protected override void Awake() {
        m_ProjectileList.Clear();
        m_ProjectileMap.Clear();
    }

    public void AddProjectile(int projID, Projectile target) {
        if (target == null || projID == ImageAnimation._cNoVaildState)
            return;
        if (!m_ProjectileMap.ContainsKey(projID))
            m_ProjectileList.Add(target);
        m_ProjectileMap[projID] = target;
    }

    public void RemoveProjectile(int projID) {
        Projectile p;
        if (m_ProjectileMap.TryGetValue(projID, out p) && p != null) {
            m_ProjectileList.Remove(p);
        }
    }
}

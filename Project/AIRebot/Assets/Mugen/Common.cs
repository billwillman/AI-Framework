using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using behaviac;

public static class Common {

   // private static Dictionary<string, Dictionary<int, BehaviorTree>> m_CacheBTreeMap = new Dictionary<string, Dictionary<int, BehaviorTree>>();

    // 战斗场景清理
    public static void FightSceneCleanup() {
     //   m_CacheBTreeMap.Clear();
       // Workspace.Instance.Cleanup();
        SoundManager.ClearAll();
    }

    /*
    public static void InitBehavic() {
        Workspace.Instance.FilePath = "assets/resources/BehavicTree";
        Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;
    }
    */

    public static string GetBehavicPlayerName(string playerName) {
        string ret = playerName.Replace('-', '_');
        return ret;
    }

    public static Projectile CreateBehaviourProjectile(string playerName) {
        GameObject gameObj = CreatePlayer(playerName, false, false, false);
        Projectile ret = gameObj.AddComponent<Projectile>();
        return ret;
    }

    public static RegisterFighter GetFighter(int ID) {
        return RegisterFighterMgr.Instance.GetFighter(ID);
    }

    public static void ApplyPosOffset(TProjPostype posType, GameObject ctl, CnsCommander owner, CnsCommander target, Vector2Int offset) {
        Vector2Int off = offset;
        off.y = -off.y;
        switch (posType) {
            case TProjPostype.p1:
                if (owner != null) {
                    var pt = ctl.transform.position;
                    var ownerPt = owner.transform.position;
                    off.x = owner.IsFlipX ? -off.x : off.x;
                    pt.x = ownerPt.x + ((float)off.x) / CnsCommander._cPosPer;
                    pt.y = ownerPt.y + ((float)off.y) / CnsCommander._cPosPer;
                    ctl.transform.position = pt;
                }
                break;
            case TProjPostype.p2:
                if (target != null) {
                    var pt = ctl.transform.position;
                    var ownerPt = target.transform.position;
                    off.x = target.IsFlipX ? -off.x : off.x;
                    pt.x = ownerPt.x + ((float)off.x) / CnsCommander._cPosPer;
                    pt.y = ownerPt.y + ((float)off.y) / CnsCommander._cPosPer;
                    ctl.transform.position = pt;
                }
                break;
            case TProjPostype.left: {
                    if (owner != null) {
                        var ownerPt = owner.transform.position;
                        float x = Common.GetCameraScreenLeft(ownerPt.z);
                        Vector3 pt = new Vector3(x + off.x, ownerPt.y + off.y, ownerPt.z);
                        ctl.transform.position = pt;
                    }
                }
                break;
            case TProjPostype.right: {
                    if (owner != null) {
                        var ownerPt = owner.transform.position;
                        float x = Common.GetCameraScreenRight(ownerPt.z);
                        Vector3 pt = new Vector3(x - off.x, ownerPt.y + off.y, ownerPt.z);
                        ctl.transform.position = pt;
                    }
                }
                break;
            case TProjPostype.front: {
                    if (owner != null) {
                        var ownerPt = owner.transform.position;
                        float x = owner.IsFlipX ? Common.GetCameraScreenLeft(ownerPt.z) + off.x : Common.GetCameraScreenRight(ownerPt.z) - off.x;
                        Vector3 pt = new Vector3(x, ownerPt.y + off.y, ownerPt.z);
                        ctl.transform.position = pt;
                    }
                }
                break;
            case TProjPostype.back: {
                    if (owner != null) {
                        var ownerPt = owner.transform.position;
                        float x = owner.IsFlipX ? Common.GetCameraScreenRight(ownerPt.z) - off.x : Common.GetCameraScreenLeft(ownerPt.z) + off.x;
                        Vector3 pt = new Vector3(x, ownerPt.y + off.y, ownerPt.z);
                        ctl.transform.position = pt;
                    }
                }
                break;
        }
    }

    public static Projectile CreateProjectile(string playerName, ProjectileCreator creator) {
        GameObject gameObj = CreatePlayer(playerName, false, false, false);
        Projectile ret = gameObj.AddComponent<Projectile>();
        ret.Attach(creator);
        
        return ret;
    }

    public static Explod CreateExplod(string playerName, ExplodCreator creator) {
        GameObject gameObj = CreatePlayer(playerName, false, false, false);
        Explod ret = gameObj.AddComponent<Explod>();
        ret.Attach(creator);
        return ret;
    }

    public static GameObject CreatePlayer(string playerName, bool isCameraFollow = false, bool isRegisterFighter = false, bool playDefAnim = true) {
        GameObject gameObj = new GameObject(playerName);
        gameObj.AddComponent<SpriteRenderer>();
        gameObj.AddComponent<ImageAnimation>();
        gameObj.AddComponent<Animation>();
        gameObj.AddComponent<PlayerLoader>();
        // 人物属性都在这里
        gameObj.AddComponent<Fighter>();
        gameObj.AddComponent<Movement>();

        gameObj.AddComponent<CnsCommander>();
        gameObj.AddComponent<AudioSource>();
        //gameObj.AddComponent<LuaComponent>();

        if (isRegisterFighter)
            gameObj.AddComponent<RegisterFighter>();

        InitPlayer(gameObj, playerName, isCameraFollow, playDefAnim);

        return gameObj;
    }

    private static void InitPlayer(GameObject gameObj, string playerName, bool isCameraFollow = false, bool playDefAnim = true) {
        var anim = gameObj.GetComponent<ImageAnimation>();
        var playerLoader = gameObj.GetComponent<PlayerLoader>();
        playerLoader.AttachPlayer(playerName);
        playerLoader.SwitchPallet(0);
        if (playDefAnim)
            anim.PlayerPlayerAni(0, true);
        var fighter = gameObj.GetComponent<Fighter>();
        var localAnim = gameObj.GetComponent<Animation>();
        localAnim.animatePhysics = true;

        if (isCameraFollow) {
            var followCam = FollowCamera.Instance;
            if (followCam != null) {
                followCam.m_Target = fighter;
            }
        }

    }

    public static GameObject CreateBehaviourPlayer(string playerName, bool isCameraFollow = false) {
        string fileName = StringHelper.Format("resources/BehavicTree/{0}.prefab", playerName);
        GameObject ret = ResourceMgr.Instance.CreateGameObject(fileName);
        if (ret != null) {
            if (ret.GetComponent<RegisterFighter>() == null)
                ret.AddComponent<RegisterFighter>();
        }
        InitPlayer(ret, playerName, isCameraFollow);
        return ret;
    }

    public static float GetCameraScreenLeft(float z) {
        var cam = Camera.main;
        if (cam == null)
            return 0f;
        Vector3 pt = new Vector3(-((float)Screen.width) / 2.0f, 0, z);
        Vector3 ret = cam.ScreenToWorldPoint(pt);
        return ret.x;
    }

    public static float GetCameraScreenRight(float z) {
        var cam = Camera.main;
        if (cam == null)
            return 0f;
        Vector3 pt = new Vector3(((float)Screen.width) / 2.0f, 0, z);
        Vector3 ret = cam.ScreenToWorldPoint(pt);
        return ret.x;
    }
}

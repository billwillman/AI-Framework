using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mugen
{
    public class PlayerAnimation
    {
        private bool m_IsVaild;
        public PlayerAnimation(string cfg) {
            AirCfgLoader airLoader = new AirCfgLoader(cfg);
            m_IsVaild = airLoader.IsVaild;
            if (m_IsVaild)
                m_IsVaild = LoadAir(airLoader);
        }

        public bool IsVaild {
            get {
                return m_IsVaild;
            }
        }

        private bool LoadAir(AirCfgLoader airLoader) {
            mStateAniMap.Clear();
            if (airLoader == null || !airLoader.IsVaild)
                return false;
            for (int i = 0; i < airLoader.GetStateCount(); ++i) {
                var state = airLoader.GetStateByIndex(i);
                if (state == AirCfgLoader._cNoVaildState)
                    continue;
                BeginAction beginAction = airLoader.GetBeginAction(state);
                if (beginAction == null || beginAction.ActionFrameListCount <= 0) {
#if DEBUG
                    Debug.LogErrorFormat("beginAction :{0:D} Failed~!", state);
#endif
                    continue;
                }

                List<ImageAnimateNode> aniNodeList;
                if (!mStateAniMap.TryGetValue(state, out aniNodeList)) {
                    aniNodeList = new List<ImageAnimateNode>();
                    mStateAniMap.Add(state, aniNodeList);
                }

                ActionFlip lastFlip = ActionFlip.afNone;
                for (int frame = 0; frame < beginAction.ActionFrameListCount; ++frame) {
                    ActionFrame actFrame;
                    if (beginAction.GetFrame(frame, out actFrame)) {
                        if (actFrame.Index >= 0) {
                            int frameIndex = actFrame.Index;
                            ImageAnimateNode aniNode = new ImageAnimateNode();
                            aniNode.AniTick = actFrame.Tick;
                            aniNode.flipTag = actFrame.Flip;
                            aniNode.drawMode = actFrame.DrawMode;
                            //aniNode.flipTag = lastFlip;
                            lastFlip = actFrame.Flip;
                            aniNode.frameIndex = frameIndex;
                            aniNode.frameGroup = actFrame.Group;
                            aniNode.actionNo = state;
                            aniNode.isLoopStart = actFrame.IsLoopStart;         
                          //  if (aniNode.isLoopStart)
                          //      Debug.LogFormat("HasLoopStart: ActionNo. {0:D}", i);
                            aniNode.defaultClsn2Arr = actFrame.defaultClsn2Arr;
                            aniNode.localCls1Arr = actFrame.localCls1Arr;
                            aniNode.localClsn2Arr = actFrame.localClsn2Arr;
                            aniNodeList.Add(aniNode);
                        }
                    }
                }

            }
            return true;
        }

        public Dictionary<int, List<ImageAnimateNode>> AnimMap {
            get {
                return mStateAniMap;
            }
        }

        private Dictionary<int, List<ImageAnimateNode>> mStateAniMap = new Dictionary<int, List<ImageAnimateNode>>();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mugen
{

    

    public struct ActionFrame
    {
        public int Group;
        public int Index;
        public int Tick;
        public ActionFlip Flip;
        public ActionDrawMode DrawMode;
        public bool IsLoopStart;
        // 防御盒(暂时不用)
        public RectF[] localClsn2Arr;
        public RectF[] defaultClsn2Arr;
        public RectF[] localCls1Arr;
    }


    public class BeginAction
    {
        private static readonly string _cClsn2Default = "Clsn2Default:";
        private static readonly string _cClsn2 = "Clsn2:";
        private static readonly string _cClsn1 = "Clsn1:";

        private static readonly string _cClsn2DKeyName = "Clsn2";
        private static readonly string _cClsn1DKeyName = "Clsn1";

        private bool ReadClsn(out RectF[] clsn2DArr, ConfigSection section, ref int aniStartIdx, string clsnName, string clsnKeyName) {
            clsn2DArr = null;
            if (string.IsNullOrEmpty(clsnName))
                return false;
            string str = section.GetContent(aniStartIdx);
            if (string.IsNullOrEmpty(str)) {
                clsn2DArr = null;
                return false;
            }

            bool ret = false;
            if (str.StartsWith(clsnName)) {
                string defClsStr = str.Substring(clsnName.Length).Trim();
                int defClsCnt;
                if (!int.TryParse(defClsStr, out defClsCnt)) {
                    clsn2DArr = null;
                    return false;
                }
                if (defClsCnt > 0) {
                    clsn2DArr = new RectF[defClsCnt];
                    for (int i = aniStartIdx + 1; i <= aniStartIdx + defClsCnt; ++i) {
                        string key;
                        string value;
                        if (section.GetKeyValue(i, out key, out value)) {
                            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value)) {
                                int idx = key.IndexOf(clsnKeyName, StringComparison.CurrentCultureIgnoreCase);
                                if (idx >= 0) {
                                    key = key.Substring(idx + clsnKeyName.Length);
                                    int startIdx = key.IndexOf("[");
                                    int endIdx = key.IndexOf("]");
                                    if (startIdx >= 0 && endIdx >= 0 && endIdx > startIdx + 1) {
                                        string idxStr = key.Substring(startIdx + 1, endIdx - startIdx - 1);
                                        if (!string.IsNullOrEmpty(idxStr)) {
                                            idxStr = idxStr.Trim();
                                            if (!string.IsNullOrEmpty(idxStr)) {
                                                int index = int.Parse(idxStr);
                                                if (index >= 0 && index < clsn2DArr.Length) {
                                                    string[] values = value.Split(ConfigSection._cContentArrSplit, StringSplitOptions.RemoveEmptyEntries);
                                                    if (values != null && values.Length > 0) {
                                                        RectF r = new RectF();
                                                        if (values.Length >= 4) {
                                                            string v = values[0].Trim();
                                                            int left = int.Parse(v);
                                                            v = values[1].Trim();
                                                            int top = int.Parse(v);
                                                            v = values[2].Trim();
                                                            int right = int.Parse(v);
                                                            v = values[3].Trim();
                                                            int bottom = int.Parse(v);
                                                            r.min = new Vector2(left, top);
                                                            r.max = new Vector2(right, bottom);
                                                        }
                                                        clsn2DArr[index] = r;
                                                        ret = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                    aniStartIdx += defClsCnt + 1;
                }
            }

            if (!ret) {
                clsn2DArr = null;
            }

            return ret;
        }

        private int GetIntFromStr(string str, int def = 0) {
            if (string.IsNullOrEmpty(str))
                return def;
            str = str.Trim();
            if (string.IsNullOrEmpty(str) || str.Length <= 0)
                return def;
            if (str[str.Length - 1] == '.') {
                str = str.Substring(0, str.Length - 1);
                if (string.IsNullOrEmpty(str) || str.Length <= 0)
                    return def;
            }
            int ret = int.Parse(str);
            return ret;
        }

        public BeginAction(/*PlayerState state, */ConfigSection section) {
            //this.State = state;
            if (section != null) {
                int aniStartIdx = 0;
                RectF[] clsn2DefaultArr = null;

                ReadClsn(out clsn2DefaultArr, section, ref aniStartIdx, _cClsn2Default, _cClsn2DKeyName);


                string str = section.GetContent(aniStartIdx);
                RectF[] frameClsn2 = null;
                RectF[] frameClsn1 = null;
                if (!string.IsNullOrEmpty(str)) {
                    List<string> arr = new List<string>();
                    bool isLoopStart = false;
                    int i = aniStartIdx;
                    while (i < section.ContentListCount) {
                        arr.Clear();

                        string line = section.GetContent(i);
                        if (string.IsNullOrEmpty(line)) {
                            ++i;
                            continue;
                        }

                        if (line.StartsWith("Loopstart", StringComparison.CurrentCultureIgnoreCase)) {
                            isLoopStart = true;
                            ++i;
                            continue;
                        }

                        // 说明需要设置默认clsDefault
                        if (line.StartsWith(_cClsn2Default)) {
                            aniStartIdx = i;
                            if (ReadClsn(out clsn2DefaultArr, section, ref aniStartIdx, _cClsn2Default, _cClsn2DKeyName))
                                i = aniStartIdx;
                            else
                                ++i;
                            continue;
                        }

                        if (line.StartsWith(_cClsn2)) {
                            aniStartIdx = i;
                            if (ReadClsn(out frameClsn2, section, ref aniStartIdx, _cClsn2, _cClsn2DKeyName))
                                i = aniStartIdx;
                            else
                                ++i;
                            continue;
                        }

                        if (line.StartsWith(_cClsn1)) {
                            aniStartIdx = i;
                            if (ReadClsn(out frameClsn1, section, ref aniStartIdx, _cClsn1, _cClsn1DKeyName))
                                i = aniStartIdx;
                            else
                                ++i;
                            continue;
                        }

                        if (section.GetArray(i, arr)) {
                            if (arr.Count >= 5) {
                                int ImageGroup = GetIntFromStr(arr[0], -1);
                                int ImageIndex = GetIntFromStr(arr[1], -1);
                                int Tick = int.Parse(arr[4].Trim());
                                bool hasFlip = arr.Count >= 6;
                                ActionFlip flipMode = ActionFlip.afNone;
                                if (hasFlip) {
                                    string flipStr = arr[5].Trim();
                                    if (string.Compare(flipStr, "H", true) == 0)
                                        flipMode = ActionFlip.afH;
                                    else if (string.Compare(flipStr, "V", true) == 0)
                                        flipMode = ActionFlip.afV;
                                    else if (string.Compare(flipStr, "HV", true) == 0 ||
                                             string.Compare(flipStr, "VH", true) == 0)
                                        flipMode = ActionFlip.afHV;
                                }
                                bool hasDrawMode = arr.Count >= 7;
                                ActionDrawMode drawMode = ActionDrawMode.adNone;
                                if (hasDrawMode) {
                                    string drawStr = arr[6].Trim();
                                    if (string.Compare(drawStr, "A", true) == 0)
                                        drawMode = ActionDrawMode.ad_A;
                                    else if (string.Compare(drawStr, "S", true) == 0)
                                        drawMode = ActionDrawMode.ad_S;
                                    else if (string.Compare(drawStr, "A1", true) == 0)
                                        drawMode = ActionDrawMode.ad_A1;
                                }

                                ActionFrame frame = new ActionFrame();
                                frame.Group = ImageGroup;
                                frame.Index = ImageIndex;
                                frame.Tick = Tick;
                                frame.Flip = flipMode;
                                frame.DrawMode = drawMode;
                                if (aniStartIdx > 0 && clsn2DefaultArr != null) {
                                    frame.defaultClsn2Arr = clsn2DefaultArr;
                                    clsn2DefaultArr = null;
                                }
                                if (frameClsn2 != null) {
                                    frame.localClsn2Arr = frameClsn2;
                                    frameClsn2 = null;
                                }
                                if (frameClsn1 != null) {
                                    frame.localCls1Arr = frameClsn1;
                                    frameClsn1 = null;
                                }
                                if (isLoopStart) {
                                    frame.IsLoopStart = isLoopStart;
                                    isLoopStart = false;
                                }
                                ActionFrameList.Add(frame);
                            }
                        }
                        ++i;
                    }
                }
            }
        }


        public int ActionFrameListCount {
            get {
                if (mActionFrameList == null)
                    return 0;
                return mActionFrameList.Count;
            }
        }

        public bool GetFrame(int index, out ActionFrame frame) {
            frame = new ActionFrame();
            if (mActionFrameList == null)
                return false;
            if ((index < 0) || (index >= mActionFrameList.Count))
                return false;
            frame = mActionFrameList[index];
            return true;
        }

        protected List<ActionFrame> ActionFrameList {
            get {
                if (mActionFrameList == null)
                    mActionFrameList = new List<ActionFrame>();
                return mActionFrameList;
            }
        }

        private List<ActionFrame> mActionFrameList = null;
    }

    public class AirCfgLoader
    {
        public AirCfgLoader(string text) {
            mIsVaild = LoadPlayer(text);
        }

        private bool LoadPlayer(string text) {
            if (string.IsNullOrEmpty(text))
                return false;

            ConfigReader reader = new ConfigReader();
            reader.LoadString(text);

            bool ret = LoadFromReader(reader);

            return ret;
        }

        private bool LoadFromReader(ConfigReader reader) {
            if (reader == null)
                return false;

            // load Begin Action
            for (int i = 0; i < reader.SectionCount; ++i) {
                ConfigSection section = reader.GetSections(i);
                if (section == null)
                    continue;
                string tile = section.Tile;
                if (tile.StartsWith(_cBeginAction, StringComparison.CurrentCultureIgnoreCase)) {
                    string stateStr = tile.Substring(_cBeginAction.Length).Trim();
                    short state;
                    if (short.TryParse(stateStr, out state) && (state >= 0) /*&& (state < (int)PlayerState.psPlayerStateCount)*/) {
                        BeginAction action = new BeginAction(/*playerState,*/ section);
                        AddOrSetBeginAction(state, action);
                    }
                }
            }
            return true;
        }

        protected void AddOrSetBeginAction(short state, BeginAction action) {
            if (action == null)
                return;
            if (mBeginActionMap != null && mBeginActionMap.ContainsKey(state)) {
                mBeginActionMap[state] = action;
            } else {
                if (mBeginActionMap == null)
                    mBeginActionMap = new Dictionary<short, BeginAction>();
                mBeginActionMap.Add(state, action);
                if (mBeginActionList == null)
                    mBeginActionList = new List<short>();
                mBeginActionList.Add(state);
            }
        }

        public BeginAction GetBeginAction(short state) {
            if (mBeginActionMap == null)
                return null;
            BeginAction ret;
            if (!mBeginActionMap.TryGetValue(state, out ret))
                ret = null;
            return ret;
        }

        public int GetStateCount() {
            if (mBeginActionList != null)
                return mBeginActionList.Count;
            /*
            if (mStrBeginActionList != null)
                return mStrBeginActionList.Count;
            */
            return 0;
        }

        public short GetStateByIndex(int index) {
            if (mBeginActionList != null) {
                if (index >= 0 && index < mBeginActionList.Count)
                    return mBeginActionList[index];
                return _cNoVaildState;
            }

            return _cNoVaildState;
        }

        /*
        public string GetStrStateByIndex(int index) {
            if (mStrBeginActionList != null) {
                if (index >= 0 && index < mStrBeginActionList.Count)
                    return mStrBeginActionList[index];
            }

            return string.Empty;
        }
        */

        public bool IsVaild {
            get {
                return mIsVaild;
            }
        }

        public static readonly short _cNoVaildState = ImageAnimation._cNoVaildState;
        private static readonly string _cBeginAction = "Begin Action";
        private bool mIsVaild = false;
        private Dictionary<short, BeginAction> mBeginActionMap = null;
        private List<short> mBeginActionList = null;
       // private List<string> mStrBeginActionList = null;
    }

}


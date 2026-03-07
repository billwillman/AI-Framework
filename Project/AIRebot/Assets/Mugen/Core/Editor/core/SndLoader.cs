using System.Collections;
using System.Collections.Generic;
using System.IO;
using Utils;
using UnityEngine;

namespace Mugen
{

    public struct SndFileHeader
    {
        public string tile;
        public uint version;
        public uint soudNum;
        public uint firstSoundOffset;

        public bool LoadFromStream(Stream stream) {
            if (stream == null || stream.Length <= 0)
                return false;
            var mgr = FilePathMgr.Instance;
            tile = mgr.ReadString(stream, 12);
            version = (uint)mgr.ReadInt(stream);
            soudNum = (uint)mgr.ReadInt(stream);
            firstSoundOffset = (uint)mgr.ReadInt(stream);
            return true;
        }
    }

    public struct SndSubFileHeader
    {
        public uint NextSoundOffset;
        public uint wavBuffSize;
        public int group;
        public int index;
        //public byte[] wavBuf;

        public bool LoadFromStream(Stream stream) {
            if (stream == null || stream.Length <= 0)
                return false;
            var mgr = FilePathMgr.Instance;
            NextSoundOffset = (uint)mgr.ReadInt(stream);
            wavBuffSize = (uint)mgr.ReadInt(stream);
            group = mgr.ReadInt(stream);
            index = mgr.ReadInt(stream);
            return true;
        }
    }

    public class SndLoader
	{
        private bool LoadFromBuffer(byte[] buffer) {
            if (buffer == null || buffer.Length <= 0)
                return false;

            MemoryStream stream = new MemoryStream(buffer);
            try {
                SndFileHeader header = new SndFileHeader();
                if (!header.LoadFromStream(stream))
                    return false;
                stream.Seek(header.firstSoundOffset, SeekOrigin.Begin);
                for (int i = 0; i < header.soudNum; ++i) {
                    SndSubFileHeader subHeader = new SndSubFileHeader();
                    if (!subHeader.LoadFromStream(stream))
                        return false;

                    if (subHeader.wavBuffSize > 0) {
                        byte[] buf = new byte[subHeader.wavBuffSize];
                        stream.Read(buf, 0, buf.Length);
                        if (m_SoundBufMap == null)
                            m_SoundBufMap = new Dictionary<KeyValuePair<int, int>, byte[]>();
                        KeyValuePair<int, int> key = new KeyValuePair<int, int>(subHeader.group, subHeader.index);
                        m_SoundBufMap[key] = buf;
                    }

                    stream.Seek(subHeader.NextSoundOffset, SeekOrigin.Begin);
                }

                return m_SoundBufMap != null && m_SoundBufMap.Count == header.soudNum;

            } finally {
                stream.Close();
                stream.Dispose();
            }
        }

        public SndLoader(byte[] buffer) {
            LoadFromBuffer(buffer);
        }

        public int SoundCount {
            get {
                if (m_SoundBufMap == null)
                    return 0;
                return m_SoundBufMap.Count;
            }
        }

        public Dictionary<KeyValuePair<int, int>, byte[]>.Enumerator GetSoundIter() {
            if (m_SoundBufMap == null)
                return new Dictionary<KeyValuePair<int, int>, byte[]>.Enumerator();
            return m_SoundBufMap.GetEnumerator();
        }

        private Dictionary<KeyValuePair<int, int>, byte[]> m_SoundBufMap = null;

    }
}

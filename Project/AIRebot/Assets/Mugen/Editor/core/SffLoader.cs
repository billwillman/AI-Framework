using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Utils;
using NsTcpClient;
using UnityEngine.Experimental.Rendering;
using Mugen;

[System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
public struct SFFHEADER
{

    /// unsigned char[11]
    [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 12)]
    public string signature;

    /// unsigned char
    public byte verhi;

    /// unsigned char
    public byte verlo;

    /// unsigned char
    public byte verhi2;

    /// unsigned char
    public byte verlo2;

    /// unsigned int
    public uint NumberOfGroups;

    /// unsigned int
    public uint NumberOfImage;

    /// unsigned int
    public uint SubHeaderFileOffset;

    /// unsigned int
    public uint SizeOfSubheader;

    /// unsigned char
    public byte PaletteType;

    /// unsigned char[476]
    //[System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst=476)]
    //public string BLANK;
};

[System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
public struct PCXHEADER
{

    public static PCXHEADER LoadFromStream(Stream stream)
    {
        PCXHEADER header = new PCXHEADER();
        if (stream == null)
        {
            return header;
        }
        var mgr = FilePathMgr.GetInstance();
        header.Manufacturer = (byte)stream.ReadByte();
        header.Version = (byte)stream.ReadByte();
        header.Encoding = (byte)stream.ReadByte();
        header.BPP = (byte)stream.ReadByte();
        header.x = (ushort)mgr.ReadShort(stream);
        header.y = (ushort)mgr.ReadShort(stream);
        header.widht = (ushort)mgr.ReadShort(stream);
        header.height = (ushort)mgr.ReadShort(stream);
        header.HRES = (ushort)mgr.ReadShort(stream);
        header.VRES = (ushort)mgr.ReadShort(stream);
        header.ColorMap = mgr.ReadString(stream, 48, System.Text.Encoding.ASCII);
        header.reserved1 = (byte)stream.ReadByte();
        header.NPlanes = (byte)stream.ReadByte();
        header.bytesPerLine = (byte)stream.ReadByte();
        header.palletInfo = (byte)stream.ReadByte();
        header.HorzScreenSize = (ushort)mgr.ReadShort(stream);
        header.VertScreenSize = (ushort)mgr.ReadShort(stream);
        header.Reserved2 = mgr.ReadString(stream, 54, System.Text.Encoding.ASCII);
        return header;
    }

    public bool IsPng
    {
        get
        {
            return NPlanes == 0;
        }
    }

    /// unsigned char
    public byte Manufacturer;

    /// unsigned char
    public byte Version;

    /// unsigned char
    public byte Encoding;

    /// unsigned char
    public byte BPP;

    /// unsigned short
    public ushort x;

    /// unsigned short
    public ushort y;

    /// unsigned short
    public ushort widht;

    /// unsigned short
    public ushort height;

    /// unsigned short
    public ushort HRES;

    /// unsigned short
    public ushort VRES;

    /// unsigned char[48]
    [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 48)]
    public string ColorMap;

    /// unsigned char
    public byte reserved1;

    /// unsigned char
    public byte NPlanes;

    /// unsigned char
    public byte bytesPerLine;

    /// unsigned char
    public byte palletInfo;

    public ushort HorzScreenSize;
    public ushort VertScreenSize;

    /// unsigned char[58]
    [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 54)]
    public string Reserved2;
};

// 2.0 文件头
[System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
public struct SFFHEADERv2
{
    /// unsigned char[11]
    [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 12)]
    public string signature;

    /// unsigned char
    public byte verhi;

    /// unsigned char
    public byte verlo;

    /// unsigned char
    public byte verhi2;

    /// unsigned char
    public byte verlo2;

    public uint reserved1;
    public uint reserved2;

    // compatVerLoad
    public byte compatverlo3;
    public byte compatverlo1;
    public byte compatverlo2;
    public byte compatverhi;

    public uint reserved3;
    public uint reserved4;

    public uint offsetSubFile;
    public uint totalImage;

    public uint offsetPaletteFile;
    public uint totalPalette;

    public uint offsetLData;
    public uint sizeLData;

    public uint offsetTData;
    public uint sizeTData;

    public uint reserved5;
    public uint reserved6;

    /// unsigned char[436]
    [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 436)]
    public string comments;

    public static SFFHEADERv2 LoadFromStream(Stream stream)
    {
        SFFHEADERv2 ret = new SFFHEADERv2();
        if (stream == null)
            return ret;
        var mgr = FilePathMgr.GetInstance();

        ret.signature = mgr.ReadString(stream, 12, System.Text.Encoding.UTF8);
        ret.verhi = (byte)stream.ReadByte();
        ret.verlo = (byte)stream.ReadByte();
        ret.verhi2 = (byte)stream.ReadByte();
        ret.verlo2 = (byte)stream.ReadByte();
        ret.reserved1 = (uint)mgr.ReadInt(stream);
        ret.reserved2 = (uint)mgr.ReadInt(stream);
        ret.compatverlo3 = (byte)stream.ReadByte();
        ret.compatverlo1 = (byte)stream.ReadByte();
        ret.compatverlo2 = (byte)stream.ReadByte();
        ret.compatverhi = (byte)stream.ReadByte();
        ret.reserved3 = (uint)mgr.ReadInt(stream);
        ret.reserved4 = (uint)mgr.ReadInt(stream);
        ret.offsetSubFile = (uint)mgr.ReadInt(stream);
        ret.totalImage = (uint)mgr.ReadInt(stream);
        ret.offsetPaletteFile = (uint)mgr.ReadInt(stream);
        ret.totalPalette = (uint)mgr.ReadInt(stream);
        ret.offsetLData = (uint)mgr.ReadInt(stream);
        ret.sizeLData = (uint)mgr.ReadInt(stream);
        ret.offsetTData = (uint)mgr.ReadInt(stream);
        ret.sizeTData = (uint)mgr.ReadInt(stream);
        ret.reserved5 = (uint)mgr.ReadInt(stream);
        ret.reserved6 = (uint)mgr.ReadInt(stream);
        ret.comments = mgr.ReadString(stream, 436, System.Text.Encoding.UTF8);

        return ret;
    }
};

public enum SffVersion
{
    none,
    v1,
    v2
};

public struct PCXDATA
{
    public byte[] data;
    public byte[] pallet;
    public KeyValuePair<short, short> palletLink;

    public bool IsVaildPalletLink {
        get {
            return palletLink.Key >= 0 && palletLink.Value >= 0;
        }
    }
};

[System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
public struct SFFSUBHEADER
{

    public static SFFSUBHEADER LoadFromStream(Stream stream)
    {
        var mgr = FilePathMgr.GetInstance();
        SFFSUBHEADER subHeader = new SFFSUBHEADER();

        subHeader.NextSubheaderFileOffset = (uint)mgr.ReadInt(stream);
        subHeader.LenghtOfSubheader = (uint)mgr.ReadInt(stream);
        subHeader.x = mgr.ReadShort(stream);
        subHeader.y = mgr.ReadShort(stream);
        subHeader.GroubNumber = mgr.ReadShort(stream);
        subHeader.ImageNumber = mgr.ReadShort(stream);
        subHeader.IndexOfPrevious = mgr.ReadShort(stream);
        subHeader.PalletSame = mgr.ReadBool(stream);
        subHeader.BALNK = mgr.ReadString(stream, 13, System.Text.Encoding.ASCII);

        return subHeader;
    }

    /// unsigned int
    public uint NextSubheaderFileOffset;

    /// unsigned int
    public uint LenghtOfSubheader;

    /// short
    public short x;

    /// short
    public short y;

    /// short
    public short GroubNumber;

    /// short
    public short ImageNumber;

    /// short
    public short IndexOfPrevious;

    /// boolean
    [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
    public bool PalletSame;

    /// unsigned char[13]
    [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 13)]
    public string BALNK;
};

public struct SffTexture
{
    public short group, image;
    public Texture indexTexture;
    public short linkPalletGroup, linkPalletIndex;
    public byte[] localPalletData;
    public float offsetX, offsetY;

    public bool UseGlobalPallet {
        get {
            return ((linkPalletGroup < 0) || (linkPalletIndex < 0)) && (localPalletData == null);
        }
    }

    public bool UseLinkPallet {
        get {
            return ((linkPalletGroup >= 0) && (linkPalletIndex >= 0));
        }
    }
}


public unsafe class SffLoader
{

    private static readonly string _cElecbyteSpr = "ElecbyteSpr";
    private bool mIsVaild = false;
    private SffVersion mVersion = SffVersion.none;
    private Dictionary<KeyValuePair<uint, uint>, KeyValuePair<PCXHEADER, PCXDATA>> mPcxDataMap = new Dictionary<KeyValuePair<uint, uint>, KeyValuePair<PCXHEADER, PCXDATA>>();
    private List<SFFSUBHEADER> mSubHeaders = null;

    private SffTexture GenSffTextue(KeyValuePair<uint, uint> Key, KeyValuePair<PCXHEADER, PCXDATA> Value, bool isReadAble = false) {
        SffTexture sffTex = new SffTexture();
        sffTex.group = (short)Key.Key;
        sffTex.image = (short)Key.Value;

        SFFSUBHEADER h;
        if (!GetSubHeader(sffTex.group, sffTex.image, out h))
            return new SffTexture();

        float offX = ((float)(Value.Key.x + h.x)) / (float)Value.Key.widht;//+ 1.0f;
        float offY = -((float)(Value.Key.y + h.y)) / (float)Value.Key.height + 1.0f;

        sffTex.offsetX = offX;
        sffTex.offsetY = offY;

        if (Value.Key.IsPng) {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false, false);
            tex.LoadImage(Value.Value.data);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.Apply(false, !isReadAble);
            sffTex.indexTexture = tex;

            sffTex.linkPalletGroup = -1;
            sffTex.linkPalletIndex = -1;
            sffTex.localPalletData = null;
        } else {
            Texture2D tex = new Texture2D(Value.Key.widht, Value.Key.height, TextureFormat.Alpha8, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.LoadRawTextureData(Value.Value.data);
            tex.Apply(false, !isReadAble);
            sffTex.indexTexture = tex;

            if (Value.Value.IsVaildPalletLink) {
                sffTex.linkPalletGroup = Value.Value.palletLink.Key;
                sffTex.linkPalletIndex = Value.Value.palletLink.Value;
            } else {
                sffTex.linkPalletGroup = -1;
                sffTex.linkPalletIndex = -1;
            }

            sffTex.localPalletData = Value.Value.pallet;

        }

        return sffTex;
    }

    public void ForEachPcx(Action<SffTexture> onCallBack, bool isReadAble = false) {
        if (onCallBack == null)
            return;
        var iter = mPcxDataMap.GetEnumerator();
        while(iter.MoveNext()) {
            SffTexture sffTex = GenSffTextue(iter.Current.Key, iter.Current.Value, isReadAble);
            onCallBack(sffTex);
        }
        iter.Dispose();
    }

    public PCXDATA GetPcxData(short group, short image) {
        KeyValuePair<PCXHEADER, PCXDATA> value;
        if (mPcxDataMap.TryGetValue(new KeyValuePair<uint, uint>((uint)group, (uint)image), out value))
            return value.Value;
        return new PCXDATA(); ;
    }

    protected List<SFFSUBHEADER> SubHeaders
    {
        get
        {
            if (mSubHeaders == null)
                mSubHeaders = new List<SFFSUBHEADER>();
            return mSubHeaders;
        }
    }

    public SffLoader(byte[] bytes)
    {
        Load(bytes);
    }

    
    /// <summary>
    /// 获得所有Textures
    /// </summary>
    /// <returns></returns>
    public List<SffTexture> GetTextures()
    {
        if (!mIsVaild)
            return null;
        List<SffTexture> ret = null;
        var iter = mPcxDataMap.GetEnumerator();
        while (iter.MoveNext())
        {
            if (ret == null)
                ret = new List<SffTexture>();

            var sffTex = GenSffTextue(iter.Current.Key, iter.Current.Value);
            ret.Add(sffTex);
        }
        iter.Dispose();
        return ret;
    }

    private bool LoadV1(byte[] bytes)
    {
        byte v1;
        byte v2;
        byte v3;
        byte v4;

        SFFHEADER header = new SFFHEADER();
        int headerSize = Marshal.SizeOf(header);
        IntPtr headerBuffer = Marshal.AllocHGlobal(headerSize);
        try
        {
            Marshal.Copy(bytes, 0, headerBuffer, headerSize);
            header = (SFFHEADER)Marshal.PtrToStructure(headerBuffer, typeof(SFFHEADER));
        }
        finally
        {
            Marshal.FreeHGlobal(headerBuffer);
        }

        if (string.Compare(header.signature, _cElecbyteSpr, true) != 0)
            return false;

        v1 = header.verlo2;
        v2 = header.verlo;
        v3 = header.verhi2;
        v4 = header.verhi;

        if (v1 > 1)
        {
            Debug.LogErrorFormat("sff file not supoort v{0:D}.{1:D}.{2:D}.{3:D}", v1, v2, v3, v4);
            return false;
        }

        //  MemoryStream stream = new MemoryStream(bytes);
        try
        {
            if (!LoadSubFiles(header, bytes))
                return false;
            // if (!LoadSubFiles(header, stream))
            //      return false;

            if (!LoadPcxs(header, bytes))
                return false;
        }
        finally
        {
            //         stream.Close();
            //         stream.Dispose();
        }

        return true;

    }

    public bool GetSubHeader(int group, int image, out SFFSUBHEADER header)
    {
        if ((mSubHeaders == null) || (group < 0) || (image < 0))
        {
            header = new SFFSUBHEADER();
            return false;
        }

        if (mSubHeaders != null)
            for (int i = 0; i < mSubHeaders.Count; ++i)
            {
                SFFSUBHEADER sub = mSubHeaders[i];

                int g = (int)sub.GroubNumber;
                int img = (int)sub.ImageNumber;
                if (g != group || img != image)
                    continue;

                while (true)
                {
                    if ((sub.LenghtOfSubheader == 0) && (sub.IndexOfPrevious >= 0) && (sub.IndexOfPrevious < mSubHeaders.Count))
                    {
                        sub = mSubHeaders[sub.IndexOfPrevious];
                    }
                    else
                        break;
                }

                header = sub;
                return true;
            }

        header = new SFFSUBHEADER();
        return false;
    }

    public bool GetSubHeader(int index, out SFFSUBHEADER header)
    {
        if ((mSubHeaders == null) || (index < 0) || (index >= mSubHeaders.Count))
        {
            header = new SFFSUBHEADER();
            return false;
        }

        header = mSubHeaders[index];

        while (true)
        {
            if ((header.LenghtOfSubheader == 0) && (header.IndexOfPrevious > 0) && (header.IndexOfPrevious < mSubHeaders.Count))
            {
                header = mSubHeaders[header.IndexOfPrevious - 1];
            }
            else
                break;
        }
        return true;
    }

    private bool LoadPcxs(SFFHEADER sffHeader, byte[] source)
    {
        if ((source == null) || (source.Length <= 0))
            return false;
        if ((mSubHeaders == null) || (mSubHeaders.Count <= 0))
            return true;

        bool ret = true;
        int offset = (int)sffHeader.SubHeaderFileOffset;
        for (int i = 0; i < mSubHeaders.Count; ++i)
        {
            SFFSUBHEADER header;
            if (!GetSubHeader(i, out header))
            {
                ret = false;
                break;
            }

            KeyValuePair<uint, uint> key = new KeyValuePair<uint, uint>((uint)header.GroubNumber, (uint)header.ImageNumber);
            if (mPcxDataMap.ContainsKey(key))
            {
                offset = (int)header.NextSubheaderFileOffset;
                if (offset == 0 || offset >= source.Length)
                    break;
                continue;
            }

            /*
            // 檢查indexPrevious
            if (header.LenghtOfSubheader == 0 && header.IndexOfPrevious != 0)
            {
                offset = (int)header.NextSubheaderFileOffset;
                if (offset == 0 || offset >= source.Length)
                    break;
                continue;
            }
             * */

            offset += Marshal.SizeOf(header);
            KeyValuePair<PCXHEADER, PCXDATA> value;
#if _USE_NEW_PCX
                Stream stream = null;
                try
                {
                    if (!LoadPcx2(offset, header, source, ref stream, out value))
#else
            if (!LoadPcx(offset, header, source, out value))
#endif
            {
                Debug.LogErrorFormat("LoadPcxs: index = {0} error", i);
                ret = false;
                break;
            }
#if _USE_NEW_PCX
                } finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                    }
                }
#endif


            mPcxDataMap.Add(key, value);

            offset = (int)header.NextSubheaderFileOffset;
            if (offset == 0 || offset >= source.Length)
                break;
        }

        return ret;
    }


    private byte[] mNormalPallet = null;
    protected bool HasNormalPallet
    {
        get
        {
            return (mNormalPallet != null);
        }
    }

    private byte[] DecodePcxData(int offset, PCXHEADER header, byte[] source)
    {
        byte[] ret = null;

        if ((offset < 0) || (offset >= source.Length))
            return ret;

        //int nTotalyByte = (int)(header.bytesPerLine * header.NPlanes);
        int bpp = (int)(header.NPlanes * 8);
        if (bpp > 8)
            return ret; // not support

        try
        {
            int width = header.widht;
            if (width < header.bytesPerLine * header.NPlanes)
                width = header.bytesPerLine * header.NPlanes;

            int size = 0;
            int Pos = 0;
            //ret = new byte[header.widht * header.NPlanes * header.height + 1];
            ret = new byte[header.widht * header.NPlanes * header.height];
            bool isEnd = false;
            for (int y = 0; y < header.height; ++y)
            {
                if (isEnd)
                    break;

                int x = 0;
                while (x < width)
                {
                    int idx = offset + Pos++;
                    if (idx >= source.Length)
                    {
                        isEnd = true;
                        break;
                    }
                    byte byData = source[idx];
                    if ((byData & 0xC0) == 0xC0)
                    {
                        size = byData & 0x3F;
                        idx = offset + Pos++;
                        if (idx >= source.Length)
                        {
                            isEnd = true;
                            break;
                        }
                        byData = source[idx];
                    }
                    else
                    {
                        size = 1;
                    }

                    while (size-- > 0)
                    {
                        if (x <= header.widht)
                        {
                            idx = x + (y * header.widht * header.NPlanes);
                            if (idx >= ret.Length)
                                break;
                            ret[idx] = byData;
                        }
                        //this it to Skip blank data on PCX image wich are on the right side
                        // TODO:OK? Skip two bytes
                        if ((x == width) && (width != header.widht))
                        {
                            int nHowManyBlank = width - (int)header.widht;
                            for (int i = 0; i < nHowManyBlank; ++i)
                                Pos += 2;
                        }


                        x++;
                    }
                }
            }

            // H changed
            byte[] temp = new byte[header.widht];
            int lineSize = header.widht * header.NPlanes;
            for (int y = 0; y < (int)header.height / 2; ++y)
            {
                int x = ((int)header.height - 1 - y);
                int s = y * lineSize;
                int d = x * lineSize;
                Buffer.BlockCopy(ret, d, temp, 0, lineSize);
                Buffer.BlockCopy(ret, s, ret, d, lineSize);
                Buffer.BlockCopy(temp, 0, ret, s, lineSize);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }



        return ret;
    }

    private KeyValuePair<short, short> m_currentLink = new KeyValuePair<short, short>(-1, -1);
    private bool LoadPcx(int offset, SFFSUBHEADER subHeader, byte[] source, out KeyValuePair<PCXHEADER, PCXDATA> dataPair)
    {
        if ((offset < 0) || (offset >= source.Length))
        {
            dataPair = new KeyValuePair<PCXHEADER, PCXDATA>();
            return false;
        }

        PCXHEADER header = new PCXHEADER();
        int bufSize = Marshal.SizeOf(header);
        if (offset + bufSize > source.Length)
        {
            dataPair = new KeyValuePair<PCXHEADER, PCXDATA>();
            return false;
        }
        IntPtr buf = Marshal.AllocHGlobal(bufSize);
        try
        {
            Marshal.Copy(source, offset, buf, bufSize);
            header = (PCXHEADER)Marshal.PtrToStructure(buf, typeof(PCXHEADER));
        }
        finally
        {
            Marshal.FreeHGlobal(buf);
        }

        if (header.BPP == 0)
        {
            dataPair = new KeyValuePair<PCXHEADER, PCXDATA>();
            return true;
        }

        offset += bufSize;

        int pcxBufSz = (int)subHeader.LenghtOfSubheader - 127;
        if (pcxBufSz <= 0)
        {
            dataPair = new KeyValuePair<PCXHEADER, PCXDATA>();
            return true;
        }

        if (offset + pcxBufSz > source.Length)
        {
            dataPair = new KeyValuePair<PCXHEADER, PCXDATA>();
            return true;
        }
        byte[] pcxBuf = new byte[pcxBufSz];
        Buffer.BlockCopy(source, offset, pcxBuf, 0, pcxBufSz);
        offset += pcxBufSz;

        if (offset >= source.Length)
        {
            dataPair = new KeyValuePair<PCXHEADER, PCXDATA>();
            return false;
        }

        header.widht = (ushort)(header.widht - header.x + 1);
        header.height = (ushort)(header.height - header.y + 1);

        //byte[] dst = DecodePcxData(offset, header, pcxBuf);
        byte[] dst = DecodePcxData(0, header, pcxBuf);
        pcxBuf = null;
        if ((dst == null) || (dst.Length <= 0))
        {
            // empty
            dataPair = new KeyValuePair<PCXHEADER, PCXDATA>();
            return true;
        }


        PCXDATA pcxData = new PCXDATA();
        pcxData.data = dst;
        pcxData.pallet = null;

        // 判断是不是9000，1
        //	if (subHeader.GroubNumber == 9000 && subHeader.ImageNumber == 1) {
        //	pcxData.palletLink = m_currentLink;
        /*} else*/
        {
            offset -= 768;
            //eat empty 8bit
            offset++;

            byte s = source[offset++];
            if ((s == 12) && !subHeader.PalletSame && !HasNormalPallet && header.NPlanes <= 1)
            {           // if (!subHeader.PalletSame && !HasNormalPallet && header.NPlanes <= 1)
                        // load pallet
                pcxData.pallet = new byte[256 * 4];
                for (int i = 0; i < 256; ++i)
                {
                    byte r = source[offset++];
                    byte g = source[offset++];
                    byte b = source[offset++];
                    byte a;
                    if (i == 0)
                        a = 0;
                    else
                    {
                        int lastPalIdx = (i - 1) * 4;
                        // r, g, b, a
                        if ((r == pcxData.pallet[lastPalIdx++]) && (g == pcxData.pallet[lastPalIdx++]) && (b == pcxData.pallet[lastPalIdx++]))
                            a = 0;
                        else
                            a = 0xFF;
                    }

                    int palIdx = i * 4;
                    pcxData.pallet[palIdx++] = r;
                    pcxData.pallet[palIdx++] = g;
                    pcxData.pallet[palIdx++] = b;
                    pcxData.pallet[palIdx++] = a;
                }
                m_currentLink = new KeyValuePair<short, short>(subHeader.GroubNumber, subHeader.ImageNumber);
            }
            else
                pcxData.palletLink = m_currentLink;
        }

        dataPair = new KeyValuePair<PCXHEADER, PCXDATA>(header, pcxData);

        return true;
    }

    private bool LoadSubFiles(SFFHEADER header, byte[] source)
    {
        if ((header.NumberOfGroups == 0) && (header.NumberOfImage == 0))
            return true;

        if ((header.SubHeaderFileOffset == 0))
            return false;
        if (!LoadSubFiles((int)header.SubHeaderFileOffset, source))
            return false;
        bool ret = (int)header.NumberOfImage == mSubHeaders.Count;
        return ret;
    }

    private bool LoadSubFiles(int offset, byte[] source)
    {
        if (offset < 0)
            return false;
        SFFSUBHEADER header = new SFFSUBHEADER();
        int headerSize = Marshal.SizeOf(header);
        if (headerSize + offset > source.Length)
        {
            // File is Eof
            return true;
        }
        IntPtr headerBuf = Marshal.AllocHGlobal(headerSize);
        try
        {
            Marshal.Copy(source, offset, headerBuf, headerSize);
            header = (SFFSUBHEADER)Marshal.PtrToStructure(headerBuf, typeof(SFFSUBHEADER));
        }
        finally
        {
            Marshal.FreeHGlobal(headerBuf);
        }

        SubHeaders.Add(header);

        // load pcx
        /*
        KeyValuePair<PCXHEADER, PCXDATA> pcxData;
        if (!LoadPcx(offset, header, source, out pcxData))
            return false;
        KeyValuePair<short, short> key = new KeyValuePair<short, short>(header.GroubNumber, header.ImageNumber);
        */
        if (header.NextSubheaderFileOffset != 0)
        {
            if (header.NextSubheaderFileOffset >= source.Length)
                return true;
            if (!LoadSubFiles((int)header.NextSubheaderFileOffset, source))
                return false;
        }

        return true;
    }

    private unsafe bool Load(byte[] bytes)
    {
        mPcxDataMap.Clear();
        if ((bytes == null) || (bytes.Length <= 0))
        {
            mIsVaild = false;
            return false;
        }

        if (bytes.Length < 16)
        {
            mIsVaild = false;
            return false;
        }

        bool ret = false;

        byte v1 = bytes[15];
       
        if (v1 == 2)
        {
            mVersion = SffVersion.v2;
            ret = LoadV2(bytes);
        } else if (v1 == 1)
        {
            mVersion = SffVersion.v1;
            ret = LoadV1(bytes);
        }
        else
        {
            Debug.LogErrorFormat("sff file not supoort v{0:D}", v1);
        }

        mIsVaild = ret;

        return ret;
    }

    private bool LoadV2(byte[] bytes)
    {
        if (mSubHeaders != null)
            mSubHeaders.Clear();
        sff.sffReader reader = new sff.sffReader(bytes);
        bool ret = reader.RawForeachV2(OnSffReaderV2);
        return ret;
    }

    public int GetSubHeaderIndex(int group, int image)
    {
        if ((mSubHeaders == null) || (group < 0) || (image < 0))
            return -1;
        if (mSubHeaders != null)
        {
            for (int i = 0; i < mSubHeaders.Count; ++i)
            {
                SFFSUBHEADER sub = mSubHeaders[i];

                int g = (int)sub.GroubNumber;
                int img = (int)sub.ImageNumber;
                if (g != group || img != image)
                    continue;
                return i;
            }
        }
        return -1;
    }

    private void OnSffReaderV2(sff.sffReader reader, sff.sprMsgV2 spr, int linkGoup, int linkIndex, int linkPalGroup, int linkPalIndex, byte[] rawData)
    {
        bool isImageLink = linkGoup >= 0 && linkIndex >= 0;
        if (!isImageLink)
        {

            KeyValuePair<uint, uint> key = new KeyValuePair<uint, uint>((uint)spr.group, (uint)spr.index);
            if (mPcxDataMap.ContainsKey(key))
                return;

            PCXHEADER header = new PCXHEADER();
            header.widht = spr.width;
            header.height = spr.height;
            //	header.x = (ushort)spr.x;
            //header.y = (ushort)spr.y;
            header.x = 0;
            header.y = 0;
            header.NPlanes = 1;

            if (rawData != null && rawData.Length > 0)
            {
                if (spr.IsPng)
                {
                    header.NPlanes = 0;
                }
                else
                {
                    int chgSize = header.NPlanes * header.widht;

                    var bufferNode = NetByteArrayPool.GetByteBufferNode(chgSize);

                    byte[] temp = bufferNode.Buffer;
                    try
                    {
                        for (int y = 0; y < (int)header.height / 2; ++y)
                        {
                            int x = ((int)header.height - 1 - y);
                            int s = y * chgSize;
                            int d = x * chgSize;
                            Buffer.BlockCopy(rawData, d, temp, 0, chgSize);
                            Buffer.BlockCopy(rawData, s, rawData, d, chgSize);
                            Buffer.BlockCopy(temp, 0, rawData, s, chgSize);
                        }
                    }
                    finally
                    {
                        bufferNode.Dispose();
                    }
                }
            }

            PCXDATA data = new PCXDATA();
            data.data = rawData;
            bool isPalletLink = (linkPalGroup >= 0 && linkPalIndex >= 0) && ((linkPalGroup != spr.group) || (linkPalIndex != spr.index));
            if (!isPalletLink)
            {
                byte[] pal = reader.GetPal(spr.group, spr.index);
                //data.pallet = GetPalletFromByteArr (pal);
                data.pallet = pal;
                data.palletLink = new KeyValuePair<short, short>(-1, -1);
            }
            else
            {
                data.palletLink = new KeyValuePair<short, short>((short)linkPalGroup, (short)linkPalIndex);
                data.pallet = null;
            }

            KeyValuePair<PCXHEADER, PCXDATA> value = new KeyValuePair<PCXHEADER, PCXDATA>(header, data);
            mPcxDataMap.Add(key, value);
        }

        SFFSUBHEADER subHeader = new SFFSUBHEADER();
        subHeader.GroubNumber = (short)spr.group;
        subHeader.ImageNumber = (short)spr.index;
        subHeader.x = spr.x;
        subHeader.y = spr.y;

        if (isImageLink)
        {
            subHeader.IndexOfPrevious = (short)GetSubHeaderIndex(linkGoup, linkIndex);
            subHeader.LenghtOfSubheader = 0;
        }
        else
        {
            subHeader.IndexOfPrevious = -1;
        }

        SubHeaders.Add(subHeader);

    }
}

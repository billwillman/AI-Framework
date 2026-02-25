using System;
using UnityEngine;

// 调色版
public class PalletLoader
{
    private bool m_Use32Bit = true;

	private byte[] m_PalletData = null;

    public PalletLoader(byte[] buffer) {
		m_PalletData = GeneratorActBytes(buffer);
	}

	public Texture2D PalletTexture(bool isBit32) {
		return GeneratorPalletTexture(m_PalletData, isBit32);
	}

	public byte[] PalletData {
		get {
			return m_PalletData;
		}
	}

	private static byte[] GeneratorActBytes(byte[] actSource) {
		if ((actSource == null) || (actSource.Length <= 0))
			return null;
		int n = (int)(actSource.Length / 3);
		if (n <= 0)
			return null;

		byte[] pallet = new byte[n * 4];
		int offset = 0;
		for (int j = n - 1; j >= 0; --j) {
			byte r = actSource[offset++];
			byte g = actSource[offset++];
			byte b = actSource[offset++];
			byte a;
			if (j == 0)
				a = 0;
			else {
				int lastPalIdx = (j - 1) * 4;
				// r, g, b, a
				if ((r == pallet[lastPalIdx++]) && (g == pallet[lastPalIdx++]) && (b == pallet[lastPalIdx++]))
					a = 0;
				else
					a = 0xFF;
			}

			int palIdx = j * 4;
			pallet[palIdx++] = r;
			pallet[palIdx++] = g;
			pallet[palIdx++] = b;
			pallet[palIdx++] = a;
		}

		return pallet;
	}

	private static Texture2D GeneratorActTexture(byte[] actSource, bool is32Bit) {
		if ((actSource == null) || (actSource.Length <= 0))
			return null;
		int n = (int)(actSource.Length / 3);
		if (n <= 0)
			return null;

		byte[] pallet = new byte[n * 4];
		int offset = 0;
		for (int j = n - 1; j >= 0; --j) {
			byte r = actSource[offset++];
			byte g = actSource[offset++];
			byte b = actSource[offset++];
			byte a;
			if (j == 0)
				a = 0;
			else {
				int lastPalIdx = (j - 1) * 4;
				// r, g, b, a
				if ((r == pallet[lastPalIdx++]) && (g == pallet[lastPalIdx++]) && (b == pallet[lastPalIdx++]))
					a = 0;
				else
					a = 0xFF;
			}

			int palIdx = j * 4;
			pallet[palIdx++] = r;
			pallet[palIdx++] = g;
			pallet[palIdx++] = b;
			pallet[palIdx++] = a;
		}

		return GeneratorPalletTexture(pallet, is32Bit);
	}

	public Texture2D TranslateIndexTexture(Texture2D indexTexture, bool is32Bit) {
		if (indexTexture == null)
			return null;
		if (m_PalletData == null)
			return indexTexture;
		var indexData = indexTexture.GetRawTextureData();
		if (indexData == null || indexData.Length <= 0)
			return indexTexture;

		TextureFormat fmt;
		if (is32Bit)
			fmt = TextureFormat.RGBA32;
		else
			fmt = TextureFormat.ARGB4444;

		byte[] raw = new byte[indexTexture.width * indexTexture.height * (is32Bit ? 4 : 2)];
		for (int i = 0; i < indexData.Length; ++i) {
			int index = indexData[i];

			int srcIdx = index * 4;
			int r = m_PalletData[srcIdx++];
			int g = m_PalletData[srcIdx++];
			int b = m_PalletData[srcIdx++];
			int a = m_PalletData[srcIdx++];

			if (is32Bit) {
				raw[i * 4] = (byte)r;
				raw[i * 4 + 1] = (byte)g;
				raw[i * 4 + 2] = (byte)b;
				raw[i * 4 + 3] = (byte)a;
			} else {

				byte v = (byte)(((b & 0xF0) >> 4) & ((g & 0xF0)));
				raw[i * 2] = v;
				v = (byte)(((r & 0xF0) >> 4) & ((a & 0xF0)));
				raw[i * 2 + 1] = v;
			}
		}

		Texture2D ret = new Texture2D(indexTexture.width, indexTexture.height, fmt, false, false);
		ret.filterMode = FilterMode.Point;
		ret.wrapMode = TextureWrapMode.Clamp;
		ret.LoadRawTextureData(raw);
		ret.Apply();

		if (Application.isPlaying)
			GameObject.Destroy(indexTexture);
		else
			GameObject.DestroyImmediate(indexTexture);

		return ret;
	}

	public static bool GetPalletColor(int idx, byte[] pallet, out Color32 color) {
		if (pallet == null || idx < 0 || idx >= pallet.Length/4) {
			color = Color.clear;
			return false;
        }

		int srcIdx = idx * 4;
		int r = pallet[srcIdx++];
		int g = pallet[srcIdx++];
		int b = pallet[srcIdx++];
		int a = pallet[srcIdx++];

		color = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
		return true;
	}

	unsafe private static byte[] GeneratorPalletBytes(byte[] pallet, bool is32Bit) {
		if ((pallet == null) || (pallet.Length <= 0))
			return null;
		int palLen = (int)(pallet.Length / 4);
		byte[] raw;
		if (!is32Bit) {
			raw = new byte[pallet.Length * 2];

			for (int idx = 0; idx < palLen; ++idx) {
				int srcIdx = idx * 4;
				int rawIdx = idx * 2;
				int r = pallet[srcIdx++];
				int g = pallet[srcIdx++];
				int b = pallet[srcIdx++];
				int a = pallet[srcIdx++];

				byte v = (byte)(((b & 0xF0) >> 4) & ((g & 0xF0)));
				raw[rawIdx++] = v;
				v = (byte)(((r & 0xF0) >> 4) & ((a & 0xF0)));
				raw[rawIdx++] = v;
			}
		} else
			raw = pallet;
		return raw;
	}

	// pallet: RGBA32
	unsafe private static Texture2D GeneratorPalletTexture(byte[] pallet, bool is32Bit) {
		if ((pallet == null) || (pallet.Length <= 0))
			return null;
		TextureFormat fmt;
		if (is32Bit)
			fmt = TextureFormat.RGBA32;
		else
			fmt = TextureFormat.ARGB4444;

		int palLen = (int)(pallet.Length / 4);
		byte[] raw;
		if (!is32Bit) {
			raw = new byte[pallet.Length * 2];

			for (int idx = 0; idx < palLen; ++idx) {
				int srcIdx = idx * 4;
				int rawIdx = idx * 2;
				int r = pallet[srcIdx++];
				int g = pallet[srcIdx++];
				int b = pallet[srcIdx++];
				int a = pallet[srcIdx++];

				byte v = (byte)(((b & 0xF0) >> 4) & ((g & 0xF0)));
				raw[rawIdx++] = v;
				v = (byte)(((r & 0xF0) >> 4) & ((a & 0xF0)));
				raw[rawIdx++] = v;
			}
		} else
			raw = pallet;

		Texture2D ret = new Texture2D(palLen, 1, fmt, false, false);
		ret.filterMode = FilterMode.Point;
		ret.wrapMode = TextureWrapMode.Clamp;
		ret.LoadRawTextureData(raw);
		ret.Apply();

		return ret;
	}
}
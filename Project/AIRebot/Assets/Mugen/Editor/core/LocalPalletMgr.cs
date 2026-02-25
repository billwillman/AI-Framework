using System;
using System.Collections;
using System.Collections.Generic;

namespace Mugen
{
    public class LocalPalletMgr
    {
        private Dictionary<KeyValuePair<short, short>, PalletLoader> m_PalletMap = new Dictionary<KeyValuePair<short, short>, PalletLoader>();
        
        public void AddPallet(short group, short image, byte[] palletData) {
            PalletLoader palletLoader = new PalletLoader(palletData);
            KeyValuePair<short, short> key = new KeyValuePair<short, short>(group, image);
            m_PalletMap[key] = palletLoader;
        }
    }
}
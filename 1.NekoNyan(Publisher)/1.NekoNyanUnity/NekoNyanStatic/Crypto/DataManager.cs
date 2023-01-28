﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NekoNyanStatic.Crypto
{
    /// <summary>
    /// 数据库
    /// </summary>
    public class DataManager
    {
        private static Dictionary<string, CryptoVersion> msGameInfo = new(16)
        {
            { "Ao no Kanata no Four Rhythm PE", CryptoVersion.V10 },
            { "Ao no Kanata no Four Rhythm Extra 1", CryptoVersion.V10 },
            { "Kinkoi: Golden Loveriche", CryptoVersion.V10},
            { "Ao no Kanata no Four Rhythm Extra 2", CryptoVersion.V11},
        };

        /// <summary>
        /// 获取游戏信息
        /// </summary>
        public static Dictionary<string,CryptoVersion> SGameInformations
        {
            get
            {
                return msGameInfo;
            }
        }
        
    }
}

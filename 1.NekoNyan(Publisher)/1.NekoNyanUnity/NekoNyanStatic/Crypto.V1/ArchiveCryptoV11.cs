﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NekoNyanStatic.Crypto.V1
{
    /// <summary>
    /// 封包加密类V11
    /// </summary>
    internal class ArchiveCryptoV11 : ArchiveCryptoV10
    {
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Initialize()
        {
            this.mFileStream.Position = 0;

            //读取并解密原封包信息
            Span<byte> rawPkgInfo = stackalloc byte[1024];
            this.mFileStream.Read(rawPkgInfo);

            int fileCount = 0;              //文件个数
            uint rawFileEntryKey = 0;        //原始文件表Key
            uint rawFileNamesKey = 0;        //原始文件名Key

            //解密封包信息
            {
                Span<int> rawPkgInfoPack4 = MemoryMarshal.Cast<byte, int>(rawPkgInfo);
                //获得文件个数
                for (int i = 3; i < 255; ++i)
                {
                    fileCount += rawPkgInfoPack4[i];
                }
                //获得原始文件表Key
                rawFileEntryKey = (uint)rawPkgInfoPack4[53];
                //获得原始文件名Key
                rawFileNamesKey = (uint)rawPkgInfoPack4[23];
            }

            //读取并解密原文件表信息
            byte[] rawEntryData = new byte[16 * fileCount];
            this.mFileStream.Read(rawEntryData);
            this.Decrypt(rawEntryData, rawFileEntryKey);

            //读取并解密原文件名信息
            byte[] rawFileNamesData = new byte[BitConverter.ToInt32(rawEntryData, 12) - (1024 + rawEntryData.Length)];
            this.mFileStream.Read(rawFileNamesData);
            this.Decrypt(rawFileNamesData, rawFileNamesKey);

            this.ParserFileEntry(rawEntryData, rawFileNamesData, fileCount);
        }

        /// <summary>
        /// 生成key  256字节长度
        /// </summary>
        /// <param name="tablePtr">表指针</param>
        /// <param name="key">key</param>
        protected override void KeyGenerator(Span<byte> tablePtr, uint key)
        {
            uint k1 = key * 0x0000131C + 0x0000A740;
            uint k2 = k1 << 0x07 ^ k1;

            for (int i = 0; i < 256; ++i)
            {
                k1 = k1 - key + k2;
                k2 = k1 + 0x9C;
                k1 *= k2 & 0xCE;
                tablePtr[i] = (byte)k1;
                k1 >>= 3;
            }

        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">解密Key</param>
        protected override void Decrypt(Span<byte> data, uint key)
        {
            Span<byte> table = stackalloc byte[256];
            this.KeyGenerator(table, key);
            for (int i = 0; i < data.Length; ++i)
            {
                byte temp = data[i];
                temp ^= table[i % 179];
                temp += 0x03;
                temp += table[i % 89];
                temp ^= 0x77;
                data[i] = temp;
            }
        }
    }
}

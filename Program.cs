using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EncryptionDecryption
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 16;
            ulong key = 1921374571; // 1921374571
            ulong vektor = 19283746192;
            String test = "Please kill me:)"; //I use this text for test my encryption:)
            List<ulong> blocks = getBlocksFromMessage(test);
            List<uint> roundKeys = getRoundKeysList(key, n);

            Console.WriteLine(test);
            Console.WriteLine("Encryption....");
            for (int i = 0; i < blocks.Count; i++)
            {
                uint bleft = (uint)(blocks[i] >> 32);
                
                uint bright = (uint)(blocks[i]);
                if (i > 0)
                {
                    vektor = blocks[i - 1];
                }
                for (int j = 0; j < n; j++)
                {
                    uint bright2 = bright ^ roundKeys[j];
                    uint bright2AfterFunction = function(bright2);
                    bleft = ((bleft^ (uint)vektor) ^ bright2AfterFunction);
                    bright = bright2;
                }
                ulong tmp = ((ulong)bleft) << 32 | (ulong) bright;
                blocks[i] = tmp;
            }
            for (int i = 0; i < blocks.Count; i++)
            {
                byte[] tmp = BitConverter.GetBytes(blocks[i]);
                string text = "";
                for (int j = 0; j < tmp.Length; j++)
                {
                    text = text + (char)tmp[j];
                }
                if (i == blocks.Count -1 )
                {
                    Console.WriteLine(text);
                }
                else
                {
                    Console.Write(text);
                }
                 
            }
            Console.WriteLine("Decryption...");
            for (int i = 0; i < blocks.Count; i++)
            {
                uint bleft = (uint)(blocks[i] >> 32);
                uint bright = (uint)(blocks[i]);
                for (int j = 0; j < n; j++)
                {
                    bleft = (bleft ^ function(bright))^ (uint)vektor;
                    bright = bright ^ roundKeys[n - 1 - j];
                }
                ulong tmp = ((ulong)bleft) << 32 | (ulong)bright;
                blocks[i] = tmp;
            }
            for (int i = 0; i < blocks.Count; i++)
            {
                byte[] tmp = BitConverter.GetBytes(blocks[i]);
                for (int j = 0; j < tmp.Length; j++)
                {
                    Console.Write((char)tmp[j]);
                }
            }
        }

        private static List<ulong> getBlocksFromMessage(string msg)
        {
            List<ulong> res = new List<ulong>();
            
            for (int i = 0; i < msg.Length % 8;i++)
            {
                msg = msg + 0;
            }
            byte[] str = Encoding.Default.GetBytes(msg);
            for (int i = 0; i < str.Length; i+=8)
            {
                byte[] tmp = new byte[8];
                tmp[0] = str[i];
                tmp[1] = str[i + 1];
                tmp[2] = str[i+2];
                tmp[3] = str[i + 3];
                tmp[4] = str[i+4];
                tmp[5] = str[i+5];
                tmp[6] = str[i + 6];
                tmp[7] = str[i+7];
                res.Add(BitConverter.ToUInt64(tmp, 0));
            }
            return res;
        }

        private static List<uint> getRoundKeysList(ulong key, int n)
        {
            List<uint> roundKeysList = new List<uint>();
            for (int i = 0; i < n; i++)
            {
                uint roundKey = (uint)RotateRight(key, i * 3);
                uint key32 = (uint)roundKey;
                for (int j = 0, jj = 0; j < 64; j += 2, jj++)
                {
                    ulong l1 = roundKey >> (63 - j) << (31 - jj);
                    roundKey |= (uint)l1;
                }
                roundKeysList.Add(roundKey);
            }
            return roundKeysList;
        }

        private static List<uint> getRoundVektorList(uint vektor, int n)
        {
            List<uint> roudVektors = new List<uint>();
            /*for (int i = 0; i < n; i++)
            {
                uint bit = vektor << 31 >> 31;
                uint iv = vektor >> 1;
                uint bit2 = vektor << (31 - 2) >> 31;
                uint resBit = bit ^ bit2;
                for (int j = 4; j < 30; j+=2)
                {
                    bit= vektor << (31 - j) >> (31 - j);
                    
                }
                
            }*/
            return roudVektors;
        }

        /*private static ulong flipBit (int pos, ulong val)
        {
            return val ^ 1 << pos;
        }
        */
        private static uint function (uint halfBlock)
        {
            uint a = RotateRight(getBitsFromIntAsOneByteByInt(halfBlock, 0), 3);
            uint b = getBitsFromIntAsOneByteByInt(halfBlock, 16) ^ getBitsFromIntAsOneByteByInt(halfBlock, 15);
            uint c = ~getBitsFromIntAsOneByteByInt(halfBlock, 24);
            uint d = RotateRight(getBitsFromIntAsOneByteByInt(halfBlock, 8), 5);
            return concat(a, b, c, d);
        }

        private static uint concat (uint a, uint b, uint c, uint d)
        {
            return a << 24 | b << 16 | c << 8 | d;
        }

        private static uint getBitsFromIntAsOneByteByInt(uint number, int startIndex)
        {
            return (number << (startIndex - 1) >> 24);
        }

        private static ulong RotateLeft(ulong x, int n)
        {
            return (ulong)(((x) << (n)) | ((x) >> (64 - (n))));
        }

        private static ulong RotateRight(ulong x, int n)
        {
            return (ulong)(((x) >> (n)) | ((x) << (64 - (n))));
        }
        private static uint RotateRight(uint x, int n)
        {
            return (uint)(((x) >> (n)) | ((x) << (32 - (n))));
        }
    }
}

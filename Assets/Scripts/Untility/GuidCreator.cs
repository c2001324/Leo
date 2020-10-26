using System;
using UnityEngine;
using System.Collections;

namespace Untility
{
    public class GuidCreator
    {
        static public uint GetOid()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToUInt32(buffer, 0);
        }

        static public int GetIntOid()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt32(buffer, 0);
        }
    }
}

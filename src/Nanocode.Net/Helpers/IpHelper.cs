using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;

namespace Nanocode.Net.Helpers
{
    public static class IPHelper
    {
        public static uint IPv4ToInteger(string ipAddress)
        {
            var address = IPAddress.Parse(ipAddress);
            byte[] bytes = address.GetAddressBytes();

            // flip big-endian(network order) to little-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToUInt32(bytes, 0);
        }
        public static string IntegerToIPv4(uint ipAddress)
        {
            byte[] bytes = BitConverter.GetBytes(ipAddress);

            // flip little-endian to big-endian(network order)
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return new IPAddress(bytes).ToString();
        }
        public static BigInteger IPv6ToBigInteger(string ipAddress)
        {
            System.Numerics.BigInteger ipnum;

            if (IPAddress.TryParse(ipAddress, out IPAddress address))
            {
                byte[] addrBytes = address.GetAddressBytes();

                if (BitConverter.IsLittleEndian)
                {
                    List<byte> byteList = new List<byte>(addrBytes);
                    byteList.Reverse();
                    addrBytes = byteList.ToArray();
                }

                if (addrBytes.Length > 8)
                {
                    //IPv6
                    ipnum = BitConverter.ToUInt64(addrBytes, 8);
                    ipnum <<= 64;
                    ipnum += BitConverter.ToUInt64(addrBytes, 0);
                }
                else
                {
                    //IPv4
                    ipnum = BitConverter.ToUInt32(addrBytes, 0);
                }
            }

            return ipnum;
        }
        public static string BigIntegerToIPv6(string ipAddress)
        {
            string retval = "";
            if (System.Numerics.BigInteger.TryParse(ipAddress, out System.Numerics.BigInteger intval))
            {
                retval = intval.ToString("x").PadLeft(32, '0');
                char[] trimme = new[] { ':' };
                retval = System.Text.RegularExpressions.Regex.Replace(retval, "(.{4})", "$1:").TrimEnd(trimme);
            }
            return retval;
        }
    }
}

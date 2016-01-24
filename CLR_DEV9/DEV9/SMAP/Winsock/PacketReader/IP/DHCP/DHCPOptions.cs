﻿using CLRDEV9.DEV9.SMAP.Winsock.PacketReader.IP;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace CLRDEV9.DEV9.SMAP.Winsock.PacketReader.DHCP
{
    class DHCPopNOP : TCPOption //unlike TCP options, DCHP length feild does not count the option header
    {
        public DHCPopNOP()
        {

        }
        public override byte Length { get { return 1; } }
        public override byte Code { get { return 0; } }

        public override byte[] GetBytes()
        {
            return new byte[] { Code };
        }
    }
    class DHCPopSubnet : TCPOption
    {
        //Subnet Mask
        byte[] mask = new byte[4];
        public DHCPopSubnet(byte[] data)
        {
            mask = data;
        }
        public DHCPopSubnet(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 2;
            NetLib.ReadByteArray(data, ref offset, 4, out mask);
        }
        public override byte Length { get { return 6; } }
        public override byte Code { get { return 1; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByteArray(ref ret, ref counter, mask);
            return ret;
        }
    }
    class DHCPopRouter : TCPOption //can be longer then 1 address (not supported)
    {
        byte[] routerip = new byte[4];
        public DHCPopRouter(byte[] data)
        {
            routerip = data;
        }
        public DHCPopRouter(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 2;
            NetLib.ReadByteArray(data, ref offset, 4, out routerip);
        }
        public override byte Length { get { return 6; } }
        public override byte Code { get { return 3; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByteArray(ref ret, ref counter, routerip);
            return ret;
        }
    }
    class DHCPopDNS : TCPOption //can be longer then 1 address (not supported)
    {
        byte[] dnsip = new byte[4];
        public DHCPopDNS(IPAddress parIP)
        {
            dnsip = parIP.GetAddressBytes();
        }
        public DHCPopDNS(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 2;
            NetLib.ReadByteArray(data, ref offset, 4, out dnsip);
        }
        public override byte Length { get { return 6; } }
        public override byte Code { get { return 6; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByteArray(ref ret, ref counter, dnsip);
            return ret;
        }
    }
    class DHCPopBCIP : TCPOption //The IP to send broadcasts to
    {
        byte[] ip = new byte[4];
        public DHCPopBCIP(byte[] data) //ip provided as byte array
        {
            ip = data;
        }
        public DHCPopBCIP(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 2;
            NetLib.ReadByteArray(data, ref offset, 4, out ip);
        }
        public override byte Length { get { return 6; } }
        public override byte Code { get { return 28; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByteArray(ref ret, ref counter, ip);
            return ret;
        }
    }
    class DHCPopDNSNAME : TCPOption
    {
        byte len;
        byte[] DomainNameBytes;
        public DHCPopDNSNAME(string name)
        {
            DomainNameBytes = Encoding.ASCII.GetBytes(name);
            len = (byte)DomainNameBytes.Length;
            if (DomainNameBytes.Length > len)
            {
                throw new Exception("Domain Name Overflow");
            }
        }
        public DHCPopDNSNAME(byte[] data, int offset) //Offset will include Kind and Len
        {
            throw new NotImplementedException();
            //len = data[offset + 1];
            //DomainNameBytes = new byte[len];
            //Utils.memcpy(ref DomainNameBytes, 0, data, offset + 2, len);
        }
        public override byte Length { get { return (byte)(2 + len); } }
        public override byte Code { get { return 15; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            ret[0] = Code;
            ret[1] = (byte)(Length - 2);
            Utils.memcpy(ref ret, 2, DomainNameBytes, 0, len);
            return ret;
        }
    }
    class DHCPopREQIP : TCPOption //can be longer then 1 address (not supported)
    {
        byte[] ip = new byte[4];
        public byte[] IPaddress
        {
            get
            {
                return ip;
            }
        }
        public DHCPopREQIP(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 2;
            NetLib.ReadByteArray(data, ref offset, 4, out ip);
        }
        public override byte Length { get { return 6; } }
        public override byte Code { get { return 50; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByteArray(ref ret, ref counter, ip);
            return ret;
        }
    }
    class DHCPopIPLT : TCPOption
    {
        public UInt32 IPLeaseTime;
        public DHCPopIPLT(UInt32 LeaseTime)
        {
            IPLeaseTime = LeaseTime;
        }
        public DHCPopIPLT(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 2;
            NetLib.ReadUInt32(data, ref offset, out IPLeaseTime);
        }
        public override byte Length { get { return 6; } }
        public override byte Code { get { return 51; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteUInt32(ref ret, ref counter, IPLeaseTime);
            return ret;
        }
    }
    class DHCPopMSG : TCPOption
    {
        byte msg;
        public byte Message
        {
            get
            {
                return msg;
            }
        }
        public DHCPopMSG(byte parMsg)
        {
            msg = parMsg;
        }
        public DHCPopMSG(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 2;
            NetLib.ReadByte08(data, ref offset, out msg);
        }
        public override byte Length { get { return 3; } }
        public override byte Code { get { return 53; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByte08(ref ret, ref counter, msg);
            return ret;
        }
    }
    class DHCPopSERVIP : TCPOption //DHCP server ip
    {
        byte[] ip = new byte[4];
        public byte[] IPaddress
        {
            get
            {
                return ip;
            }
        }
        public DHCPopSERVIP(byte[] data) //ip provided as byte array
        {
            ip = data;
        }
        public DHCPopSERVIP(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 2;
            NetLib.ReadByteArray(data, ref offset, 4, out ip);
        }
        public override byte Length { get { return 6; } }
        public override byte Code { get { return 54; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByteArray(ref ret, ref counter, ip);
            return ret;
        }
    }
    class DHCPopREQLIST : TCPOption
    {
        byte len;
        byte[] Requests;
        public byte[] RequestList
        {
            get
            {
                return Requests;
            }
        }
        public DHCPopREQLIST(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 1;
            NetLib.ReadByte08(data, ref offset, out len);
            NetLib.ReadByteArray(data, ref offset, len, out Requests);
        }
        public override byte Length { get { return (byte)(2 + len); } }
        public override byte Code { get { return 55; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByteArray(ref ret, ref counter, Requests);
            return ret;
        }
    }
    //class DHCPopMSGStrOld : TCPOption
    //{
    //    byte len;
    //    byte[] MsgBytes;
    //    public DHCPopMSGStrOld(byte[] data, int offset) //Offset will include Kind and Len
    //    {
    //        len = data[offset + 1];
    //        MsgBytes = new byte[len];
    //        Utils.memcpy(ref MsgBytes, 0, data, offset + 2, len);
    //        Encoding enc = Encoding.ASCII;
    //        //Error.WriteLine(enc.GetString(MsgBytes));
            
    //    }
    //    public override byte Length { get { return (byte)(2 + len); } }
    //    public override byte Code { get { return 56; } }

    //    public override byte[] GetBytes()
    //    {
    //        byte[] ret = new byte[Length];
    //        ret[0] = Code;
    //        ret[1] = (byte)(Length - 2);
    //        Utils.memcpy(ref ret, 2, MsgBytes, 0, len);
    //        return ret;
    //    }
    //}
    class DHCPopMSGStr : TCPOption
    {
        byte len;
        byte[] MsgBytes;
        public DHCPopMSGStr(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 1;
            NetLib.ReadByte08(data, ref offset, out len);
            NetLib.ReadByteArray(data, ref offset, len, out MsgBytes);

            Encoding enc = Encoding.ASCII;
            PSE.CLR_PSE_PluginLog.WriteLine(TraceEventType.Information, (int)DEV9LogSources.Winsock, "DCHP", enc.GetString(MsgBytes));
        }
        public override byte Length { get { return (byte)(2 + len); } }
        public override byte Code { get { return 56; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByteArray(ref ret, ref counter, MsgBytes);
            return ret;
        }
    }
    class DHCPopMMSGS : TCPOption
    {
        public UInt16 MaxMessageSize;
        public DHCPopMMSGS(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 2;
            NetLib.ReadUInt16(data, ref offset, out MaxMessageSize);
        }
        public override byte Length { get { return 4; } }
        public override byte Code { get { return 57; } }
        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteUInt16(ref ret, ref counter, MaxMessageSize);
            return ret;
        }
    }
    class DHCPopCID : TCPOption
    {
        byte len;
        byte[] ClientID;
        public DHCPopCID(byte[] data, int offset) //Offset will include Kind and Len
        {
            offset += 1;
            NetLib.ReadByte08(data, ref offset, out len);
            //ClientID = new byte[len];
            NetLib.ReadByteArray(data, ref offset, len, out ClientID);
        }
        public override byte Length { get { return (byte)(2 + len); } }
        public override byte Code { get { return 61; } }

        public override byte[] GetBytes()
        {
            byte[] ret = new byte[Length];
            int counter = 0;
            NetLib.WriteByte08(ref ret, ref counter, Code);
            NetLib.WriteByte08(ref ret, ref counter, (byte)(Length - 2));
            NetLib.WriteByteArray(ref ret, ref counter, ClientID);
            return ret;
        }
    }
    class DHCPopEND : TCPOption
    {
        public DHCPopEND()
        {

        }
        public override byte Length { get { return 1; } }
        public override byte Code { get { return 255; } }

        public override byte[] GetBytes()
        {
            return new byte[] { Code };
        }
    }
}

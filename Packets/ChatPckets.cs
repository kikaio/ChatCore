using ChatCore.Enums;
using CoreNet.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatCore.Packets
{
    public interface ISerializePacket
    {
        void SerRead();
        void SerWrite();
    }

    public class ChatPacket : Packet, ISerializePacket
    {
        public ECONTENT cType { get; set; }

        public ChatPacket()
        {
        }

        public ChatPacket(Packet _p) : base(_p)
        {
            cType = Translate.Read<ECONTENT>(data);
        }

        public ChatPacket(PACKET_TYPE _pt, ECONTENT _ct)
            : base(128)
        {
            pType = _pt;
            cType = _ct;
        }

        public virtual void SerRead()
        {
        }

        public virtual void SerWrite()  
        {
            Translate.Write(data, pType);
            Translate.Write(data, cType);
        }
    }

    public class HelloReq : ChatPacket
    {
        public string publicRsaXml;
        public HelloReq()
            : base(PACKET_TYPE.REQ, ECONTENT.HELLO)
        {
        }

        public HelloReq(ChatPacket _cp)
        {
            pType = _cp.pType;
            cType = _cp.cType;
            data = _cp.data;
            header = _cp.header;
        }

        public override void SerRead()
        {
            publicRsaXml = Translate.Read<string>(data);
        }

        public override void SerWrite()
        {
            base.SerWrite();
            Translate.Write(data, publicRsaXml);
            UpdateHeader();
        }
    }

    public class WelcomeAns : ChatPacket
    {
        public long sId;
        public WelcomeAns()
            :base(PACKET_TYPE.ANS, ECONTENT.WELCOME)
        {
        }
        public WelcomeAns(ChatPacket _cp)
        {
            pType = _cp.pType;
            cType = _cp.cType;
            data = _cp.data;
            header = _cp.header;
        }

        public override void SerRead()
        {
            sId = Translate.Read<long>(data);
        }
        public override void SerWrite()
        {
            base.SerWrite();
            Translate.Write(data, sId);
            UpdateHeader();
        }
    }

    public class ChatNoti : ChatPacket
    {
        public string msg { get; set; }
        public long sId { get; set; }
        public ChatNoti()
            : base(PACKET_TYPE.NOTI, ECONTENT.CHAT)
        {
        }

        public ChatNoti(ChatPacket _cp)
        {
            pType = _cp.pType;
            cType = _cp.cType;
            data = _cp.data;
            header = _cp.header;
        }

        public override void SerRead()
        {
            sId = Translate.Read<long>(data);
            msg = Translate.Read<string>(data);
        }

        public override void SerWrite()
        {
            base.SerWrite();
            Translate.Write(data, sId);
            Translate.Write(data, msg);
            UpdateHeader();
        }
    }
    //c to s
    public class DH_Req : ChatPacket
    {
        public DH_Req()
            : base(PACKET_TYPE.REQ, ECONTENT.DH_KEY_SWAP)
        {
        }

        public DH_Req(ChatPacket _cp)
        {
            pType = _cp.pType;
            cType = _cp.cType;
            data = _cp.data;
            header = _cp.header;
        }

        public override void SerWrite()
        {
            base.SerWrite();
            UpdateHeader();
        }
    }

    //s to c
    public class DH_Ans : ChatPacket
    {
        public string dhKey { get; set; }
        public string dhIV { get; set; }

        public DH_Ans()
            : base(PACKET_TYPE.ANS, ECONTENT.DH_KEY_SWAP)
        {
        }
        public DH_Ans(ChatPacket _cp)
        {
            pType = _cp.pType;
            cType = _cp.cType;
            data = _cp.data;
            header = _cp.header;
        }

        public override void SerRead()
        {
            dhKey = Translate.Read<string>(data);
            dhIV = Translate.Read<string>(data);
        }

        public override void SerWrite()
        {
            base.SerWrite();
            Translate.Write(data, dhKey);
            Translate.Write(data, dhIV);
            UpdateHeader();
        }
    }
}

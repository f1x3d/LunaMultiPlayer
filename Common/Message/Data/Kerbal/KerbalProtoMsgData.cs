﻿using LunaCommon.Message.Types;

namespace LunaCommon.Message.Data.Kerbal
{
    public class KerbalProtoMsgData : KerbalBaseMsgData
    {
        /// <inheritdoc />
        internal KerbalProtoMsgData() { }
        public override KerbalMessageType KerbalMessageType => KerbalMessageType.Proto;

        public double SendTime { get; set; }
        public string KerbalName { get; set; }
        public byte[] KerbalData { get; set; }
    }
}
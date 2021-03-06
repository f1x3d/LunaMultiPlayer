﻿using LunaCommon.Message.Types;

namespace LunaCommon.Message.Data.Flag
{
    public class FlagDeleteMsgData : FlagBaseMsgData
    {
        /// <inheritdoc />
        internal FlagDeleteMsgData() { }
        public override FlagMessageType FlagMessageType => FlagMessageType.FlagDelete;
        public string FlagName { get; set; }
    }
}
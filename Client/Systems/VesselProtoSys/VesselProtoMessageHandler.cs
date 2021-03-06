﻿using LunaClient.Base;
using LunaClient.Base.Interface;
using LunaClient.Network;
using LunaClient.Systems.VesselRemoveSys;
using LunaCommon.Enums;
using LunaCommon.Message.Data.Vessel;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Types;
using System.Collections.Concurrent;

namespace LunaClient.Systems.VesselProtoSys
{
    public class VesselProtoMessageHandler : SubSystem<VesselProtoSystem>, IMessageHandler
    {
        public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

        public void HandleMessage(IServerMessageBase msg)
        {
            if (!(msg.Data is VesselBaseMsgData msgData)) return;

            switch (msgData.VesselMessageType)
            {
                case VesselMessageType.ListReply:
                    HandleVesselList((VesselListReplyMsgData)msgData);
                    break;
                case VesselMessageType.VesselsReply:
                    HandleVesselResponse((VesselsReplyMsgData)msgData);
                    break;
                case VesselMessageType.Proto:
                    HandleVesselProto((VesselProtoMsgData)msgData);
                    break;
                default:
                    LunaLog.LogError($"[LMP]: Cannot handle messages of type: {msgData.VesselMessageType} in VesselProtoMessageHandler");
                    break;
            }
        }

        private static void HandleVesselProto(VesselProtoMsgData messageData)
        {
            if (!SystemsContainer.Get<VesselRemoveSystem>().VesselWillBeKilled(messageData.VesselId))
                VesselsProtoStore.HandleVesselProtoData(messageData.VesselData, messageData.VesselId);
        }

        private static void HandleVesselResponse(VesselsReplyMsgData messageData)
        {
            //We read the vessels syncronously so when we start the game we have the dictionary of all the vessels already loaded
            foreach (var vesselDataKv in messageData.VesselsData)
            {
                if (!SystemsContainer.Get<VesselRemoveSystem>().VesselWillBeKilled(vesselDataKv.Key))
                    VesselsProtoStore.HandleVesselProtoData(vesselDataKv.Value, vesselDataKv.Key, true);
            }

            MainSystem.NetworkState = ClientState.VesselsSynced;
        }

        /// <summary>
        /// Here we receive the vessel list msg from the server. And we request all the vesseslw e don't have.
        /// Before we used a cache system but it was a bad idea as protovessels change very often as they hold the orbit data, etc.
        /// </summary>
        private static void HandleVesselList(VesselListReplyMsgData messageData)
        {
            //Request the vessel data that we don't have.
            var msgData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselsRequestMsgData>();
            msgData.RequestList = messageData.Vessels;

            System.MessageSender.SendMessage(msgData);
        }
    }
}

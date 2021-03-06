﻿using LunaClient.Base;
using LunaClient.Base.Interface;
using LunaClient.Systems.Chat;
using LunaClient.Systems.SettingsSys;
using LunaCommon.Enums;
using LunaCommon.Message.Data.CraftLibrary;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Types;
using System.Collections.Concurrent;

namespace LunaClient.Systems.CraftLibrary
{
    public class CraftLibraryMessageHandler : SubSystem<CraftLibrarySystem>, IMessageHandler
    {
        public ConcurrentQueue<IServerMessageBase> IncomingMessages { get; set; } = new ConcurrentQueue<IServerMessageBase>();

        public void HandleMessage(IServerMessageBase msg)
        {
            if (!(msg.Data is CraftLibraryBaseMsgData msgData)) return;

            switch (msgData.CraftMessageType)
            {
                case CraftMessageType.ListReply:
                    {
                        var data = (CraftLibraryListReplyMsgData)msgData;
                        var playerList = data.PlayerCrafts;
                        foreach (var playerCraft in playerList)
                        {
                            var vabExists = playerCraft.Value.VabExists;
                            var sphExists = playerCraft.Value.SphExists;
                            var subassemblyExists = playerCraft.Value.SubassemblyExists;
                            LunaLog.Log($"[LMP]: Player: {playerCraft.Key}, VAB: {vabExists}, SPH: {sphExists}, SUBASSEMBLY {subassemblyExists}");
                            if (vabExists)
                            {
                                var vabCrafts = playerCraft.Value.VabCraftNames;
                                foreach (var vabCraft in vabCrafts)
                                {
                                    var cce = new CraftChangeEntry
                                    {
                                        PlayerName = playerCraft.Key,
                                        CraftType = CraftType.Vab,
                                        CraftName = vabCraft
                                    };
                                    System.QueueCraftAdd(cce);
                                }
                            }
                            if (sphExists)
                            {
                                var sphCrafts = playerCraft.Value.SphCraftNames;
                                foreach (var sphCraft in sphCrafts)
                                {
                                    var cce = new CraftChangeEntry
                                    {
                                        PlayerName = playerCraft.Key,
                                        CraftType = CraftType.Sph,
                                        CraftName = sphCraft
                                    };
                                    System.QueueCraftAdd(cce);
                                }
                            }
                            if (subassemblyExists)
                            {
                                var subassemblyCrafts = playerCraft.Value.SubassemblyCraftNames;
                                foreach (var subassemblyCraft in subassemblyCrafts)
                                {
                                    var cce = new CraftChangeEntry
                                    {
                                        PlayerName = playerCraft.Key,
                                        CraftType = CraftType.Subassembly,
                                        CraftName = subassemblyCraft
                                    };
                                    System.QueueCraftAdd(cce);
                                }
                            }
                        }
                        MainSystem.NetworkState = ClientState.CraftlibrarySynced;
                    }
                    break;
                case CraftMessageType.AddFile:
                    {
                        var data = (CraftLibraryAddMsgData)msgData;
                        var cce = new CraftChangeEntry
                        {
                            PlayerName = data.PlayerName,
                            CraftType = data.UploadType,
                            CraftName = data.UploadName
                        };
                        System.QueueCraftAdd(cce);
                        SystemsContainer.Get<ChatSystem>().Queuer.QueueChannelMessage(SettingsSystem.ServerSettings.ConsoleIdentifier, "",
                            $"{cce.PlayerName} shared {cce.CraftName} ({cce.CraftType})");
                    }
                    break;
                case CraftMessageType.DeleteFile:
                    {
                        var data = (CraftLibraryDeleteMsgData)msgData;
                        var cce = new CraftChangeEntry
                        {
                            PlayerName = data.PlayerName,
                            CraftType = data.CraftType,
                            CraftName = data.CraftName
                        };
                        System.QueueCraftDelete(cce);
                    }
                    break;
                case CraftMessageType.RespondFile:
                    {
                        var data = (CraftLibraryRespondMsgData)msgData;
                        var cre = new CraftResponseEntry
                        {
                            PlayerName = data.PlayerName,
                            CraftType = data.RequestedType,
                            CraftName = data.RequestedName
                        };
                        var hasCraft = data.HasCraft;
                        if (hasCraft)
                        {
                            cre.CraftData = data.CraftData;
                            System.QueueCraftResponse(cre);
                        }
                    }
                    break;
            }
        }
    }
}
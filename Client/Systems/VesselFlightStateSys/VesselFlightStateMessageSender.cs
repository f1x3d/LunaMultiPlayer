﻿using LunaClient.Base;
using LunaClient.Base.Interface;
using LunaClient.Network;
using LunaCommon.Message.Client;
using LunaCommon.Message.Data.Vessel;
using LunaCommon.Message.Interface;

namespace LunaClient.Systems.VesselFlightStateSys
{
    public class VesselFlightStateMessageSender : SubSystem<VesselFlightStateSystem>, IMessageSender
    {
        public void SendMessage(IMessageData msg)
        {
            NetworkSender.QueueOutgoingMessage(MessageFactory.CreateNew<VesselCliMsg>(msg));
        }

        public void SendCurrentFlightState()
        {
            var flightState = new FlightCtrlState();
            flightState.CopyFrom(FlightGlobals.ActiveVessel.ctrlState);
            var id = FlightGlobals.ActiveVessel.id;

            TaskFactory.StartNew(() =>
            {
                var msgData = NetworkMain.CliMsgFactory.CreateNewMessageData<VesselFlightStateMsgData>();
                msgData.VesselId = id;
                msgData.GearDown = flightState.gearDown;
                msgData.GearUp = flightState.gearUp;
                msgData.Headlight = flightState.headlight;
                msgData.KillRot = flightState.killRot;
                msgData.MainThrottle = flightState.mainThrottle;
                msgData.Pitch = flightState.pitch;
                msgData.PitchTrim = flightState.pitchTrim;
                msgData.Roll = flightState.roll;
                msgData.RollTrim = flightState.rollTrim;
                msgData.WheelSteer = flightState.wheelSteer;
                msgData.WheelSteerTrim = flightState.wheelSteerTrim;
                msgData.WheelThrottle = flightState.wheelThrottle;
                msgData.WheelThrottleTrim = flightState.wheelThrottleTrim;
                msgData.X = flightState.X;
                msgData.Y = flightState.Y;
                msgData.Yaw = flightState.yaw;
                msgData.YawTrim = flightState.yawTrim;
                msgData.Z = flightState.Z;

                SendMessage(msgData);
            });
        }
    }
}

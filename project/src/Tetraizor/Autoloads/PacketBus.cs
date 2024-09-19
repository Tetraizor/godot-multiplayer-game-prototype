namespace Tetraizor.Autoloads;

using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Tetraizor.Data.Networking.Packet;
using Tetraizor.Enums;
using Generic = System.Collections.Generic;

public partial class PacketBus : AutoloadBase<PacketBus>
{
    public bool IsActive => NetworkManager.Instance.IsConnectionActive;

    private List<PacketBase> _packetList = new List<PacketBase>();

    public delegate void PacketReceivedEventHandler(int senderId, PacketType type, Dictionary rawData);
    public event PacketReceivedEventHandler PacketReceived;

    private float _timer;
    public override void _Process(double dDelta)
    {
        float delta = (float)dDelta;

        _timer += delta;
        if (_timer > 1f / NetworkManager.TICK_RATE)
        {
            _timer = 0;

            if (!IsActive) return;
            ProcessPackets();
        }
    }

    public void ProcessPackets()
    {
        foreach (PacketBase packet in _packetList)
        {
            // RpcConfig("ReceivePacket", new Dictionary {
            //     {"rpc_mode", (int)MultiplayerApi.RpcMode.AnyPeer},
            //     {"call_local", true},
            //     {"transfer_mode", (int)packet.TransferMode},
            // });

            if (packet.Receivers == null || packet.Receivers.Length == 0)
            {
                Rpc("ReceivePacket", (int)packet.Type, packet.Serialize());
            }
            else
            {
                foreach (int receiver in packet.Receivers)
                {
                    RpcId(receiver, "ReceivePacket", (int)packet.Type, packet.Serialize());
                }
            }
        }

        _packetList.Clear();
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, CallLocal = true)]
    public void ReceivePacket(PacketType type, Dictionary packetData)
    {
        int senderId = Multiplayer.GetRemoteSenderId();

        PacketReceived?.Invoke(senderId, type, packetData);
    }

    public static void QueuePacket(PacketBase packet, bool overrideSameType = false)
    {
        if (overrideSameType) Instance._packetList.RemoveAll(p => p.Type == packet.Type);
        Instance._packetList.Add(packet);
    }
}
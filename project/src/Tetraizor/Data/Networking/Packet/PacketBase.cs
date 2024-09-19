namespace Tetraizor.Data.Networking.Packet;

using Godot;
using Godot.Collections;
using Tetraizor.Autoloads;
using Tetraizor.Enums;

public abstract class PacketBase : INetworkSerializable
{
    public int[] Receivers { get; set; }
    public int SenderId { get; set; }
    public PacketType Type { get; set; }
    public MultiplayerPeer.TransferModeEnum TransferMode { get; set; } = MultiplayerPeer.TransferModeEnum.Reliable;

    public PacketBase(PacketType type, int senderId = -1, int[] receivers = null)
    {
        Type = type;
        Receivers = receivers;

        if (senderId == -1)
            SenderId = NetworkManager.LocalProfile.NetworkId;
    }

    public abstract Dictionary Serialize();
    public abstract void Deserialize(Dictionary rawData);
}
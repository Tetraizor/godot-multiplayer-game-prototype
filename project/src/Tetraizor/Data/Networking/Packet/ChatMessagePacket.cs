using Godot.Collections;
using Tetraizor.Enums;

namespace Tetraizor.Data.Networking.Packet;

public class ChatMessagePacket : PacketBase
{
    public string Message { get; set; }
    public string SenderName { get; set; }

    public ChatMessagePacket(PacketType type, string message, string senderName) : base(type, -1)
    {
        Message = message;
        SenderName = senderName;
        Type = PacketType.ChatMessage;
    }

    public override void Deserialize(Dictionary rawData)
    {
        Message = (string)rawData["message"];
        SenderName = (string)rawData["sender_name"];
    }

    public override Dictionary Serialize()
    {
        return new Dictionary
        {
            {"message", Message},
            {"sender_name", SenderName},
        };
    }
}
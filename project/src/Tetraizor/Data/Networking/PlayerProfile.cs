namespace Tetraizor.Data.Networking;

using System.Numerics;
using Godot.Collections;

public class PlayerProfile : INetworkSerializable
{
    public int NetworkId { get; set; }
    public string Username { get; set; }

    public void Deserialize(Dictionary rawData)
    {
        NetworkId = (int)rawData["network_id"];
        Username = (string)rawData["username"];
    }

    public Dictionary Serialize()
    {
        return new Dictionary
        {
            {"network_id", NetworkId},
            {"username", Username}
        };
    }

    public static PlayerProfile From(Dictionary rawData)
    {
        var profile = new PlayerProfile();
        profile.Deserialize(rawData);

        return profile;
    }
}
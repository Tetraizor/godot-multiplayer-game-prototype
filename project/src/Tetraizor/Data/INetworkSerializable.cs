namespace Tetraizor.Data;

using Godot.Collections;

public interface INetworkSerializable
{
    public Dictionary Serialize();
    public void Deserialize(Dictionary rawData);
}
namespace Tetraizor.Data.Networking;

using Godot;
using Godot.Collections;
using Tetraizor.Entity;

public class PlayerWrapper
{
    public PlayerProfile Profile { get; set; }
    public EntityWrapper EntityWrapper { get; set; }

    public int Id => Profile.NetworkId;
}
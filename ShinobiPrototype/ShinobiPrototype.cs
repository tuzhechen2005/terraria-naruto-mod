using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ShinobiPrototype.Common.Systems;

namespace ShinobiPrototype;

public sealed class ShinobiPrototype : Mod
{
    internal enum Packet : byte
    {
        BuildBridge,      // client -> server: shore X, direction, water Y
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        switch ((Packet)reader.ReadByte())
        {
            case Packet.BuildBridge:
                BridgeSite site = new(reader.ReadInt32(), reader.ReadSByte(), reader.ReadInt32());
                if (Main.netMode == NetmodeID.Server)
                    BridgeBlueprintNet.BuildOnServer(site, whoAmI);
                break;
        }
    }
}

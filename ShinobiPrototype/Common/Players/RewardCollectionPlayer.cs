using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ShinobiPrototype.Content.Items;
using ShinobiPrototype.Content.Items.Weapons;

namespace ShinobiPrototype.Common.Players;

// Which chapter rewards this character has ever held, so the handbook can show them in colour instead of as black
// silhouettes. Checked about once a second on the local client.
public sealed class RewardCollectionPlayer : ModPlayer
{
    private HashSet<string> seen = new();

    public static int[] WaveRewards => new[]
    {
        ModContent.ItemType<Kubikiribocho>(), ModContent.ItemType<Senbon>(),
        ModContent.ItemType<WaterDragonJutsu>(), ModContent.ItemType<IceMirrorJutsu>(),
        ModContent.ItemType<ZabuzaHeadband>(), ModContent.ItemType<HakuMask>(),
    };

    public override void Initialize() => seen = new HashSet<string>();

    public bool HasSeen(int type) => seen.Contains(ItemLoader.GetItem(type)?.FullName ?? "");

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer || Main.GameUpdateCount % 60 != 0)
            return;
        foreach (int type in WaveRewards)
            if (!HasSeen(type) && (Player.HasItem(type) || IsWorn(type)))
                seen.Add(ItemLoader.GetItem(type).FullName);
    }

    private bool IsWorn(int type)
    {
        for (int i = 0; i < Player.armor.Length; i++)
            if (Player.armor[i].type == type)
                return true;
        return false;
    }

    public override void SaveData(TagCompound tag)
    {
        if (seen.Count > 0)
            tag["seenRewards"] = seen.ToList();
    }

    public override void LoadData(TagCompound tag) => seen = new HashSet<string>(tag.GetList<string>("seenRewards"));
}

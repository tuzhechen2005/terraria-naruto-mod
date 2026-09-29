using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ShinobiPrototype.Common.Players;

public sealed class DebugGodPlayer : ModPlayer
{
    public bool Enabled { get; private set; }

    public override void Initialize() => Enabled = false;

    public override void OnEnterWorld() => Enabled = false;

    public void SetEnabled(bool enabled) => Enabled = enabled;

    public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable) =>
        Enabled;
}

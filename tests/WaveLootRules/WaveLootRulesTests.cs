using ShinobiPrototype.Common;

static void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);
    Console.WriteLine($"PASS {name}");
}

Random random = new(1234);
const int wins = 100000;
int noWeapon = 0, twoWeapons = 0, duplicate = 0, zabuzaMask = 0, hakuMask = 0;
Dictionary<WaveLoot, int> weaponCount = new();
for (int i = 0; i < wins; i++)
{
    List<WaveLoot> loot = WaveLootRules.Roll(random.Next, random.NextDouble);
    List<WaveLoot> weapons = loot.Where(l => WaveLootRules.Weapons.Contains(l)).ToList();
    if (weapons.Count == 0) noWeapon++;
    if (weapons.Count == 2) twoWeapons++;
    if (weapons.Distinct().Count() != weapons.Count) duplicate++;
    foreach (WaveLoot w in weapons)
        weaponCount[w] = weaponCount.GetValueOrDefault(w) + 1;
    if (loot.Contains(WaveLoot.ZabuzaHeadband)) zabuzaMask++;
    if (loot.Contains(WaveLoot.HakuMask)) hakuMask++;
}

Check(noWeapon == 0, "Every win drops at least one class weapon");
Check(Math.Abs(twoWeapons / (double)wins - WaveLootRules.ExtraWeaponChance) < 0.01, "About one win in five drops a second weapon");
Check(duplicate == 0, "The second weapon is never the same as the first");
Check(weaponCount.Values.All(c => Math.Abs(c / (double)wins - 1.2 / 4) < 0.01), "All four weapons are equally likely");
Check(Math.Abs(zabuzaMask / (double)wins - 1.0 / 7) < 0.01 && Math.Abs(hakuMask / (double)wins - 1.0 / 7) < 0.01,
    "Each boss mask drops about one win in seven");

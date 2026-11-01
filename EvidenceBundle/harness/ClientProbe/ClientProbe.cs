// Sanitized snapshot for reading. Generated from tools/ClientProbe/ClientProbe.cs.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using EpicLoot;
using HarmonyLib;
using UnityEngine;

// Test-only client plugin. Sets up scenes and logs what happened; it never presses a key or calls a Use handler.
// E presses and mouse movement come from the OS through tools/ui.ps1. Removed from the profile after testing.
[BepInPlugin("povplusplus.clientprobe", "PoV++ Client Probe", "1.0.0")]
[BepInDependency("randyknapp.mods.epicloot")]
[BepInDependency("sighsorry.FineDining")]
[BepInDependency("sighsorry.FeedLikeGrandma")]
[BepInDependency("sighsorry.UsefulTankards")]
[BepInDependency("sighsorry.TrollingFishing")]
[BepInDependency("povplusplus.feedbarrelstackcompat", BepInDependency.DependencyFlags.SoftDependency)]
public class ClientProbe : BaseUnityPlugin
{
    private static string _dir, _cmd;
    private static StreamWriter _log;
    private static long _cmdPos;
    private string _lastGui = "";
    private static Func<bool> _shardOpen;

    internal static void Say(string s)
    {
        _log.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + s);
        _log.Flush();
    }

    // Test only. Observes the open warning causes; nothing here changes their outcome.
    private static List<string> _dbNames;

    private static string ConvCounts()
    {
        var z = ZNetScene.instance; if (z == null) return "no scene";
        var parts = new System.Collections.Generic.List<string>();
        foreach (var n in new[] { "piece_cookingstation", "piece_cookingstation_iron", "piece_oven" })
        {
            var s = z.GetPrefab(n)?.GetComponent<CookingStation>();
            parts.Add(n.Replace("piece_", "") + "=" + (s == null ? -1 : s.m_conversion.Count) + "/tw" + (s == null ? 0 : s.m_conversion.Count(c => c.m_from != null && c.m_from.name.EndsWith("_TW"))));
        }
        var f = z.GetPrefab("fermenter")?.GetComponent<Fermenter>();
        parts.Add("fermenter=" + (f == null ? -1 : f.m_conversion.Count) + "/tw" + (f == null ? 0 : f.m_conversion.Count(c => c.m_from != null && c.m_from.name.EndsWith("_TW"))));
        return string.Join(" ", parts.ToArray());
    }

    private static void BindConvTimeline(Harmony h)
    {
        var awake = AccessTools.Method(typeof(ZNetScene), "Awake");
        h.Patch(awake, prefix: new HarmonyMethod(typeof(ClientProbe), nameof(SceneBefore)) { priority = Priority.First }, postfix: new HarmonyMethod(typeof(ClientProbe), nameof(SceneAfter)) { priority = -1000 });
        var vc = AccessTools.TypeByName("ValheimCuisine.ValheimCuisinePlugin+ConversionsZNetScene_AwakePost_Patch");
        var reapply = vc == null ? null : AccessTools.DeclaredMethod(vc, "ReapplyConversions");
        if (reapply != null) h.Patch(reapply, prefix: new HarmonyMethod(typeof(ClientProbe), nameof(ReapplyBefore)), postfix: new HarmonyMethod(typeof(ClientProbe), nameof(ReapplyAfter)));
        Say("CONVT bound reapply=" + (reapply != null));
    }

    private static void SceneBefore() => Say("CONVT ZNetScene.Awake before");
    private static void SceneAfter()
    {
        Say("CONVT ZNetScene.Awake after " + ConvCounts());
        var info = Harmony.GetPatchInfo(AccessTools.Method(typeof(ZNetScene), "Awake"));
        Say("CONVT postfixes: " + string.Join(" | ", info.Postfixes.OrderBy(p => p.index).Where(p => p.PatchMethod.DeclaringType.FullName.Contains("Cooking") || p.PatchMethod.DeclaringType.FullName.Contains("Conversions")).Select(p => p.index + ":" + p.PatchMethod.DeclaringType.Name + " pri" + p.priority)));
    }
    private static void ReapplyBefore() { var st = new System.Diagnostics.StackTrace(1, false); Say("CONVT Reapply before " + ConvCounts() + " caller=" + string.Join("<", st.GetFrames().Take(9).Select(fr => fr.GetMethod()?.DeclaringType?.Name + "." + fr.GetMethod()?.Name))); }
    private static void ReapplyAfter() => Say("CONVT Reapply after " + ConvCounts());
    private static void BindWarningProbes(Harmony h)
    {
        var catalog = AccessTools.TypeByName("PortalRules.PublicPortalCatalog");
        h.Patch(AccessTools.Method(catalog, "OnCatalogReceived"), prefix: new HarmonyMethod(typeof(ClientProbe), nameof(CatalogBefore)), postfix: new HarmonyMethod(typeof(ClientProbe), nameof(CatalogAfter)));
        h.Patch(AccessTools.Method(catalog, "RequestRefresh"), prefix: new HarmonyMethod(typeof(ClientProbe), nameof(CatalogRequest)));
        h.Patch(AccessTools.Method("ExpandWorldData.Locations.LocationClientData:Apply"), prefix: new HarmonyMethod(typeof(ClientProbe), nameof(EwdApply)));
        h.Patch(AccessTools.Method("FineDining.DecayRuntime:EnsureItemState"), postfix: new HarmonyMethod(typeof(ClientProbe), nameof(FdStamp)));
        h.Patch(AccessTools.Method("FineDining.DecayRuntime:ExpireItem"), prefix: new HarmonyMethod(typeof(ClientProbe), nameof(FdExpire)));
        var scan = new HarmonyMethod(typeof(ClientProbe), nameof(DbScan)) { priority = Priority.First };
        h.Patch(AccessTools.Method(typeof(ObjectDB), "Awake"), prefix: scan);
        h.Patch(AccessTools.Method(typeof(ObjectDB), "CopyOtherDB"), prefix: scan);
        foreach (var m in AccessTools.GetDeclaredMethods(AccessTools.TypeByName("Jotunn.Managers.ItemManager")).Where(m => m.GetParameters().Any(p => p.ParameterType == typeof(ObjectDB))))
            h.Patch(m, prefix: scan);
    }

    private static void FdStamp(ItemDrop.ItemData item, bool __result) { if (__result) Say("FDSTAMP " + (item?.m_dropPrefab ? item.m_dropPrefab.name : "?")); }
    private static void FdExpire(ItemDrop.ItemData item) => Say("FDEXPIRE " + (item?.m_dropPrefab ? item.m_dropPrefab.name : "?"));

    private static string PortalState()
    {
        var t = Traverse.CreateWithType("PortalRules.PublicPortalCatalog");
        return "hasSnapshot=" + t.Property("HasSnapshot").GetValue() + " revision=" + t.Field("_clientRevision").GetValue() + " entries=" + ((System.Collections.ICollection)t.Field("ClientEntries").GetValue()).Count;
    }

    private static void CatalogBefore(ZRpc rpc) => Say("CATALOG arrival t=" + Time.realtimeSinceStartup.ToString("F2") + " serverRpcNull=" + (ZNet.instance == null || ZNet.instance.GetServerRPC() == null) + " sameRpc=" + (ZNet.instance != null && rpc == ZNet.instance.GetServerRPC()) + " player=" + (Player.m_localPlayer != null) + " " + PortalState());
    private static void CatalogAfter() => Say("CATALOG done t=" + Time.realtimeSinceStartup.ToString("F2") + " " + PortalState());
    private static void CatalogRequest(bool force) => Say("CATALOG request t=" + Time.realtimeSinceStartup.ToString("F2") + " force=" + force + " serverRpcNull=" + (ZNet.instance == null || ZNet.instance.GetServerRPC() == null));
    private static void EwdApply(string yaml) => Say("EWD apply t=" + Time.realtimeSinceStartup.ToString("F2") + " yaml=" + (yaml == null ? "null" : "len" + yaml.Length));

    private static void DbScan(object[] __args, object __instance, MethodBase __originalMethod)
    {
        var db = __instance as ObjectDB ?? __args.OfType<ObjectDB>().FirstOrDefault();
        if (db == null) return;
        var items = db.m_items;
        for (var i = 0; i < items.Count; i++)
            if (!items[i]) Say("DBNULL via " + __originalMethod.Name + " idx=" + i + " of " + items.Count + " was=" + (_dbNames != null && i < _dbNames.Count ? _dbNames[i] : "?") + " before=" + (i > 0 && items[i - 1] ? items[i - 1].name : "?") + " after=" + (i + 1 < items.Count && items[i + 1] ? items[i + 1].name : "?"));
        var live = items.Where(x => x).Select(x => x.name).ToList();
        if (live.Count < items.Count && _dbNames != null)
            foreach (var gone in _dbNames.Except(live).Take(5))
                if (_dbMissing.Add(gone)) Say("DBMISSING " + gone + " via " + __originalMethod.Name);
        if (items.All(x => x) && (_dbNames == null || items.Count >= _dbNames.Count)) _dbNames = live;
    }

    private static readonly HashSet<string> _dbMissing = new HashSet<string>();

    private void Start() { var w = new Harmony("povplusplus.clientprobe.warnings"); BindConvTimeline(w); BindWarningProbes(w); }

    private void Awake()
    {
        _dir = Path.Combine(Paths.BepInExRootPath, "clientprobe");
        Directory.CreateDirectory(_dir);
        _cmd = Path.Combine(_dir, "cmd.txt");
        File.WriteAllText(_cmd, "");
        _log = new StreamWriter(Path.Combine(_dir, "log.txt"), true);
        Say("=== probe start");
        var h = new Harmony("povplusplus.clientprobe");
        Observe(h, "FeedLikeGrandma.FeedAutoFeedSystem:TryRemoveAndDrop", nameof(Dispensed));
        Observe(h, "FeedLikeGrandma.FeedAutoFeedSystem:TryFindClosestConsumableFromFeedBarrel", nameof(Finder));
        Observe(h, "Tameable:OnConsumedItem", nameof(Consumed));
        Observe(h, "UsefulTankards.TankardStorageSystem:OpenTankardStorage", nameof(TankardOpen));
        Observe(h, "TrollingFishing.FishingOverrideSystem:OpenFishingRodBag", nameof(RodOpen));
        Observe(h, "EpicLoot.ShardStones.ShardUI:Toggle", nameof(ShardToggle));
        Observe(h, "Container:DropAllItems", nameof(Spill));
        Observe(h, "Projectile:Setup", nameof(ProjectileSetup));
        Observe(h, "Attack:OnAttackTrigger", nameof(Triggered));
        Observe(h, "Humanoid:StartAttack", nameof(StartAttackPost));
        h.Patch(AccessTools.Method("FineDining.PathOfValheimanCompatibility:HealFood"), prefix: new HarmonyMethod(typeof(ClientProbe), nameof(HealBefore)), postfix: new HarmonyMethod(typeof(ClientProbe), nameof(HealAfter)));
        Observe(h, "Player:ConsumeResources", nameof(Consumed2));
        Observe(h, "SEMan:AddStatusEffect", nameof(StatusAdded));
        if (AccessTools.TypeByName("PoVPlus.Compat.SleepHud") != null) Observe(h, "PoVPlus.Compat.SleepHud:OwnsHandlers", nameof(BridgeTick));
        var open = AccessTools.PropertyGetter(typeof(API).Assembly.GetType("EpicLoot.ShardStones.ShardUI"), "IsOpen");
        _shardOpen = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), open);
    }

    private static void Observe(Harmony h, string target, string patch)
    {
        try
        {
            var parts = target.Split(':');
            var hits = AccessTools.GetDeclaredMethods(AccessTools.TypeByName(parts[0])).Where(m => m.Name == parts[1]).ToList();
            if (hits.Count == 0) Say("OBSERVE MISSING " + target);
            foreach (var m in hits) h.Patch(m, postfix: new HarmonyMethod(typeof(ClientProbe), patch));
        }
        catch (Exception e) { Say("OBSERVE FAILED " + target + " " + e.Message); }
    }

    private static void Dispensed(bool __result, ItemDrop.ItemData __1) { Say("EVENT dispense result=" + __result + " item=" + __1?.m_shared.m_name); }
    private static int _finder;
    private static void Finder(bool __result, MonsterAI __0) { if (__result || (__0.name.StartsWith("Boar") && ++_finder % 5 == 1)) Say("EVENT finder " + __0.name + " result=" + __result + " calls=" + _finder); }
    private static void Consumed(Tameable __instance) { Say("EVENT consumed by " + __instance.name + " hungry=" + __instance.IsHungry()); }
    private static void TankardOpen() { Say("EVENT tankard storage open"); }
    private static void RodOpen() { Say("EVENT rod bag open"); }
    private static void ShardToggle() { Say("EVENT socket overlay toggle open=" + _shardOpen()); }
    private static void ProjectileSetup(Projectile __instance, HitData __3) { if (__3 != null) Say("EVENT projectile " + __instance.name + " dmg=" + __3.GetTotalDamage().ToString("0.0") + " stamina=" + Player.m_localPlayer?.GetStamina().ToString("0")); }
    private static void Triggered(Attack __instance) { Say("EVENT attack trigger projectiles=" + __instance.m_projectiles + " bursts=" + __instance.m_projectileBursts + " anim=" + __instance.m_attackAnimation); }
    private static void StartAttackPost(Humanoid __instance, bool __1, bool __result) { if (__instance == Player.m_localPlayer) Say("EVENT StartAttack secondary=" + __1 + " result=" + __result); }
    private static void Compose() { Say("EVENT compose expiry"); }
    private static void Spill(Container __instance) { Say("EVENT spill " + __instance.name); }
    private static float _hp;
    private static void HealBefore(Player __0) { _hp = __0.GetHealth(); }
    private static void HealAfter(Player __0, float __1) { Say("EVENT heal amount=" + __1 + " hpGain=" + (__0.GetHealth() - _hp) + " foods=" + __0.GetFoods().Count + " gear=" + __0.GetTotalActiveMagicEffectValue("AddHealthRegen")); }
    private static void Consumed2(Piece.Requirement[] requirements, int qualityLevel) { Say("EVENT ConsumeResources q=" + qualityLevel + " " + string.Join(", ", requirements.Select(r => r.m_resItem?.name + "=" + r.GetAmount(qualityLevel)))); }
    private static void StatusAdded(StatusEffect __result, SEMan __instance) { if (__result) Say("EVENT status " + __result.name + " on " + (Traverse.Create(__instance).Field("m_character").GetValue<Character>()?.name)); }

    private static Type _clock => AccessTools.TypeByName("FineDining.SpoilageClock");
    private static long Now() { var a = new object[] { 0L }; AccessTools.Method(_clock, "TryGetWorldTicks").Invoke(null, a); return (long)a[0]; }
    private static string Remaining(ItemDrop.ItemData item)
    {
        var a = new object[] { item, Now(), 0L, false };
        return (bool)AccessTools.Method(_clock, "TryGetSpoilageClock").Invoke(null, a) ? ((long)a[2] / TimeSpan.TicksPerSecond) + "s paused=" + a[3] : "none";
    }
    private static void SetClock(ItemDrop.ItemData item, long seconds)
    {
        item.m_customData.Remove("sighsorry.FineDining.ExpiryWorldTicks");
        var value = (long)AccessTools.Method(_clock, "EncodeClockValue").Invoke(null, new object[] { Now(), seconds * TimeSpan.TicksPerSecond, false });
        AccessTools.Method("FineDining.DecayRuntime:SetClockValue").Invoke(null, new object[] { item, value });
    }

    private static MethodInfo _waveFix;
    private static float _mapShift;
    private static System.Collections.IEnumerator WaveLog(float seconds, string tag, Player player)
    {
        var path = Path.Combine(Paths.BepInExRootPath, "clientprobe", "wave-" + tag + ".csv");
        var wrapped = AccessTools.Field(typeof(WaterVolume), "s_wrappedDayTimeSeconds");
        var shader = AccessTools.Field(typeof(WaterVolume), "s_waterTime");
        var lines = new List<string> { "real,net,wrapped,shader,surface,boatY,hp,players,extra" };
        var st = AccessTools.TypeByName("BetterSleepBruh.Components.SleepTracker");
        var end = Time.realtimeSinceStartup + seconds;
        var ship = FindObjectsByType<Ship>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).FirstOrDefault();
        while (Time.realtimeSinceStartup < end)
        {
            var pos = ship ? ship.transform.position : player.transform.position;
            var surface = Floating.GetLiquidLevel(pos);
            var hp = ship ? ship.GetComponent<WearNTear>().GetHealthPercentage() : 0f;
            var aboard = ship ? Traverse.Create(ship).Field("m_players").GetValue<System.Collections.ICollection>().Count : 0;
            lines.Add(string.Join(",", Time.realtimeSinceStartup.ToString("0.000"), ZNet.instance.GetTimeSeconds().ToString("0.00"), wrapped.GetValue(null), shader.GetValue(null), surface.ToString("0.000"), ship ? ship.transform.position.y.ToString("0.000") : "", hp.ToString("0.000"), aboard, Traverse.Create(st).Property("CurrentExtraRate").GetValue()));
            yield return null;
        }
        File.WriteAllLines(path, lines);
        Say("WAVELOG " + tag + " frames=" + (lines.Count - 1));
    }
    private static int _ticks;
    private static void BridgeTick() { if (++_ticks % 5 == 0) Say("TICK " + _ticks); }
    private static readonly Dictionary<string, ZDOID> _rig = new Dictionary<string, ZDOID>();
    private static GameObject Rig(string key) { ZDOID id; if (!_rig.TryGetValue(key, out id)) return null; var z = ZDOMan.instance.GetZDO(id); return z == null ? null : ZNetScene.instance.FindInstance(z)?.gameObject; }
    private void Update()    {
        Commands();
        if (ZInput.GetButtonDown("SecondaryAttack")) Say("PRESS SecondaryAttack");
        var gui = InventoryGui.instance;
        if (!gui) return;
        var container = Traverse.Create(gui).Field("m_currentContainer").GetValue<Container>();
        var state = "visible=" + InventoryGui.IsVisible() + " container=" + (container ? container.name : "none") + " overlay=" + _shardOpen();
        if (state != _lastGui) { Say("GUI " + state); _lastGui = state; }
        if (InventoryGui.IsVisible() && ZInput.GetButtonDown("Use"))
        {
            var m = Input.mousePosition;
            var vanilla = gui.m_playerGrid.GetItem(new Vector2i(Mathf.RoundToInt(m.x), Mathf.RoundToInt(m.y)));
            var cell = SeneaLUI.Inv.GridSkin.HoverCell;
            var skin = cell != null ? SeneaLUI.Inv.GridSkin.HoverInv?.GetItemAt(cell.X, cell.Y) : null;
            Say("PRESS Use mouse=" + m.x + "," + m.y + " vanilla=" + vanilla?.m_shared.m_name + " seneal=" + skin?.m_shared.m_name + " cell=" + (cell == null ? "none" : cell.X + "," + cell.Y));
        }
    }

    private void Commands()
    {
        var f = new FileInfo(_cmd);
        if (!f.Exists || f.Length <= _cmdPos) return;
        string text;
        using (var s = new FileStream(_cmd, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            s.Position = _cmdPos;
            text = new StreamReader(s).ReadToEnd();
            _cmdPos = s.Length;
        }
        foreach (var line in text.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0))
        {
            Say("CMD " + line);
            try { Run(line.Split(' ')); }
            catch (Exception e) { Say("CMD ERROR " + e); }
        }
    }

    private static Vector3 Near(float dx, float dz)
    {
        var p = Player.m_localPlayer.transform;
        var at = p.position + p.right * dx + p.forward * dz;
        at.y = ZoneSystem.instance.GetGroundHeight(at);
        return at;
    }

    private static GameObject Prefab(string n) { return ZNetScene.instance.GetPrefab(n) ?? throw new Exception("no prefab " + n); }

    private static ItemDrop.ItemData NewItem(string prefab, int stack, int quality = 1)
    {
        var d = Prefab(prefab).GetComponent<ItemDrop>().m_itemData.Clone();
        d.m_dropPrefab = Prefab(prefab);
        d.m_stack = stack;
        d.m_quality = quality;
        d.m_durability = d.GetMaxDurability();
        return d;
    }

    private void Run(string[] a)
    {
        var player = Player.m_localPlayer;
        switch (a[0])
        {
            case "give":
                Say("add=" + player.GetInventory().AddItem(NewItem(a[1], int.Parse(a[2]), a.Length > 3 ? int.Parse(a[3]) : 1)));
                break;
            case "magic": // prefab effect value sockets
            {
                var item = NewItem(a[1], 1);
                var mi = new MagicItem { Rarity = ItemRarity.Magic, SocketCount = int.Parse(a[4]) };
                if (a[2] != "none") mi.Effects.Add(new MagicItemEffect(a[2], float.Parse(a[3], System.Globalization.CultureInfo.InvariantCulture)));
                item.SaveMagicItem(mi);
                Say("add=" + player.GetInventory().AddItem(item) + " at " + item.m_gridPos);
                break;
            }
            case "inv":
                foreach (var i in player.GetInventory().GetAllItems())
                    Say("item " + i.m_gridPos + " " + (i.m_dropPrefab ? i.m_dropPrefab.name : i.m_shared.m_name) + " x" + i.m_stack + " q" + i.m_quality + " eq=" + i.m_equipped + " magic=" + MagicItemData.HasMagicData(i) + " data=" + string.Join("|", i.m_customData.Keys.Select(k => k.Split('.').Last())));
                break;
            case "clearinv":
                player.UnequipAllItems();
                player.GetInventory().RemoveAll();
                break;
            case "cell":
            {
                var el = Traverse.Create(InventoryGui.instance.m_playerGrid).Method("GetElement", int.Parse(a[1]), int.Parse(a[2]), player.GetInventory().GetWidth()).GetValue<InventoryElement>();
                var c = new Vector3[4];
                el.GetElementRectTransform().GetWorldCorners(c);
                var p = (c[0] + c[2]) / 2;
                Say("CELL " + a[1] + "," + a[2] + " unity=" + p.x + "," + p.y + " screenY=" + (Screen.height - p.y) + " active=" + el.gameObject.activeInHierarchy + " screen=" + Screen.width + "x" + Screen.height);
                break;
            }
            case "god": player.SetGodMode(true); player.SetGhostMode(true); break;
            case "nogod": player.SetGodMode(false); player.SetGhostMode(false); break;
            case "hp": player.SetHealth(float.Parse(a[1])); break;
            case "state":
                Say("STATE hp=" + player.GetHealth() + "/" + player.GetMaxHealth() + " foods=" + string.Join(",", player.GetFoods().Select(x => x.m_name + ":" + x.m_item.m_shared.m_foodRegen)) + " gear=" + player.GetTotalActiveMagicEffectValue("AddHealthRegen") + " pos=" + player.transform.position + " se=" + string.Join(",", player.GetSEMan().GetStatusEffects().Select(s => s.name)));
                break;
            case "spawn": // prefab right forward tame
            {
                var go = Instantiate(Prefab(a[1]), Near(float.Parse(a[2]), float.Parse(a[3])) + Vector3.up * 0.2f, Quaternion.identity);
                if (a.Length > 4 && a[4] == "1") go.GetComponent<Character>().SetTamed(true);
                Say("spawned " + go.name);
                break;
            }
            case "piece": // prefab right forward: placed flat on the terrain, as the hammer does
            {
                var go = Instantiate(Prefab(a[1]), Near(float.Parse(a[2]), float.Parse(a[3])), Quaternion.identity);
                go.GetComponent<ZNetView>().GetZDO().Set(ZDOVars.s_creator, player.GetPlayerID()); // what Piece.SetCreator writes on hammer placement
                go.GetComponent<WearNTear>()?.OnPlaced();
                Say("placed " + go.name + " at " + go.transform.position);
                break;
            }
            case "stock": // prefab count, nearest container
            {
                var c = FindObjectsByType<Container>(FindObjectsSortMode.None).Where(x => x.name.StartsWith("FLG_FeedBarrel")).OrderBy(x => (x.transform.position - player.transform.position).sqrMagnitude).First();
                Say("stock add=" + c.GetInventory().AddItem(Prefab(a[1]), int.Parse(a[2])) + " n=" + c.GetInventory().CountItems(Prefab(a[1]).GetComponent<ItemDrop>().m_itemData.m_shared.m_name));
                break;
            }
            case "own":
                foreach (var c in FindObjectsByType<Container>(FindObjectsSortMode.None).Where(x => x.name.StartsWith("FLG_FeedBarrel")))
                    c.GetComponent<ZNetView>().GetZDO().Set(ZDOVars.s_creator, player.GetPlayerID());
                break;
            case "barrel":
                foreach (var c in FindObjectsByType<Container>(FindObjectsSortMode.None).Where(x => x.name.StartsWith("FLG_FeedBarrel")))
                    Say("BARREL items=" + string.Join(",", c.GetInventory().GetAllItems().Select(i => i.m_shared.m_name + "x" + i.m_stack)) + " at " + c.transform.position);
                break;
            case "animals":
                foreach (var t in FindObjectsByType<Tameable>(FindObjectsSortMode.None).Where(x => x.GetComponent<Character>().IsTamed()))
                    Say("ANIMAL " + t.name + " hungry=" + t.IsHungry() + " dist=" + Vector3.Distance(t.transform.position, player.transform.position));
                break;
            case "drops":
                foreach (var d in FindObjectsByType<ItemDrop>(FindObjectsSortMode.None).Where(x => Vector3.Distance(x.transform.position, player.transform.position) < 30))
                {
                    var v = d.GetComponent<ZNetView>();
                    Say("DROP " + d.name + " x" + d.m_itemData.m_stack + " protected=" + (v.IsValid() && v.GetZDO().GetBool("flg.feedbarrel.food.v1".GetStableHashCode())) + " expiry=" + (d.m_itemData.m_customData.TryGetValue("sighsorry.FineDining.ExpiryWorldTicks", out var e) ? e : "none") + " magic=" + MagicItemData.RawOf(d.m_itemData)?.Length);
                }
                break;
            case "drop": // prefab count raw|- expiryOffsetSeconds|-
            {
                var d = Instantiate(Prefab(a[1]), Near(0, 2) + Vector3.up, Quaternion.identity).GetComponent<ItemDrop>();
                d.m_itemData.m_stack = int.Parse(a[2]);
                if (a[3] != "-") d.m_itemData.m_customData[MagicItemData.Key] = a[3];
                if (a[4] != "-") d.m_itemData.m_customData["sighsorry.FineDining.ExpiryWorldTicks"] = (ZNet.instance.GetTime().Ticks + long.Parse(a[4]) * TimeSpan.TicksPerSecond).ToString();
                if (a.Length > 5 && a[5] == "m") d.GetComponent<ZNetView>().GetZDO().Set("flg.feedbarrel.food.v1".GetStableHashCode(), true);
                Traverse.Create(d).Method("Save").GetValue();
                Say("dropped " + d.name + " data=" + string.Join("|", d.m_itemData.m_customData.Keys.Select(k => k.Split('.').Last())));
                break;
            }
            case "diag":
            {
                var ai = FindObjectsByType<MonsterAI>(FindObjectsSortMode.None).Where(m => m.GetComponent<Character>().IsTamed()).OrderBy(m => (m.transform.position - player.transform.position).sqrMagnitude).First();
                var c = FindObjectsByType<Container>(FindObjectsSortMode.None).Where(x => x.name.StartsWith("FLG_FeedBarrel")).OrderBy(x => (x.transform.position - ai.transform.position).sqrMagnitude).First();
                var sys = AccessTools.TypeByName("FeedLikeGrandma.FeedAutoFeedSystem");
                var pos = ai.transform.position;
                var range = Traverse.Create(ai).Field("m_consumeSearchRange").GetValue<float>();
                var sel = new object[] { ai, c, null };
                var dropArgs = new object[] { ai, c, pos, range, Vector3.zero };
                Say("DIAG " + ai.name + " range=" + range + " consumeRange=" + ai.m_consumeRange + " dist=" + Vector3.Distance(pos, c.transform.position) + " alerted=" + ai.IsAlerted() + " hungry=" + ai.GetComponent<Tameable>().IsHungry() + " owner=" + ai.GetComponent<ZNetView>().IsOwner() + " items=" + ai.m_consumeItems.Count);
                Say("DIAG registered=" + ((System.Collections.ICollection)AccessTools.Field(sys, "RegisteredFeedContainers").GetValue(null)).Count + " eligible=" + AccessTools.Method(sys, "IsEligibleContainer").Invoke(null, new object[] { c, pos, range * range }) + " food=" + AccessTools.Method(sys, "TrySelectFood").Invoke(null, sel) + " " + (sel[2] as ItemDrop.ItemData)?.m_shared.m_name + " drop=" + AccessTools.Method(sys, "TryGetReachableDropPoint").Invoke(null, dropArgs) + " " + dropArgs[4] + " havePath=" + Traverse.Create(ai).Method("HavePath", c.transform.position).GetValue<bool>());
                break;
            }
            case "dropstamped": // prefab offsetSeconds: real stamped inventory data, only the expiry moved
            {
                var src = player.GetInventory().GetAllItems().First(i => i.m_dropPrefab && i.m_dropPrefab.name == a[1] && i.m_customData.ContainsKey("sighsorry.FineDining.ExpiryWorldTicks"));
                var item = src.Clone();
                item.m_stack = 1;
                item.m_customData = new Dictionary<string, string>(src.m_customData);
                item.m_customData["sighsorry.FineDining.ExpiryWorldTicks"] = (ZNet.instance.GetTime().Ticks + long.Parse(a[2]) * TimeSpan.TicksPerSecond).ToString();
                var d = ItemDrop.DropItem(item, 1, Near(0, 2) + Vector3.up, Quaternion.identity);
                Say("dropstamped expiry=" + item.m_customData["sighsorry.FineDining.ExpiryWorldTicks"] + " now=" + ZNet.instance.GetTime().Ticks);
                break;
            }
            case "roof": // floors over the nearest station; decay removed because they float
            {
                var s = FindObjectsByType<CraftingStation>(FindObjectsSortMode.None).OrderBy(x => (x.transform.position - player.transform.position).sqrMagnitude).First();
                for (var dx = -4; dx <= 4; dx += 2)
                    for (var dz = -4; dz <= 4; dz += 2)
                    {
                        var r = Instantiate(Prefab("wood_floor"), s.transform.position + new Vector3(dx, 3.5f, dz), Quaternion.identity);
                        r.GetComponent<ZNetView>().GetZDO().Set(ZDOVars.s_creator, player.GetPlayerID());
                        Destroy(r.GetComponent<WearNTear>());
                    }
                Say("roof over " + s.name);
                break;
            }
            case "spawnat": // prefab: on the nearest lightning trap
            {
                var trap = FindObjectsByType<Aoe>(FindObjectsSortMode.None).OrderBy(x => (x.transform.position - player.transform.position).sqrMagnitude).First(x => !string.IsNullOrEmpty(x.m_statusEffect) || x.name.Contains("Trap") || x.transform.root.name.Contains("Trap"));
                var go = Instantiate(Prefab(a[1]), trap.transform.position + Vector3.up * 0.3f, Quaternion.identity);
                Say("spawnat " + trap.transform.root.name + " effect=" + trap.m_statusEffect + " " + go.name);
                break;
            }
            case "count": Say("COUNT " + a[1] + "=" + player.GetInventory().CountItems(Prefab(a[1]).GetComponent<ItemDrop>().m_itemData.m_shared.m_name)); break;
            case "noroof":
                foreach (var s in FindObjectsByType<CraftingStation>(FindObjectsSortMode.None)) { s.m_craftRequireRoof = false; s.m_craftRequireFire = false; Say("station " + s.name + " level=" + s.GetLevel()); }
                break;
            case "ammo": Say("AMMO arrows=" + player.GetInventory().CountItems(Prefab(a[1]).GetComponent<ItemDrop>().m_itemData.m_shared.m_name) + " stamina=" + player.GetStamina()); break;
            case "stamp": // prefab seconds: real inventory food, then the expiry is set through FineDining's own writer
            {
                var src = player.GetInventory().GetAllItems().First(i => i.m_dropPrefab && i.m_dropPrefab.name == a[1]);
                var item = src.Clone();
                item.m_stack = 1;
                item.m_customData = new Dictionary<string, string>(src.m_customData);
                SetClock(item, long.Parse(a[2]));
                var d = ItemDrop.DropItem(item, 1, Near(0, a.Length > 3 ? float.Parse(a[3]) : 2) + Vector3.up, Quaternion.identity);
                Say("stamped remaining=" + Remaining(item));
                break;
            }
            case "sleepstate":
            {
                var st = AccessTools.TypeByName("BetterSleepBruh.Components.SleepTracker");
                var hud = GameObject.Find("_GameMain")?.transform.Find("LoadingGUI/PixelFix/IngameGui/HUD/hudroot");
                var root = Minimap.instance.m_smallRoot.transform;
                var path = root.name; for (var x = root.parent; x != null; x = x.parent) path = x.name + "/" + path;
                var fns = Traverse.Create(ZRoutedRpc.instance).Field("m_functions").GetValue<System.Collections.IDictionary>();
                Say("SLEEP net=" + ZNet.instance.GetTimeSeconds().ToString("0.0") + " day=" + EnvMan.instance.GetDayFraction().ToString("0.0000") + " extra=" + Traverse.Create(st).Property("CurrentExtraRate").GetValue() + " boostTotal=" + Traverse.Create(st).Property("TotalPartialSleepBoostSeconds").GetValue() + " tracker=" + (Traverse.Create(st).Property("Instance").GetValue() != null));
                Say("SLEEP hudroot=" + (hud != null) + " hardcoded=" + (hud?.Find("MiniMap/small") != null) + " real=" + path + " rpcInfo=" + fns.Contains("RPC_SleepingPlayerInfo".GetStableHashCode()) + " rpcTotal=" + fns.Contains("BSB_SyncSleepBoostTotal".GetStableHashCode()) + " hud=" + (root.Find("BetterSleepBruh_SleepHud") != null));
                break;
            }
            case "hudattach": // test only: SleepHudView.TryCreate at Minimap's real small root
            {
                var view = AccessTools.TypeByName("BetterSleepBruh.Components.SleepHudView");
                Say("HUDATTACH " + (AccessTools.Method(view, "TryCreate").Invoke(null, new object[] { Minimap.instance.m_smallRoot.transform, 1f }) != null));
                break;
            }
            case "sleephud": // every SleepHudView, with screen rectangle
            {
                var vt = AccessTools.TypeByName("BetterSleepBruh.Components.SleepHudView");
                var views = Resources.FindObjectsOfTypeAll(vt).OfType<Component>().Where(v => v.gameObject.scene.IsValid()).ToList();
                var fns = Traverse.Create(ZRoutedRpc.instance).Field("m_functions").GetValue<System.Collections.IDictionary>();
                var main = BepInEx.Bootstrap.Chainloader.PluginInfos["vapok.mods.BetterSleepBruh"].Instance;
                var cur = Traverse.Create(main).Field("_sleepHud").GetValue() as Component;
                var names = new[] { "RPC_SleepingPlayerInfo", "RPC_StartSleep", "RPC_StopSleep", "BSB_SyncSleepBoostTotal" };
                var bound = names.Count(n => fns.Contains(n.GetStableHashCode()) && ReferenceEquals((AccessTools.Field(fns[n.GetStableHashCode()].GetType(), "m_action").GetValue(fns[n.GetStableHashCode()]) as Delegate)?.Target, cur));
                Say("HUDVIEWS count=" + views.Count + " current=" + (cur != null) + " rpcs=" + names.Count(n => fns.Contains(n.GetStableHashCode())) + " boundToCurrent=" + bound);
                foreach (var v in views)
                {
                    var c = new Vector3[4];
                    ((RectTransform)v.transform).GetWorldCorners(c);
                    var txt = v.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true).Select(x => x.text);
                    var segs = Traverse.Create(v).Field("_segments").GetValue<UnityEngine.UI.Image[]>();
                    Say("HUDVIEW id=" + v.GetInstanceID() + " lit=" + (segs == null ? -1 : segs.Count(g => g.color == Color.white)) + "/" + (segs?.Length ?? 0) + " active=" + v.gameObject.activeInHierarchy + " self=" + v.gameObject.activeSelf + " parent=" + v.transform.parent.name + " x=" + c[0].x.ToString("0") + ".." + c[2].x.ToString("0") + " y=" + (Screen.height - c[2].y).ToString("0") + ".." + (Screen.height - c[0].y).ToString("0") + " text=" + string.Join("|", txt) + " sameAsCurrent=" + (v == cur));
                }
                break;
            }
            case "rpcdrop": // test only: lose one handler on a healthy view
                Traverse.Create(ZRoutedRpc.instance).Field("m_functions").GetValue<System.Collections.IDictionary>().Remove("RPC_StopSleep".GetStableHashCode());
                Say("RPCDROP done");
                break;
            case "rpcforeign": // test only: another owner on one RPC name, then the view dies
            {
                var fns2 = Traverse.Create(ZRoutedRpc.instance).Field("m_functions").GetValue<System.Collections.IDictionary>();
                fns2.Remove("RPC_StartSleep".GetStableHashCode());
                ZRoutedRpc.instance.Register("RPC_StartSleep", (long s) => Say("FOREIGN handler ran"));
                var main2 = BepInEx.Bootstrap.Chainloader.PluginInfos["vapok.mods.BetterSleepBruh"].Instance;
                Destroy((Traverse.Create(main2).Field("_sleepHud").GetValue() as Component).gameObject);
                Say("RPCFOREIGN done");
                break;
            }
            case "viewkill": // test only: destroy the live view so BetterSleepBruh rebuilds it
            {
                var main = BepInEx.Bootstrap.Chainloader.PluginInfos["vapok.mods.BetterSleepBruh"].Instance;
                var cur = Traverse.Create(main).Field("_sleepHud").GetValue() as Component;
                if (cur != null) Destroy(cur.gameObject);
                Say("VIEWKILL " + (cur != null));
                break;
            }
            case "minimapel": // 0|1: SeneaL UI's own minimap element setting
            {
                var prefs = AccessTools.TypeByName("SeneaLUI.Core.Prefs") ?? AccessTools.AllTypes().First(x => x.Name == "Prefs" && x.Namespace != null && x.Namespace.StartsWith("SeneaLUI"));
                var el = Traverse.Create(prefs).Field("Minimap").GetValue();
                Traverse.Create(el).Field("On").Property("Value").SetValue(a[1] == "1");
                Say("MINIMAPEL " + a[1]);
                break;
            }
            case "scan": // client-owned timers and wave clock
            {
                var waveTime = Traverse.CreateWithType("WaterVolume").Field("s_waterTime").GetValue();
                var line = "SCAN real=" + Time.realtimeSinceStartup.ToString("0.0") + " net=" + ZNet.instance.GetTimeSeconds().ToString("0.0") + " wave=" + waveTime;
                foreach (var pl in FindObjectsByType<Plant>(FindObjectsSortMode.None).Take(1))
                {
                    var nv = pl.GetComponent<ZNetView>();
                    line += " plant owner=" + nv.IsOwner() + " grown=" + Traverse.Create(pl).Method("TimeSincePlanted").GetValue().ToString().Split('.')[0];
                }
                foreach (var sm in FindObjectsByType<Smelter>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).Take(1))
                {
                    var z = sm.GetComponent<ZNetView>().GetZDO();
                    line += " smelter owner=" + sm.GetComponent<ZNetView>().IsOwner() + " bake=" + z.GetFloat(ZDOVars.s_bakeTimer).ToString("0.0") + " ore=" + z.GetInt(ZDOVars.s_queued);
                }
                Say(line);
                break;
            }
            case "furnace": // right forward: a smelter loaded through its own RPCs
            {
                var go = Instantiate(Prefab("smelter"), Near(float.Parse(a[1]), float.Parse(a[2])), Quaternion.identity);
                var nv = go.GetComponent<ZNetView>();
                nv.GetZDO().Set(ZDOVars.s_creator, player.GetPlayerID());
                go.GetComponent<WearNTear>()?.OnPlaced();
                for (var n = 0; n < 10; n++) AccessTools.Method(typeof(Smelter), "RPC_AddOre").Invoke(go.GetComponent<Smelter>(), new object[] { 0L, "CopperOre", false });
                for (var n = 0; n < 20; n++) AccessTools.Method(typeof(Smelter), "RPC_AddFuel").Invoke(go.GetComponent<Smelter>(), new object[] { 0L });
                Say("furnace at " + go.transform.position);
                break;
            }
            case "rig": // oven, fermenter, sapling, pickable: real components, loaded through their own handlers
            {
                Func<string, float, float, GameObject> put = (name, r, f2) => {
                    var g = Instantiate(Prefab(name), Near(r, f2), Quaternion.identity);
                    g.GetComponent<ZNetView>().GetZDO().Set(ZDOVars.s_creator, player.GetPlayerID());
                    g.GetComponent<WearNTear>()?.OnPlaced();
                    _rig[name] = g.GetComponent<ZNetView>().GetZDO().m_uid;
                    return g;
                };
                var oven = put("piece_oven", -3f, 5f).GetComponent<CookingStation>();
                AccessTools.Method(typeof(CookingStation), "SetFuel").Invoke(oven, new object[] { 20f });
                AccessTools.Method(typeof(CookingStation), "RPC_AddItem").Invoke(oven, new object[] { 0L, oven.m_conversion[0].m_from.gameObject.name, false });
                var fer = put("fermenter", 3f, 5f).GetComponent<Fermenter>();
                AccessTools.Method(typeof(Fermenter), "RPC_AddItem").Invoke(fer, new object[] { 0L, fer.m_conversion[0].m_from.gameObject.name.GetStableHashCode(), false });
                put("sapling_turnip", 0f, 7f);
                put("RaspberryBush", 6f, 7f).GetComponent<Pickable>().SetPicked(true);
                Say("RIG done");
                break;
            }
            case "fillmead": // nearest fermenter, through its own handler
            {
                var fe2 = FindObjectsByType<Fermenter>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).First();
                AccessTools.Method(typeof(Fermenter), "RPC_AddItem").Invoke(fe2, new object[] { 0L, fe2.m_conversion[0].m_from.gameObject.name.GetStableHashCode(), false });
                Say("FILLMEAD " + fe2.m_conversion[0].m_from.gameObject.name + " time=" + Traverse.Create(fe2).Method("GetFermentationTime").GetValue());
                break;
            }
            case "fillbake": // nearest oven: clear every slot, fuel it, add dough through its own handler
            {
                var ov2 = FindObjectsByType<CookingStation>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).First();
                var setSlot = AccessTools.Method(typeof(CookingStation), "SetSlot");
                var notDone = Enum.ToObject(setSlot.GetParameters()[3].ParameterType, 0);
                for (var n = 0; n < ov2.m_slots.Length; n++) setSlot.Invoke(ov2, new object[] { n, "", 0f, notDone, false, 0L });
                AccessTools.Method(typeof(CookingStation), "SetFuel").Invoke(ov2, new object[] { 20f });
                AccessTools.Method(typeof(CookingStation), "RPC_AddItem").Invoke(ov2, new object[] { 0L, "BreadDough", false });
                Say("FILLBAKE done");
                break;
            }
            case "rigscan":
            {
                var line = "RIG real=" + Time.realtimeSinceStartup.ToString("0.0") + " net=" + ZNet.instance.GetTimeSeconds().ToString("0.0");
                var ov = FindObjectsByType<CookingStation>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).FirstOrDefault();
                if (ov != null) { var args = new object[] { 0, null, 0f, null, false }; AccessTools.Method(typeof(CookingStation), "GetSlot").Invoke(ov, args); line += " oven=" + ((float)args[2]).ToString("0.0") + "/" + args[1] + " own=" + ov.GetComponent<ZNetView>().IsOwner(); }
                var fe = FindObjectsByType<Fermenter>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).FirstOrDefault();
                if (fe != null) line += " mead=" + Traverse.Create(fe).Method("GetFermentationTime").GetValue().ToString().Split('.')[0] + " own=" + fe.GetComponent<ZNetView>().IsOwner();
                var pl = FindObjectsByType<Plant>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).FirstOrDefault();
                if (pl != null) line += " crop=" + Traverse.Create(pl).Method("TimeSincePlanted").GetValue().ToString().Split('.')[0] + " own=" + pl.GetComponent<ZNetView>().IsOwner();
                var pk = FindObjectsByType<Pickable>(FindObjectsSortMode.None).Where(x => x.m_respawnTimeMinutes > 0f && x.GetComponent<ZNetView>().GetZDO().GetLong(ZDOVars.s_pickedTime, 0L) > 1).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).FirstOrDefault();
                if (pk != null) { Traverse.Create(pk).Method("ShouldRespawn").GetValue(); var pz = pk.GetComponent<ZNetView>().GetZDO(); line += " pick=" + ((ZNet.instance.GetTime().Ticks - pz.GetLong(ZDOVars.s_pickedTime, 0L)) / TimeSpan.TicksPerSecond) + " own=" + pk.GetComponent<ZNetView>().IsOwner(); }
                Say(line);
                break;
            }
            case "boat": // Karve under the player, who stays aboard
            {
                var g = Instantiate(Prefab("Karve"), player.transform.position + Vector3.down * 0.2f, player.transform.rotation);
                g.GetComponent<ZNetView>().GetZDO().Set(ZDOVars.s_creator, player.GetPlayerID());
                _rig["Karve"] = g.GetComponent<ZNetView>().GetZDO().m_uid;
                Say("BOATSPAWN at " + g.transform.position);
                break;
            }
            case "aboard": // puts the player on the nearest ship's deck; the ship's own trigger registers them
            {
                var sh = FindObjectsByType<Ship>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).First();
                var bodyPos = sh.transform.position + Vector3.up * 1f;
                var rb = Traverse.Create(player).Field("m_body").GetValue<Rigidbody>();
                rb.position = bodyPos;
                rb.linearVelocity = Vector3.zero;
                Say("ABOARD " + sh.transform.position);
                break;
            }
            case "boatscan":
            {
                var s = FindObjectsByType<Ship>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).FirstOrDefault();
                if (s == null) { Say("BOAT missing"); break; }
                var hp = s.GetComponent<WearNTear>().GetHealthPercentage();
                Say("BOAT " + s.name + " hp=" + hp.ToString("0.000") + " players=" + Traverse.Create(s).Field("m_players").GetValue<System.Collections.ICollection>().Count + " lastImpact=" + Traverse.Create(s).Field("m_lastWaterImpactTime").GetValue() + " y=" + s.transform.position.y.ToString("0.00") + " now=" + Time.time.ToString("0"));
                break;
            }
            case "bed": // 1|0: real Bed attach path on a placed bed, so BetterSleepBruh sees real occupancy
            {
                if (a[1] == "1")
                {
                    var g = Instantiate(Prefab("bed"), Near(-2f, 2f), Quaternion.identity);
                    var bedc = g.GetComponent<Bed>();
                    player.AttachStart(bedc.m_spawnPoint, g, hideWeapons: true, isBed: true, onShip: false, "attach_bed", new Vector3(0f, 0.5f, 0f));
                    Say("BED in, inBed=" + player.InBed());
                }
                else { player.AttachStop(); Say("BED out, inBed=" + player.InBed()); }
                break;
            }
            case "wave": // per-frame recording: seconds tag
                StartCoroutine(WaveLog(float.Parse(a[1], System.Globalization.CultureInfo.InvariantCulture), a[2], player));
                break;
            case "wavefix": // 0|1: remove or restore PoVPlus.Compat's once-per-frame wave step, for a matched comparison
            {
                var target = AccessTools.Method(typeof(WaterVolume), "StaticUpdate");
                var mine = Harmony.GetPatchInfo(target)?.Prefixes.FirstOrDefault(p => p.PatchMethod.Name == "OneWaveStepPerFrame");
                if (a[1] == "0" && mine != null) { _waveFix = mine.PatchMethod; new Harmony("probe.wave").Unpatch(target, mine.PatchMethod); }
                if (a[1] == "1" && _waveFix != null) new Harmony("povplusplus.bettersleephudbridge").Patch(target, prefix: new HarmonyMethod(_waveFix));
                Say("WAVEFIX " + a[1] + " had=" + (mine != null));
                break;
            }
            case "lockcheck":
            {
                var ws = AccessTools.TypeByName("PassiveTree.ConfigManagerWorldSettings");
                var locked = AccessTools.Method(ws, "Locked");
                var isServer = AccessTools.Method("PassiveTree.RuntimeSettings:IsServerEntry");
                var pov = BepInEx.Bootstrap.Chainloader.PluginInfos["treextr.pathofvalheiman"].Instance.Config;
                var srv = pov.Keys.Select(k => pov[k]).FirstOrDefault(e => (bool)isServer.Invoke(null, new object[] { e }));
                var cli = pov.Keys.Select(k => pov[k]).FirstOrDefault(e => !(bool)isServer.Invoke(null, new object[] { e }) && !(bool)locked.Invoke(null, new object[] { e, true }) && e.SettingType == typeof(bool));
                var client = (bool)AccessTools.PropertyGetter(AccessTools.TypeByName("PassiveTree.DifficultyAdmin"), "IsClient").Invoke(null, null);
                Say("LOCK isClient=" + client + " EntryType=" + (Traverse.Create(ws).Field("EntryType").GetValue() != null) + " patches=" + (Harmony.GetPatchInfo(AccessTools.Method(AccessTools.TypeByName("ConfigurationManager.ConfigSettingEntry"), "Get"))?.Owners.Count ?? 0));
                var cm = AccessTools.TypeByName("ConfigurationManager.PathOfValheimanCompatibility");
                Say("LOCK cm bound=" + (Traverse.Create(cm).Field("_locked").GetValue() != null) + " native=" + Traverse.Create(cm).Field("_nativeGet").GetValue() + " faulted=" + Traverse.Create(cm).Field("_faulted").GetValue());
                var povInfo = BepInEx.Bootstrap.Chainloader.PluginInfos["treextr.pathofvalheiman"].Instance;
                foreach (var e in new[] { srv, cli })
                {
                    var entry = AccessTools.Constructor(AccessTools.TypeByName("ConfigurationManager.ConfigSettingEntry"), new[] { typeof(BepInEx.Configuration.ConfigEntryBase), typeof(BepInEx.BaseUnityPlugin) }).Invoke(new object[] { e, povInfo });
                    Traverse.Create(entry).Property("PluginInfo").SetValue(povInfo.Info.Metadata);
                    var before = e.BoxedValue;
                    Say("LOCK entry " + e.Definition.Key + " server=" + isServer.Invoke(null, new object[] { e }) + " lockedClient=" + locked.Invoke(null, new object[] { e, true }) + " cmLocked=" + AccessTools.Method(cm, "IsLocked").Invoke(null, new[] { entry }) + " cmBlock=" + AccessTools.Method(cm, "ShouldBlockSet").Invoke(null, new[] { entry }) + " value=" + before);
                    if (e.SettingType == typeof(bool))
                    {
                        AccessTools.Method(entry.GetType(), "SetValue").Invoke(entry, new object[] { !(bool)before });
                        Say("LOCK set attempt " + e.Definition.Key + " " + before + " -> " + e.BoxedValue);
                        e.BoxedValue = before;
                    }
                }
                break;
            }
            case "seneal":
            {
                var pv = AccessTools.AllTypes().First(x => x.FullName == "SeneaLUI.Mods.Pov");
                Say("SENEAL On=" + Traverse.Create(pv).Property("On").GetValue() + " Dual=" + Traverse.Create(pv).Property("DualUtility").GetValue() + " Second=" + Traverse.Create(pv).Property("SecondUtility").GetValue() + " utilEquipped=" + player.GetInventory().GetAllItems().Count(i => i.m_equipped && i.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Utility));
                var api = AccessTools.TypeByName("Valheiman.Api.Equipment");
                if (api != null) Say("POVGEAR On=" + AccessTools.Property(api, "On").GetValue(null) + " cells=" + ((System.Collections.ICollection)AccessTools.Property(api, "Cells").GetValue(null)).Count + " rings=" + player.GetInventory().GetAllItems().Count(i => i.m_equipped));
                break;
            }
            case "guistate":
            {
                var q = AccessTools.TypeByName("BowsBeforeHoes.QuiverFunctionality.QuiverContainerGui");
                var cont = Traverse.Create(InventoryGui.instance).Field("m_container").GetValue<RectTransform>();
                Say("GUISTATE quiverOpen=" + (q == null ? "?" : AccessTools.Property(q, "IsOpen")?.GetValue(null)) + " container=" + (cont && cont.gameObject.activeInHierarchy));
                break;
            }
            case "chippos": // on-screen position of a visible text label
            {
                foreach (var label in Resources.FindObjectsOfTypeAll<TMPro.TextMeshProUGUI>().Where(x => x.gameObject.activeInHierarchy && x.text != null && x.text.IndexOf(a[1], StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    var c = new Vector3[4];
                    label.rectTransform.GetWorldCorners(c);
                    Say("CHIP " + a[1] + " x=" + ((c[0].x + c[2].x) / 2).ToString("0") + " y=" + (Screen.height - (c[0].y + c[2].y) / 2).ToString("0"));
                }
                var vis = AccessTools.Field(typeof(Minimap), "m_visibleIconTypes").GetValue(Minimap.instance) as bool[];
                Say("CHIPVIS bounty=" + vis[(int)EpicLoot.EpicLoot.BountyPinType] + " treasure=" + vis[(int)EpicLoot.EpicLoot.TreasureMapPinType]);
                break;
            }
            case "epicfilter":
            {
                var mc = AccessTools.TypeByName("EpicLoot.Adventure.MinimapController");
                foreach (var name in new[] { "_bountyPinFilter", "_treasurePinFilter" })
                {
                    var flt = AccessTools.Field(mc, name).GetValue(null);
                    if (flt == null) { Say("FILTER " + name + " null"); continue; }
                    var go = Traverse.Create(flt).Field("instance").GetValue<GameObject>();
                    var c = new Vector3[4];
                    go.GetComponent<RectTransform>()?.GetWorldCorners(c);
                    Say("FILTER " + name + " self=" + go.activeSelf + " hierarchy=" + go.activeInHierarchy + " visiblePins=" + Traverse.Create(flt).Property("Visible").GetValue() + " at " + c[0].x.ToString("0") + "," + (Screen.height - c[2].y).ToString("0"));
                }
                var em = AccessTools.AllTypes().First(x => x.FullName == "SeneaLUI.Mods.EpicMap");
                Say("FILTER seneal Enabled=" + AccessTools.Property(em, "Enabled").GetValue(null) + " large=" + Minimap.instance.m_largeRoot.activeInHierarchy);
                break;
            }
            case "mapedge": // 1 moves SeneaL's minimap to the left screen edge for the placement check, 0 restores it
            {
                var root = Minimap.instance.m_smallRoot.transform;
                var mask = root.parent.GetComponentInParent<UnityEngine.UI.Mask>();
                var rt = (mask ? mask.transform.parent : root) as RectTransform;
                var c = new Vector3[4];
                rt.GetWorldCorners(c);
                if (a[1] == "1") { _mapShift = -c[0].x + 20f; rt.position += new Vector3(_mapShift, 0f, 0f); }
                else { rt.position -= new Vector3(_mapShift, 0f, 0f); }
                Say("MAPEDGE " + a[1] + " shift=" + _mapShift);
                break;
            }
            case "dfcheck":
            {
                var hammer = Prefab("Hammer").GetComponent<ItemDrop>().m_itemData.m_shared.m_buildPieces;
                var hoe = Prefab("Hoe").GetComponent<ItemDrop>().m_itemData.m_shared.m_buildPieces;
                var r = ObjectDB.instance.m_recipes;
                Say("DF recipes=" + r.Count + " nullRecipes=" + r.Count(x => !x) + " hammerPieces=" + hammer.m_pieces.Count + " nullHammer=" + hammer.m_pieces.Count(x => !x) + " hoePieces=" + hoe.m_pieces.Count + " items=" + ObjectDB.instance.m_items.Count + " forge=" + hammer.m_pieces.Count(x => x && x.name == "forge") + " dirtyHoe=" + hoe.m_pieces.Count(x => x && x.name.StartsWith("mm_")) + " hash=" + string.Join(",", r.Where(x => x && x.m_item).Select(x => x.name).OrderBy(x => x)).GetStableHashCode());
                break;
            }
            case "craft": // recipe item: the game's own craft path with inventory resources
            {
                var rec = ObjectDB.instance.m_recipes.First(x => x && x.m_item && x.m_item.name == a[1]);
                var gui = InventoryGui.instance;
                foreach (var res in rec.m_resources) if (res.m_resItem) player.GetInventory().AddItem(NewItem(res.m_resItem.gameObject.name, res.m_amount * 2));
                var before = player.GetInventory().CountItems(rec.m_item.m_itemData.m_shared.m_name);
                var noCost = player.NoCostCheat();
                Traverse.Create(gui).Field("m_craftRecipe").SetValue(rec);
                Traverse.Create(gui).Field("m_craftVariant").SetValue(0);
                Traverse.Create(gui).Field("m_multiCraftAmount").SetValue(1);
                AccessTools.Method(typeof(InventoryGui), "DoCrafting").Invoke(gui, new object[] { player });
                Say("CRAFT " + a[1] + " " + before + " -> " + player.GetInventory().CountItems(rec.m_item.m_itemData.m_shared.m_name) + " noCost=" + noCost);
                break;
            }
            case "placetest": // prefab right forward: the game's own PlacePiece from the hammer's table
            {
                var hm = Prefab("Hammer").GetComponent<ItemDrop>().m_itemData.m_shared.m_buildPieces;
                var pc = hm.m_pieces.First(x => x && x.name == a[1]).GetComponent<Piece>();
                var pos = Near(float.Parse(a[2]), float.Parse(a[3]));
                foreach (var res in pc.m_resources) if (res.m_resItem) player.GetInventory().AddItem(NewItem(res.m_resItem.gameObject.name, res.m_amount * 2));
                var ok = AccessTools.Method(typeof(Player), "PlacePiece").GetParameters().Length;
                var count = FindObjectsByType<Piece>(FindObjectsSortMode.None).Count(x => x.name.StartsWith(a[1]));
                AccessTools.Method(typeof(Player), "PlacePiece").Invoke(player, new object[] { pc, pos, Quaternion.identity, false, false });
                Say("PLACE " + a[1] + " params=" + ok + " " + count + " -> " + FindObjectsByType<Piece>(FindObjectsSortMode.None).Count(x => x.name.StartsWith(a[1])));
                break;
            }
            case "portalstate":
            {
                var c = AccessTools.TypeByName("PortalRules.PublicPortalCatalog");
                Say("PORTAL revision=" + Traverse.Create(c).Field("_clientRevision").GetValue() + " hasSnapshot=" + Traverse.Create(c).Field("_hasSnapshot").GetValue() + " entries=" + ((System.Collections.ICollection)Traverse.Create(c).Field("ClientEntries").GetValue()).Count);
                break;
            }
            case "ccell": // x y: container grid slot on screen
            {
                var grid = Traverse.Create(InventoryGui.instance).Field("m_containerGrid").GetValue<InventoryGrid>();
                var inv = Traverse.Create(InventoryGui.instance).Field("m_currentContainer").GetValue<Container>()?.GetInventory();
                if (inv == null) { Say("CCELL no container open"); break; }
                var el = Traverse.Create(grid).Method("GetElement", int.Parse(a[1]), int.Parse(a[2]), inv.GetWidth()).GetValue<InventoryElement>();
                var c = new Vector3[4];
                el.GetElementRectTransform().GetWorldCorners(c);
                var p = (c[0] + c[2]) / 2;
                Say("CELL " + a[1] + "," + a[2] + " unity=" + p.x + "," + p.y + " screenY=" + (Screen.height - p.y));
                break;
            }
            case "invset": // prefab seconds: FineDining's own writer on an inventory stack
                SetClock(player.GetInventory().GetAllItems().First(i => i.m_dropPrefab && i.m_dropPrefab.name == a[1]), long.Parse(a[2]));
                break;
            case "chestset": // seconds: first stack of the open or nearest chest
            {
                var c = FindObjectsByType<Container>(FindObjectsSortMode.None).Where(x => x.name.StartsWith("piece_chest_wood")).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).First();
                SetClock(c.GetInventory().GetAllItems().First(), long.Parse(a[1]));
                Traverse.Create(c).Method("Save").GetValue();
                break;
            }
            case "stacks": // inventory and chest stacks with full custom data
            {
                foreach (var i in player.GetInventory().GetAllItems().Where(x => !x.m_equipped))
                    Say("STACK inv " + i.m_gridPos + " " + i.m_dropPrefab?.name + " x" + i.m_stack + " rem=" + Remaining(i) + " data=" + string.Join("|", i.m_customData.Select(k => k.Key.Split('.').Last() + "=" + k.Value)));
                foreach (var c in FindObjectsByType<Container>(FindObjectsSortMode.None).Where(x => x.name.StartsWith("piece_chest_wood")))
                    foreach (var i in c.GetInventory().GetAllItems())
                        Say("STACK chest " + i.m_gridPos + " " + i.m_dropPrefab?.name + " x" + i.m_stack + " rem=" + Remaining(i) + " data=" + string.Join("|", i.m_customData.Select(k => k.Key.Split('.').Last() + "=" + k.Value)));
                break;
            }
            case "chestfood": // prefab seconds: real stamped food in a placed chest
            {
                var go = Instantiate(Prefab("piece_chest_wood"), Near(3, 3), Quaternion.identity);
                go.GetComponent<ZNetView>().GetZDO().Set(ZDOVars.s_creator, player.GetPlayerID());
                go.GetComponent<WearNTear>()?.OnPlaced();
                var c = go.GetComponent<Container>();
                var src = player.GetInventory().GetAllItems().First(i => i.m_dropPrefab && i.m_dropPrefab.name == a[1]);
                var item = src.Clone();
                item.m_stack = 2;
                item.m_customData = new Dictionary<string, string>(src.m_customData);
                SetClock(item, long.Parse(a[2]));
                Say("chest add=" + c.GetInventory().AddItem(item) + " at " + go.transform.position);
                Traverse.Create(c).Method("Save").GetValue();
                break;
            }
            case "timers":
            {
                foreach (var i in player.GetInventory().GetAllItems().Where(x => x.m_customData.ContainsKey("sighsorry.FineDining.ExpiryWorldTicks")))
                    Say("TIMER inv " + i.m_dropPrefab?.name + " x" + i.m_stack + " rem=" + Remaining(i));
                foreach (var c in FindObjectsByType<Container>(FindObjectsSortMode.None).Where(x => x.name.StartsWith("piece_chest_wood")))
                    foreach (var i in c.GetInventory().GetAllItems())
                        Say("TIMER chest " + i.m_dropPrefab?.name + " x" + i.m_stack + " rem=" + Remaining(i));
                foreach (var d in FindObjectsByType<ItemDrop>(FindObjectsSortMode.None).Where(x => x.m_itemData.m_customData.ContainsKey("sighsorry.FineDining.ExpiryWorldTicks")))
                {
                    var v = d.GetComponent<ZNetView>();
                    Say("TIMER ground " + d.name + " x" + d.m_itemData.m_stack + " rem=" + Remaining(d.m_itemData) + " protected=" + (v.IsValid() && v.GetZDO().GetBool("flg.feedbarrel.food.v1".GetStableHashCode())));
                }
                Say("TIMER worldnow=" + ZNet.instance.GetTime().Ticks);
                break;
            }
            case "roll": // prefab rarity: real EpicLoot roll, added to the inventory
            {
                var item = NewItem(a[1], 1);
                Say("roll=" + API.TryMakeMagicItem(item, int.Parse(a[2]), 1f, null) + " raw=" + MagicItemData.RawOf(item)?.Length);
                player.GetInventory().AddItem(item);
                break;
            }
            case "rollfor": // prefab effect: real rolls until the effect appears
            {
                for (var n = 0; n < 4000; n++)
                {
                    var item = NewItem(a[1], 1);
                    if (!API.TryMakeMagicItem(item, 3, 1f, null)) continue;
                    var value = item.GetMagicItem()?.GetTotalEffectValue(a[2]) ?? 0f;
                    if (value <= 0) continue;
                    Say("rollfor tries=" + (n + 1) + " value=" + value + " at " + player.GetInventory().AddItem(item));
                    break;
                }
                break;
            }
            case "droproll": // prefab rarity: two real rolls dropped side by side
                for (var n = 0; n < 2; n++)
                {
                    var item = NewItem(a[1], 1);
                    API.TryMakeMagicItem(item, int.Parse(a[2]), 1f, null);
                    ItemDrop.DropItem(item, 1, Near(0, 2.2f) + Vector3.up, Quaternion.identity);
                    Say("rolled " + a[1] + " rawHash=" + MagicItemData.RawOf(item)?.GetHashCode());
                }
                break;
            case "rollraw": // stackable: real rolled JSON from a sword placed on a stackable. copies=1 same, 0 different
            {
                Func<string> raw = () => { var s = NewItem("SwordIron", 1); API.TryMakeMagicItem(s, 2, 1f, null); return MagicItemData.RawOf(s); };
                var r1 = raw();
                var r2 = a[2] == "1" ? r1 : raw();
                foreach (var r in new[] { r1, r2 })
                {
                    var d = Instantiate(Prefab(a[1]), Near(0, 2) + Vector3.up, Quaternion.identity).GetComponent<ItemDrop>();
                    d.m_itemData.m_stack = 1;
                    d.m_itemData.m_customData[MagicItemData.Key] = r;
                    Traverse.Create(d).Method("Save").GetValue();
                }
                Say("rollraw differs=" + (r1 != r2));
                break;
            }
            case "cfg": Say("feedbarrel=" + Traverse.CreateWithType("FeedLikeGrandma.FeedBarrelSystem").Property("IsEnabled").GetValue()); break;
            case "liveroll":
            {
                var c = Character.GetAllCharacters().FirstOrDefault(x => x.name.StartsWith(a[1]));
                var cd = c != null ? c.GetComponent<CharacterDrop>() : null;
                if (cd == null) { Say("ROLL no live " + a[1]); break; }
                var counts = new Dictionary<string, int>();
                var generate = AccessTools.Method(typeof(CharacterDrop), "GenerateDropList");
                var rolls = int.Parse(a[2]);
                for (var n = 0; n < rolls; n++)
                    foreach (var kv in (List<KeyValuePair<GameObject, int>>)generate.Invoke(cd, null))
                        counts[kv.Key.name] = (counts.TryGetValue(kv.Key.name, out var v) ? v : 0) + 1;
                Say("ROLL " + a[1] + " level=" + c.GetLevel() + " rolls=" + rolls + " " + string.Join(", ", counts.Select(kv => kv.Key + "=" + kv.Value)));
                break;
            }
            case "dbmissing":
                foreach (var name in _dbMissing)
                {
                    var hash = name.GetStableHashCode();
                    Say("DBCHECK " + name + " objectDB=" + (ObjectDB.instance.GetItemPrefab(hash) != null) + " listed=" + ObjectDB.instance.m_items.Any(x => x && x.name == name) + " scene=" + (ZNetScene.instance.GetPrefab(hash) != null) + " recipes=" + ObjectDB.instance.m_recipes.Count(r => r != null && r.m_item != null && r.m_item.gameObject.name == name) + " total=" + ObjectDB.instance.m_items.Count);
                }
                if (_dbMissing.Count == 0) Say("DBCHECK none missing, total=" + ObjectDB.instance.m_items.Count);
                break;
            case "fdclock":
            {
                var clock = AccessTools.TypeByName("FineDining.SpoilageClock");
                var world = new object[] { 0L };
                var hasWorld = (bool)AccessTools.Method(clock, "TryGetWorldTicks").Invoke(null, world);
                Say("FDCLOCK world=" + hasWorld + " ticks=" + world[0]);
                foreach (var i in player.GetInventory().GetAllItems())
                {
                    var rule = Traverse.Create(AccessTools.Method("FineDining.SpoilagePolicy:Resolve").Invoke(null, new object[] { i }));
                    var args = new object[] { i, world[0], 0L, false };
                    var has = (bool)AccessTools.Method(clock, "TryGetSpoilageClock").Invoke(null, args);
                    Say("FDCLOCK " + (i.m_dropPrefab ? i.m_dropPrefab.name : i.m_shared.m_name) + " x" + i.m_stack + " state=" + rule.Property("State").GetValue() + " group=" + rule.Property("Group").GetValue() + " ruleHours=" + (rule.Property("LifetimeTicks").GetValue<long>() / 36000000000.0).ToString("F1") + " rot=" + rule.Property("ReplacementPrefab").GetValue() + " hasClock=" + has + " remainingHours=" + ((long)args[2] / 36000000000.0).ToString("F2") + " paused=" + args[3]);
                }
                break;
            }
            case "fdconv":
                foreach (var piece in new[] { "piece_cookingstation", "piece_cookingstation_iron", "piece_oven" })
                {
                    var s = ZNetScene.instance.GetPrefab(piece)?.GetComponent<CookingStation>();
                    Say("FDCONV " + piece + " total=" + (s == null ? -1 : s.m_conversion.Count) + " match=" + (s == null ? "" : string.Join(",", s.m_conversion.Where(x => x.m_from != null && a.Skip(1).Contains(x.m_from.name)).Select(x => x.m_from.name + ">" + (x.m_to != null ? x.m_to.name : "null")))));
                }
                break;
            case "pickall":
            {
                var drops = UnityEngine.Object.FindObjectsByType<ItemDrop>(FindObjectsSortMode.None).Where(x => Vector3.Distance(x.transform.position, player.transform.position) < 20f).ToList();
                var taken = 0;
                foreach (var x in drops) if (player.Pickup(x.gameObject, false, false)) taken++;
                Say("PICKALL near=" + drops.Count + " taken=" + taken);
                break;
            }
            case "fdstation":
            {
                var pos = player.transform.position + player.transform.forward * 3f;
                var go = UnityEngine.Object.Instantiate(ZNetScene.instance.GetPrefab("piece_cookingstation_iron"), pos, Quaternion.identity);
                var st = go.GetComponent<CookingStation>();
                var allowed = AccessTools.Method(typeof(CookingStation), "IsItemAllowed", new[] { typeof(string) });
                Say("FDSTATION instance conversions=" + st.m_conversion.Count + " " + string.Join(", ", a.Skip(1).Select(n => n + "=" + allowed.Invoke(st, new object[] { n }))));
                ZNetScene.instance.Destroy(go);
                break;
            }
            case "fdcounts": Say("CONVT now " + ConvCounts()); break;
            case "fdrule":
                foreach (var name in a.Skip(1))
                {
                    var item = ObjectDB.instance.GetItemPrefab(name)?.GetComponent<ItemDrop>()?.m_itemData;
                    if (item == null) { Say("FDRULE " + name + " no item"); continue; }
                    var rule = AccessTools.Method("FineDining.SpoilagePolicy:Resolve").Invoke(null, new object[] { item });
                    var tr = Traverse.Create(rule);
                    Say("FDRULE " + name + " state=" + tr.Property("State").GetValue() + " hours=" + (tr.Property("LifetimeTicks").GetValue<long>() / 36000000000.0).ToString("F1") + " group=" + tr.Property("Group").GetValue() + " rot=" + tr.Property("ReplacementPrefab").GetValue());
                }
                break;
            case "pcatalog": Say("PORTAL " + PortalState()); break;
            case "ewdstate":
            {
                var value = Traverse.CreateWithType("ExpandWorldData.Configuration").Field("valueLocationClientData").Property("Value").GetValue<string>();
                Say("EWD value=" + (value == null ? "null" : "len" + value.Length) + " hasData=" + Traverse.CreateWithType("ExpandWorldData.Locations.LocationClientData").Property("HasData").GetValue());
                break;
            }
            case "dropcheck":
            {
                var drop = ZNetScene.instance.GetPrefab(a[1])?.GetComponent<CharacterDrop>();
                Say("DROPS " + a[1] + ": " + (drop == null ? "none" : string.Join(", ", drop.m_drops.Select(x => (x.m_prefab != null ? x.m_prefab.name : "null") + " " + x.m_amountMin + "-" + x.m_amountMax + " @" + x.m_chance))));
                break;
            }
            case "aurastate":
            {
                foreach (var c in Character.GetAllCharacters().Where(c => !c.IsPlayer() && Vector3.Distance(c.transform.position, player.transform.position) < 40f))
                {
                    var z = c.GetComponent<ZNetView>()?.GetZDO();
                    Say("AURA " + c.name + " dist=" + Vector3.Distance(c.transform.position, player.transform.position).ToString("F1") + " mask=" + z?.GetLong("CreatureManager_ModifierMask64", 0L) + " power=" + z?.GetFloat("CreatureManager_ToxicAuraPower", -1f) + " owner=" + (z != null && z.IsOwner()) + " tamed=" + c.IsTamed());
                }
                Say("AURA player hp=" + player.GetHealth().ToString("F1") + "/" + player.GetMaxHealth().ToString("F1") + " status=" + string.Join(",", player.GetSEMan().GetStatusEffects().Select(s => s.name)));
                break;
            }
            case "aurago":
            {
                var c = Character.GetAllCharacters().Where(x => !x.IsPlayer()).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).FirstOrDefault();
                if (c != null) player.transform.position = c.transform.position + c.transform.forward * 1.5f;
                Say("AURAGO " + (c != null ? c.name : "none"));
                break;
            }
            default: Say("unknown"); break;
        }
    }
}

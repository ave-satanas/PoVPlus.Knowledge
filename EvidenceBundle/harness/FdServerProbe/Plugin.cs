// Sanitized snapshot for reading. Generated from tools/FdServerProbe/Plugin.cs.
using System;
using System.Collections;
using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;

// Test only, dedicated server instance only. Reports FineDining's rule resolution and any timer it writes here. Changes nothing it measures.
[BepInPlugin("0.fdserverprobe", "FdServerProbe", "1.0.0")]
public class FdServerProbe : BaseUnityPlugin
{
    private static readonly string[] Foods = { "CookedBearSteak_TW", "CookedFoxMeat_TW", "CookedSharkMeat_TW", "BerryCakePlains_TW", "BearSteak_TW", "FoxMeat_TW", "SharkMeat_TW", "UncookedBerryCakePlains_TW", "CookedMeat", "NeckTailGrilled", "Raspberry", "RawMeat" };
    private static StreamWriter _log;
    private static void Say(string s) { _log.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " " + s); _log.Flush(); }

    private void Start()
    {
        _log = new StreamWriter(Path.Combine(Paths.BepInExRootPath, "fdserver.txt"), false);
        var h = new Harmony("0.fdserverprobe");
        h.Patch(AccessTools.Method("FineDining.DecayRuntime:EnsureItemState"), postfix: new HarmonyMethod(typeof(FdServerProbe), nameof(Stamp)));
        h.Patch(AccessTools.Method("FineDining.DecayRuntime:ExpireItem"), prefix: new HarmonyMethod(typeof(FdServerProbe), nameof(Expire)));
        BindConvTimeline(h);
        StartCoroutine(Run());
    }

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
        h.Patch(awake, prefix: new HarmonyMethod(typeof(FdServerProbe), nameof(SceneBefore)) { priority = Priority.First }, postfix: new HarmonyMethod(typeof(FdServerProbe), nameof(SceneAfter)) { priority = -1000 });
        var vc = AccessTools.TypeByName("ValheimCuisine.ValheimCuisinePlugin+ConversionsZNetScene_AwakePost_Patch");
        var reapply = vc == null ? null : AccessTools.DeclaredMethod(vc, "ReapplyConversions");
        if (reapply != null) h.Patch(reapply, prefix: new HarmonyMethod(typeof(FdServerProbe), nameof(ReapplyBefore)), postfix: new HarmonyMethod(typeof(FdServerProbe), nameof(ReapplyAfter)));
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
    private static void Stamp(ItemDrop.ItemData item, bool __result) { if (__result) Say("STAMP " + (item?.m_dropPrefab ? item.m_dropPrefab.name : "?")); }
    private static void Expire(ItemDrop.ItemData item) => Say("EXPIRE " + (item?.m_dropPrefab ? item.m_dropPrefab.name : "?"));

    private IEnumerator Run()
    {
        while (ZNet.instance == null || ObjectDB.instance == null || ZNetScene.instance == null || !ZNet.instance.IsServer() || Time.time < 30f) yield return new WaitForSeconds(2f);
        Say("server dedicated=" + ZNet.instance.IsDedicated());
        for (var round = 0; round < 14; round++)
        {
            yield return new WaitForSeconds(round == 0 ? 25f : 60f);
            Say("--- round " + round + " peers=" + ZNet.instance.GetPeerConnections());
            Conversions(); Say("CONVT now " + ConvCounts());
            foreach (var name in Foods)
            {
                var item = ObjectDB.instance.GetItemPrefab(name)?.GetComponent<ItemDrop>()?.m_itemData;
                if (item == null) { Say("RULE " + name + " no item"); continue; }
                var rule = Traverse.Create(AccessTools.Method("FineDining.SpoilagePolicy:Resolve").Invoke(null, new object[] { item }));
                Say("RULE " + name + " state=" + rule.Property("State").GetValue() + " group=" + rule.Property("Group").GetValue() + " hours=" + (rule.Property("LifetimeTicks").GetValue<long>() / 36000000000.0).ToString("F1") + " rot=" + rule.Property("ReplacementPrefab").GetValue());
            }
        }
    }

    private static void Conversions()
    {
        foreach (var piece in new[] { "piece_cookingstation", "piece_cookingstation_iron", "piece_oven" })
        {
            var s = ZNetScene.instance.GetPrefab(piece)?.GetComponent<CookingStation>();
            Say("CONV " + piece + " total=" + (s == null ? -1 : s.m_conversion.Count) + " match=" + (s == null ? "" : string.Join(",", s.m_conversion.Where(c => c.m_from != null && Foods.Contains(c.m_from.name)).Select(c => c.m_from.name + ">" + (c.m_to != null ? c.m_to.name : "null")))));
        }
    }
}

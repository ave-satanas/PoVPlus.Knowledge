// Sanitized snapshot for reading. Generated from tools/WorldCensus/Plugin.cs.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;
using UnityEngine;

// Test only, server side. Counts world objects by prefab after load, lists each unknown prefab's objects and fields, and finds PlanBuild items stored in containers or stands.
[BepInPlugin("0.worldcensus", "WorldCensus", "2.0.0")]
public class WorldCensus : BaseUnityPlugin
{
    private static readonly string[] PlanItems = { "PlanHammer", "BlueprintRune", "piece_bpcenterpoint" };
    private void Start() => StartCoroutine(Run());
    private static string Short(string s) => s.Length > 90 ? s.Substring(0, 90) + "..." : s;
    private IEnumerator Run()
    {
        while (ZNet.instance == null || ZDOMan.instance == null || ZNetScene.instance == null || !ZNet.instance.IsServer() || Time.time < 30f) yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(25f);
        var o = Path.Combine(Paths.BepInExRootPath, "census.txt");
        var zdos = ((IDictionary)AccessTools.Field(typeof(ZDOMan), "m_objectsByID").GetValue(ZDOMan.instance)).Values.Cast<ZDO>().ToList();
        var lines = new List<string>();
        var groups = zdos.GroupBy(z => z.GetPrefab()).Select(g => new { hash = g.Key, n = g.Count(), go = ZNetScene.instance.GetPrefab(g.Key), list = g.ToList() }).ToList();
        var unknown = groups.Where(g => g.go == null).OrderByDescending(g => g.n).ToList();
        lines.Add("world=" + ZNet.instance.GetWorldName() + " zdos=" + zdos.Count + " prefabs=" + groups.Count + " unknownPrefabs=" + unknown.Count + " unknownObjects=" + unknown.Sum(g => g.n));
        foreach (var g in unknown)
        {
            lines.Add("UNKNOWN " + g.hash + " x" + g.n);
            foreach (var z in g.list)
            {
                var id = z.m_uid;
                lines.Add("  OBJ pos=" + z.GetPosition().ToString("F1") + " sector=" + z.GetSector() + " type=" + z.Type + " persistent=" + z.Persistent + " owner=" + (z.GetOwner() != 0) + " distToOrigin=" + z.GetPosition().magnitude.ToString("F0"));
                ZDOExtraData.GetData(id, out var floats, out var vec3s, out var quats, out var ints, out var longs, out var strings, out var bytes, out var connection);
                foreach (var kv in strings) lines.Add("    str " + kv.Key + "=" + Short(kv.Value));
                foreach (var kv in ints) lines.Add("    int " + kv.Key + "=" + kv.Value);
                foreach (var kv in floats) lines.Add("    flt " + kv.Key + "=" + kv.Value);
                foreach (var kv in longs) lines.Add("    lng " + kv.Key + "=" + (kv.Key == ZDOVars.s_creator ? "(creator set)" : kv.Value.ToString()));
                foreach (var kv in vec3s) lines.Add("    vec " + kv.Key + "=" + kv.Value);
                foreach (var kv in bytes) lines.Add("    byt " + kv.Key + " len=" + kv.Value.Length);
            }
        }
        int stored = 0, scanned = 0;
        foreach (var z in zdos)
        {
            var name = z.GetString(ZDOVars.s_item);
            if (PlanItems.Contains(name)) { stored++; lines.Add("STAND " + name + " in " + (ZNetScene.instance.GetPrefab(z.GetPrefab())?.name ?? z.GetPrefab().ToString()) + " at " + z.GetPosition().ToString("F1")); }
            var items = z.GetByteArray(ZDOVars.s_items);
            if (items == null) continue;
            scanned++;
            var text = Encoding.GetEncoding(28591).GetString(items);
            foreach (var p in PlanItems.Where(text.Contains)) { stored++; lines.Add("CONTAINER " + p + " in " + (ZNetScene.instance.GetPrefab(z.GetPrefab())?.name ?? z.GetPrefab().ToString()) + " at " + z.GetPosition().ToString("F1")); }
        }
        foreach (var g2 in ZoneSystem.instance.m_locationInstances.Values.GroupBy(l => l.m_location != null ? l.m_location.m_prefab.Name : "null").OrderBy(g2 => g2.Key)) lines.Add("LOC " + g2.Key + " " + g2.Count());
        lines.Add("containersScanned=" + scanned + " planItemsStored=" + stored);
        lines.Add("DONE");
        File.WriteAllLines(o, lines);
    }
}
